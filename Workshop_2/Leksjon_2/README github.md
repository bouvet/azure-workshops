
# Leksjon 2: Infrastructure as Code - Github actions

​Infrastruktur som kode (IaC) er en praksis som lar deg administrere og klargjøre skyressursene dine gjennom kode, noe som gjør infrastrukturen mer konsekvent, repeterbar og skalerbar. Ved å bruke IaC kan du versjonskontrollere infrastrukturen din, redusere manuelle feil og effektivisere distribusjonsprosessene dine, noe som til slutt fører til mer effektive og pålitelige skyoperasjoner. I denne veiledningen lærer du hvordan du automatiserer infrastrukturdistribusjonene dine ved hjelp av GitHub Actions, et kraftig CI/CD-verktøy integrert med GitHub.

I tillegg skal vi utforske Bicep, et domenespesifikt språk (DSL) for deklarativ distribusjon av Azure-ressurser, noe som forenkler redigeringsopplevelsen og forbedrer lesbarheten til infrastrukturkoden din.

## Ett repo, to workflows

Vi har nå en pipeline for å bygge og distribuere applikasjonen og en for å opprette ressurser i Azure. Vi legger begge pipeline i samme repo, men det er også mulig å legge dem i forskjellige repo. Vi legger også samme trigger på begge workflows slik at både applikasjonskode og infrastruktur er synkronisert.

Hvis vi ønsker å sette opp en inner/outer loop workflow så kan vi gjøre det med å bruke Github environments.
​
>**Forutsetninger** for denne leksjonen er at du har både en egen github konto og en egen Azure subsription. Enten som en del av MSDN eller en demo Azure konto.

## Start prosjekt

>Hopp over dette punktet hvis du allerede har en fork fra leksjon 1, samt konfigurert tilgang til Azure for Github actions.

Se **leksjon 1** for hvordan du gjør dette. Hopp over skrittet med å lage en web applikasjon.

- Fork repo fra Bouvet sin github konto.
- Opprett ressursgrupper i Azure.
- Legg til test og produksjonsmiljø i ditt Github repo.
- Konfigurer tilgang til Azure for Github actions. (Siden vi bruker samme miljø i begge leksjoner trenger du ikke å legge til nye tilganger hvis du har gjort det tidligere.)
  - Legg til app registration i Azure
  - Gi appen tilganger den trenger til ressurser i Azure.
  - Opprett federated credetials i Azure
  - Legg til hemmeligheter i Github secrets på repo nivå.

> Slutt på hopp-over avsnitt.

## Opprett workflow fil

- Opprett **\\.github\workflows** folder i prosjektet ditt. **\Workshop_2\Start\AzureWorkshopInfrastruktur**
- Legg til en ny yaml fil for github action.
  
```yaml
# This workflow validates Bicep Infrastructure as Code (IaC) files

# Define the workflow name that appears in GitHub Actions
name: IaC bicep validate

# Set required permissions for the workflow
# - id-token: write is needed for Azure OIDC authentication
# - contents: read is needed to read the repository contents
permissions:
  id-token: write
  contents: read

# Define when this workflow should run
on:
  push:
    branches:
      - main  # Run on every push to main branch
  pull_request:
    branches:
      - main  # Run on pull requests targeting main branch
  workflow_dispatch:  # Allow manual trigger of the workflow

jobs:
    deploy-infrastructure:
      runs-on: ubuntu-latest  # Use Ubuntu as the runner operating system
      env:
        RESOURCE_GROUP: your-resource-group-name  # Define environment variable for Azure resource group
      steps:
        # Step 1: Check out the repository code
        - name: Checkout
          uses: actions/checkout@v4
  
        # Step 2: Authenticate with Azure using OIDC
        - name: Azure Login
          uses: azure/login@v2
          with:
            client-id: ${{ secrets.AZURE_CLIENT_ID }}        # Azure App Registration client ID
            tenant-id: ${{ secrets.AZURE_TENANT_ID }}        # Azure tenant ID
            subscription-id: ${{ secrets.AZURE_SUBSCRIPTION_ID }}  # Azure subscription ID

        # Step 3: Validate Bicep syntax and build
        - name: Bicep lint
          uses: azure/cli@v1
          with:
            inlineScript: |
              # Build checks if Bicep can compile to ARM
              az bicep build --file bicep/main.bicep
              # Lint checks for best practices and potential errors
              az bicep lint --file bicep/main.bicep

        # Step 4: Validate the deployment against Azure
        - name: Bicep Validate
          uses: Azure/cli@v1
          with:
            inlineScript: |
              # Validates if the template is valid for deployment
              az deployment group validate \
                --name validate-${{ github.run_id }} \
                --resource-group ${{ env.RESOURCE_GROUP_NAME }} \
                --template-file ./bicep/main.bicep 

        - name: What-If Deployment
          uses: azure/arm-deploy@v1
          with:
            resourceGroupName: your-resource-group
            template: ./path/to/template.json
            parameters: ./path/to/parameters.json
            deploymentMode: Validate
            additionalArguments: --what-if
```

Lag en ny folder som du kaller for bicep. I den oppretter du en ny fil for bicept template.

- Kall den **main.bicep**.

```json
// This is the main deployment file that orchestrates our Azure infrastructure

// Parameters allow you to provide values at deployment time
// The @description decorator provides documentation for the parameter
@description('The environment name. "dev" and "prod" are valid values.')
param environmentName string = 'dev'  // Default value if none is provided
param location string = 'norwayeast'  // Location where resources will be deployed

// Variables are used to store reusable values
// Here we define tags that will be applied to all resources
var tags = {
  environment: environmentName 
  project: 'Azure Workshop'
}

// targetScope defines at which level this template operates
// Options are: 'resourceGroup', 'subscription', 'managementGroup', 'tenant'
targetScope = 'resourceGroup'

// Modules are used to break down complex templates into smaller, reusable parts
// This module deploys a storage account using a separate template file
module storageAccount 'modules/storageAccount.bicep' = {
  name: 'st${environmentName}'  // Dynamic naming using string interpolation
  params: {
    location: location  // Passing parameters to the module
    tags: tags         // Passing our tags to ensure consistent tagging
  }
}
```

I bicep folderen lager du en ny folder som du kaller for **modules**. Vi ønsker å dele bicep malene opp i moduler for lettere kunne organisere dem, samt gjenbruke moduler flere steder.

- Opprett en fil i modul folderen som du kaller for **storageAccount.bicep**.

```json
// This module defines an Azure Storage Account resource

// Input parameters that will be provided by the parent template
param location string        // Azure region where the storage account will be created
param tags object           // Resource tags for organization and billing

// Variables help us define reusable values and improve readability
var storageAccountName = 'st${uniqueString(resourceGroup().id)}'  // Generate a unique storage account name

// Storage Account resource definition
resource storageAccount 'Microsoft.Storage/storageAccounts@2021-09-01' = {
  // Name must be globally unique, lowercase, and 3-24 characters long
  name: storageAccountName
  location: location
  tags: tags
  
  // SKU defines the type and replication strategy
  sku: {
    name: 'Standard_LRS'    // Locally-redundant storage (cheapest option)
  }
  
  // Kind specifies the type of storage account
  kind: 'StorageV2'         // General-purpose v2 accounts support all storage services
  
  // Properties define the configuration of the storage account
  properties: {
    minimumTlsVersion: 'TLS1_2'                // Enforce minimum TLS version for security
    supportsHttpsTrafficOnly: true             // Only allow secure connections
    allowBlobPublicAccess: false               // Disable public access for security
    accessTier: 'Hot'                          // Hot tier for frequently accessed data
  }
}

// Outputs allow the parent template to access information about the deployed resource
output storageAccountId string = storageAccount.id
output storageAccountName string = storageAccount.name
```

### Legg til deploy

Nå som vi har validert at vår bicep mal virker skal vi legge til et **deploy** skritt. Vi kan legge tl deploy som en ege jobb, men her legger vi det til som et eget step. Men først legger vi til en sjekk om de foregående skrittene genererte feil eller ikke. Først konfigurerer vi inline skript til å stoppe ved feil:

```yaml
          with:
            inlineScript: |
              set -e  # Exit on any error
              echo "Running Bicep build..."
```

```yaml
        # Step 5: Deploy if all validation passes
        - name: Deploy Bicep
          if: success()  # Only deploys if validation succeeds
          id: deploy
          uses: Azure/arm-deploy@v1
          with:
            scope: resourcegroup     # Deploy to resource group scope
            resourceGroupName: ${{ env.RESOURCE_GROUP_NAME }}
            template: ./bicep/main.bicep
            parameters:              # Parameters passed to Bicep template
              environmentName=${{ env.ENVIRONMENT_NAME }}
              location=${{ env.AZURE_LOCATION }}
            failOnStdErr: false     # Continue on non-critical errors
```

# Del to. Organsiere Github pipeline i to flyter

Så langt har vi skrevet all yaml og bicept mer eller mindre rett fram uten å tenke så mye på organisering og struktur. Det har vært bra for å få en følelse av hvordan skrive yamle og bicep maler. Nå skal vi strukturere IaC prosjektet vårt for å gjøre det mer oversiktelig, øke gjenbruk og sikkerhet ved å skille mellom applikasjonskode og infrastruktur.

Vi skal nå opprette infrastructur for å kjøre web app vi publiserte i leksjon 1. I den leksjonen opprettet vi infrastrukturen manuelt i protalen. Nå skal vi gjøre det med bicept template og yaml skript i en Github Action pipeline.

## Rydde opp i Azure

- Gjenbruk ressursgruppe fra leksjon 1. (For å gjøre det litt enklere for oss selv i denne leksjonen oppretter vi ikke ressursgruppen med bicep.)
- Slett web app og Appservice du opprettet i leksjon 1.

## Omorganisere IaC prosjektet vårt

Vi skal nå opprette to workflows, en for å bygge og distribuere applikasjonskode og en for å opprette infrastruktur i Azure. Ved å dele i to arbidsflyter kan vi både begrense hvor ofte infrastruktur malene blir publisert ved å legge på filter og konfigurere ulik sikkerhet for distribusjon av kode og opprettelse av infrastruktur.

- Opprett to nye foldere hvor under **.github\worksflows**.
- Lag en for infrastruktur yaml filer. Kall den for **infrastructure**.
- Opprett en for applikasjons skript. Kall denne for **application**.
- I applikasajonsfolderen skap en ny fil. Kall den for **application.yml**.
- I infrastrukturfolderen opprett en ny fil og kall den for **infrastructur.yml**.
- I applikasjonfila kopierer du inn all yaml vi tidligere har skrevet som omhandler pålogging i Azure, bygg, test og distribusjon av applikasjonkode.
- Gjør tilsvarende for infrastuktur. I tellegg må vi opprette ny bicep maler for å kunne opprette infrastrastruktur for å kunne kjøre applikasjonen fra leksjon 1.
 
> Jobber her
>
La oss starte med main.bicep fil. Denne fila er den overordnede bicep malen og kaller andre moduler med parametre for test og produksjon for å gjenbruke moduler. Den bruker **modules/webapp.bicep** og **modules/storage.bicep**

```json
// This is the main Bicep file that orchestrates the deployment of our Azure resources.
// Bicep is a Domain Specific Language (DSL) that represents your Azure infrastructure as code.

// Parameter declarations with decorators for validation
// @allowed decorator restricts the possible values to a predefined set
@allowed([
  'test'
  'prod'
])
param environment string

// Parameters use decorators to enforce naming conventions and provide metadata
// @minLength and @maxLength ensure the name meets Azure's requirements
// @description provides documentation for the parameter
@minLength(2)
@maxLength(36)
@description('The name of the web site.')
param webSiteName string = 'app-${uniqueString(resourceGroup().id)}'

@minLength(3)
@maxLength(20)
@description('The name of the Azure Storage account. May contain numbers and lowercase letters only!')
param storageAccountName string = 'st-azskl-images'

@description('The SKU of the App Service Plan')
param appServiceSku string = 'F1'

@description('The location in which the Azure Storage resources should be deployed.')
param location string = resourceGroup().location

// Variables are used to compute values that will be reused throughout the template
// They help maintain consistency and reduce repetition
var servicePlanName = 'asp-${webSiteName}'
var blobContainerName = 'ci-image-${webSiteName}'
var websiteNameWithEnvironment = '${webSiteName}-${environment}'
var storageAccountNameWithEnvironment = '${storageAccountName}${environment}'

// Tags are metadata attached to Azure resources
// They are useful for organizing, managing, and tracking costs
var tags = {
  environment: environment
  project: 'Azure Workshop'
}

// Conditional expression example: determines the tier based on the SKU
var skuTier = appServiceSku == 'F1' ? 'Free' : 'Basic'

// Modules are reusable components that help organize and maintain your infrastructure
// They promote code reuse and separation of concerns
module webApp 'modules/webapp.bicep' = {
  name: 'webApp'  // This is the deployment name, not the resource name
  params: {
    // Pass parameters to the module for configuration
    location: location
    appServicePlanName: servicePlanName
    webAppName: websiteNameWithEnvironment
    skuName: appServiceSku
    skuTier: skuTier
    tags: tags
  }
}

// Another module for storage resources
module storage 'modules/storage.bicep' = {
  name: 'storage'
  params: {
    location: location
    storageAccountName: storageAccountNameWithEnvironment
    containerName: blobContainerName
    tags: tags
  }
}

// Outputs make specific values available after deployment
// They can be used by other templates or scripts
output webAppName string = webApp.outputs.webAppName
output webAppHostName string = webApp.outputs.webAppHostName
output storageAccountName string = storage.outputs.storageAccountName
output blobContainerName string = storage.outputs.blobContainerName
```

Bicep for web applikasjonen

```json
param location string
param appServicePlanName string
param webAppName string
param skuName string
param skuTier string
param tags object = {}

resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: appServicePlanName
  location: location
  tags: tags
  kind: 'app'
  sku: {
    name: skuName
    tier: skuTier
  }
}

resource webApp 'Microsoft.Web/sites@2022-03-01' = {
  name: webAppName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
  }
}

output webAppName string = webApp.name
output webAppHostName string = webApp.properties.defaultHostName
```

Bicep for Azure blob container

```json
param location string
param storageAccountName string
param containerName string
param tags object = {}

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: storageAccountName
  location: location
  tags: tags
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    minimumTlsVersion: 'TLS1_2'
    allowBlobPublicAccess: false
    supportsHttpsTrafficOnly: true
    accessTier: 'Hot'
  }
}

resource blobServices 'Microsoft.Storage/storageAccounts/blobServices@2022-09-01' = {
  parent: storageAccount
  name: 'default'
}

resource blobContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2022-09-01' = {
  parent: blobServices
  name: containerName
  properties: {
    publicAccess: 'None'
  }
}

output storageAccountName string = storageAccount.name
output storageAccountKey string = storageAccount.listKeys().keys[0].value
output blobContainerName string = containerName
```



> Jobber her
## Klargjør parameter-filer for test-miljøet og prod

Parameter-filen i ARM-templates (kalt *azuredeploy.{environment}.parameters.json* i dette prosjektet), gjør det mulig å spinne opp et sett med Azure ressurser med ulik konfigurasjon til forskjellige miljøer. For hvert miljø kan du legge inn parameters, eksempel hvis du vil oppskalere produksjonsmiljøet i forhold til test. Det er også vanlig å definere navn på ressurser her.
​

1. Gjenbruk ressursgruppen og ressurser du opprettet for brukte for test-miljøet og . Slett Web App'en og App Service Plan du opprettet i den gruppen. Merk deg navnet på Web App'en din, da du skal gjenbruke denne i steg 3.
2. I `azuredeploy.json` Fyll ut default verdier for `webSiteName` og `storageAccountName` uten miljø. Templaten vil concate disse verdiene med parameteren `environment` som du skal fylle ut i neste steg.
3. Kopier `azuredeploy.test.parameters.json` og opprett `azuredeploy.prod.parameters.json` , Dette vil være parameter-filene dine for test og prod.
4. I `azuredeploy.prod.parameters.json`, endre value i `environment` fra 'test' -> 'prod' inn. Bruk gjerne samme navn base navn ( uten miljø) som i forrige leksjon.
5. Valider templaten din ved å gjøre en `az deployment group what-if` deploy via Azure CLI
  a. Åpne Powershell eller en terminal
  b. Log inn via ``az login``
  c. Sjekk at du har riktig subscription valgt eller endre subscription ved å kjøre ```az account set --subscription "{subscription name or id}"```
  d. Naviger til riktig mappe (for eksempelet under så bør man være i samme mappe som AzureWorkshopInfrastruktur.sln filen)
  e. Kjør kommandoen

  ```
  az deployment group what-if `
    --name {ExampleDeployment} `
    --resource-group {dinRessursGruppe} `
    --template-file .\AzureWorkshopInfrastruktur\azuredeploy.json ` 
    --parameters '@AzureWorkshopInfrastruktur\azuredeploy.test.parameters.json' 
  ```

  f. Deploymenten bør da liste opp både keyvault, storage og en web app. Avhengig av hva slags ressurser du har slettet. Så betyr grønn/+: Ny ressurser, Lilla/~: Endringer på eksisterende ressurser, Oransje/-: Fjerning.

6. Selv om du får grønt her, så betyr det ikke nødvendigvis at navnet du har angitt er globalt unikt. Men du vil få en verifikasjon på at templaten din er gyldig og for spesielt ved endringer av allerede deployerte ressurser vil du enkelt kunne se endringer. Hvis du får noen feilmeldinger, så forsøk å rette dem og prøv på nytt.
7. Lag en commit og push den til ditt DevOps-repo.
​

## Build-pipeline

Lag en ny byggedefinisjon i Azure DevOps. Den vil være veldig kort, fordi det eneste den trenger å gjøre er å tilgjengeliggjøre ARM-templaten din til neste steg i kjeden (release-pipeline).
​

1. Lag en ny build-definisjon (Trykk på Pipelines->Pipelines->"Azure Repos Git (YAML)").
2. Velg git-repoet du laget i forrige sted.
3. Velg "ASP. NET Core" helt øverst, og gi YAML-filen et passende navn (f.eks. azure-pipelines-infra.yml).
4. Fjern alt under "steps:" (to linjer).
5. Legg til "Publish Build Artifacts
6. I "Path to publish" endres $(Build.ArtifactStagingDirectory) til $(System.DefaultWorkingDirectory)
7. Lagre og commit til git

​
Når du har fått build-pipelinen din til å kjøre, sjekk at bygge-artifacten din inneholder ARM-templaten. Dette gjøres enten gjennom loggen på jobben eller siden som viser jobben som ble kjørt (under "Related" skal det stå "1 published"). Vi har nå publisert hele mappe strukturen, vi kunne selvfølgelig ha bare publisert infrastruktur mappen også ved å endre på "Path to publish".
​

## Release-pipeline

​
For å deploye infrastrukturen trenger å opprette en Release-pipeline som tar et bygg og deployer det ut til et miljø. I denne øvelsen skal du kun deploye infrastrukturen for test-miljøet. Vi lager en Release pipeline utenom YAML-filen for å vise en annen måte å ha release på. Dette kunne selvfølgelig også blitt gjort gjennom YAML.
​

1. Lag en egen release-pipeline for infrastruktur (Pipelines->Releases->New).
2. Her velger du "Empty job" helt øverst, og ikke noen ferdig template.
3. Gi steget "Stage 1" et nytt navn, og kall det f.eks. "Test". Dette vil være navnet på miljøet ditt.
4. Gi release-pipelinen din et navn, f.eks. "Infrastruktur" (på toppen av siden).
5. Velg artifact til venstre fra build-pipelinen du lagde i forrige oppgave, trykk "Add an artifact". Velg byggepipelinen du opprettet i forrige steg. Her kan du også velge om du vil spesifisere hvilket bygg du vil bruke for hver release, eller om den skal bruke siste bygget hver gang.
6. Trykk så på "Pre-deployment conditions" for Test-miljøet ditt. Her kan du velge om du ønsker at det skal bli deployet automatisk når du oppretter en release, eller om du må gjøre dette manuelt. Dette er liten oval runding til venstre for Test-boksen med et lyn og en person på.
7. Åpne så Tasks->"Navnet på din stage (f.eks. Test)" (menyen øverst står på pipeline, bytt til Tasks) og legg til tasken "ARM Template Deployment".
8. Velg så Azure Resource Manager connection som du valgte i forrige leksjon.
9. Velge Azure subscription, ressursgruppen du har laget og sett location til samme sted som ressursgruppen.
10. Åpne opp "Template" settings
11. I template feltet, finn stien til `azuredeploy.json`.
12. I template parameters feltet, finn stien til `azuredeploy.test.parameters.json`
13. Trykk så på "Save" og "Ok".
14. Trykk så på "Create Release" og lag en release
15. Gå inn i releasen
16. Klikk på deploy
​
Verifiser at ressursene dine blir deployet ut til ressursgruppen din.
​Forsøk å kjøre what if kommandoen igjen, og se om om det evenutelt oppstår noen endringer.

### Tilpass applikasjonen til å hente ut secrets fra Keyvault

I forrige leksjon lagret vi connection stringen til storage accounten direkte i konfigen til applikasjonen, dette er worst practise.
I denne leksjonen skal vi istedenfor opprette ett eget keyvault, lagre account keyen under secrets i keyvault og gi applikasjonen rettigheter til å hente ut secrets fra keyvaulten.

Åpne `azuredeploy.json` og gjør følgende endringer:

* Under variables legg til følgende:

```
"keyvaultNameWithEnvironment": "[concat(parameters('webSiteName'),'-',parameters('environment'),'-kv')]"
```

* Søk etter `Microsoft.Web/sites` og legg inn følgende

```
"identity": {
  "type": "SystemAssigned"
}
```

og under properties.siteConfig.appsettings, legg inn følgende setting:

```
{
  "name": "KeyVaultUri",
  "value": "[reference(variables('keyVaultName')).vaultUri]"
},
{
  "name": "AzureStorageConfig:AccountName",
  "value": "[variables('storageAccountNameWithEnvironment')]"
},
```

Dette oppretter en managed identity som applikasjonen kan bruke for å autentisere seg mot Azure AD istedenfor å bruke f.eks connection strings mot azure tjenester.

* Under resources legg inn følgende:

```
{
      "type": "Microsoft.KeyVault/vaults",
      "apiVersion": "2021-10-01",
      "name": "[variables('keyvaultNameWithEnvironment')]",
      "location": "[parameters('location')]",
      "properties": {
        "enabledForDeployment": false,
        "enabledForDiskEncryption": false,
        "enabledForTemplateDeployment": false,
        "enablePurgeProtection": true,
        "enableRbacAuthorization": false,
        "enableSoftDelete": true,
        "softDeleteRetentionInDays": 90,
        "tenantId": "[subscription().tenantId]",
        "sku": {
          "name": "standard",
          "family": "A"
        },
        "accessPolicies": [
        ],
        "publicNetworkAccess": "Enabled"
      }
    }
```

Dette oppretter ett keyvault,
Merk: at `"publicNetworkAccess": "Enabled"`, betyr at keyvaultet er tilgjengelig på internett. En god prakis er å opprette ett eget vnet, som vi kan bruke for at app servicen skal ha tilgang til key vaulten.

*For å ikke skape sirkulær referanse, må vi gi tilgang til app servicen i ett eget steg.
I ressurs arrayet, legg inn følgende.

```
{
  "type": "Microsoft.KeyVault/vaults/accessPolicies",
  "name": "[concat(variables('keyvaultNameWithEnvironment'), '/add')]",
  "apiVersion": "2018-02-14",
  "properties": {
    "accessPolicies": [
      {
        "tenantId": "[subscription().tenantId]",
        "objectId": "[reference(variables('websiteNameWithEnvironment'),'2018-02-01', 'Full').identity.principalId]",
        "permissions": {
          "keys": [ "all" ],
          "secrets": [ "all" ],
          "certificates": [ "all" ],
          "storage": [ "all" ]
        }
      }
    ]
  }
}
```

Dette gir managed identityen til app servicen full tilgang via keyvaultet sin access policy.

* Legg så inn følgende:

```
{
      "type": "Microsoft.KeyVault/vaults/secrets",
      "apiVersion": "2018-02-14",
      "name": "[concat(variables('keyvaultNameWithEnvironment'), '/AzureStorageConfig--AccountKey')]",
      "location": "[parameters('location')]",
      "dependsOn": [
          "[resourceId('Microsoft.KeyVault/vaults', variables('keyvaultNameWithEnvironment'))]",
          "[variables('storageAccountNameWithEnvironment')]"
      ],
      "properties": {
          "value": "[listKeys(resourceId('Microsoft.Storage/storageAccounts', variables('storageAccountNameWithEnvironment')), '2019-04-01').keys[0].value]"
      }
    }

```

Dette lager en secret i keyvaulten som inneholder storage account key'en. Denne skal vi i neste steg hente ut automatisk.

Commit og push koden til devops. Trigg en ny deploy til test miljø.

### Åpne Azureworkshop.sln

Legg inn følgende Nuget pakker: `Azure.Extensions.AspNetCore.Configuration.Secrets` og `Azure.Identity`.
I Program.cs legger du inn følgende i CreateWebHostBuilder funksjonen mellom linjene: WebHost.CreateDefaultBuilder(args) og .UseStartup<Startup>();

```
.ConfigureAppConfiguration((context, builder) =>
{
    var config = builder.Build();
    if (!string.IsNullOrEmpty(config["KeyVaultUri"]))
    {
        var secretClient = new SecretClient(new Uri(config["KeyVaultUri"]), new DefaultAzureCredential());
        builder.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
    }
})
```

Denne kodebiten her forteller applikasjonen til å hente ut secrets fra keyvault, med managed identitien som vi nettopp opprettet ved oppstart av applikasjonen.

Modellen vår støtter ikke `AccountKey` og `AccountName`, Gå til Models/AzureStorageConfig.cs
Bytt ut klassen med følgende:

```
public class AzureStorageConfig
{
    public string AccountKey { get; set; }
    public string AccountName { get; set; }
    public string ImageContainer { get; set; }
}

```

I StorageService, lag en private readonly string. og bytt ut følgende kodesnutter i UploadFileToStorage og GetImageUrls med

```
private readonly AzureStorageConfig _storageConfig;
private readonly string _storageConnectionString;

public StorageService(IOptions<AzureStorageConfig> storageConfig)
{
    _storageConfig = storageConfig != null ? storageConfig.Value : throw new ArgumentNullException(nameof(storageConfig));

    _storageConnectionString = $"DefaultEndpointsProtocol=https;AccountName={_storageConfig.AccountName};AccountKey={_storageConfig.AccountKey};EndpointSuffix=core.windows.net";
}
```

```
BlobServiceClient blobServiceClient = new BlobServiceClient(_storageConnectionString);
```

Under helpers/StorageConfigValidator.cs

Fjern

```
if (storageConfig.ConnectionString == string.Empty)
{
    validation.AddError("ConnectionString", "ConnectionString key is empty. Check configuration.");
}
```

Og erstatt med

```
if (storageConfig.AccountKey == string.Empty)
{
    validation.AddError("AccountKey", "AccountKey key is empty. Check configuration.");
}

if (storageConfig.AccountName == string.Empty)
{
    validation.AddError("AccountName", "AccountName key is empty. Check configuration.");
}

```

### Endring av miljø

Du ønsker å gjøre det mulig å sette størrelsen på App Service Planen til forskjellige størrelse basert på om det er et test-miljø eller et produksjonsmiljø. For å gjøre dette må du legge inn et nytt steg i release pipelinen. Dette kan du gjøre ved å legge til en ny stage. Trykk "+ Add" og "New Stage" og velg "Empty Job" øverst. Repeter så det du gjorde for Test stagen for Prod (endre navn, gå til tasks->prod og legg til "Arm Template Deployment" og velg riktige filer, subscription, resource group og location). Hvis du ikke har en template fil for prod. Så kan du lage en ny ved å kopiere `azuredeploy.test.parameters.json` og kalle den `azuredeploy.prod.parameters.json`, bytt ut navn på 'webSiteName' og 'storageAccountName' og commit og push endringene dine. Gå tilbake til å editere på release-pipelinen og legg til riktige verdier i "Arm Template Deployment" tasken for Prod stagen (bygg må bli ferdig før du kan velge `azuredeploy.prod.parameters.json` som parameter fil). Nå har du to stages i release pipelinen og disse kan nå ha forskjellige verdier via `azuredeploy.test.parameters.json` og `azuredeploy.prod.parameters.json`.
​

1. Editer så `azuredeploy.parameters.test.json`
1. Legg inn parameter for `appServiceSku`.
1. Sett denne verdien til en f.eks. F1, slik at den overskriver parameteren D1 som er satt som default verdi.
2. Sjekk så inn koden, lag et nytt bygg og kjør en release og deploy (hvis du ikke har satt på automatisk deploy).
​
Se så at SKU har oppdatert seg på App Service Planen din via *portal.azure.com*.
​

## Legge til ny ressurs

​
I neste leksjon skal du bruke Application Insights for å overvåke løsning. For å gjøre dette må du legge til selve ressursen i miljøet ditt.
​

1. Editer `azuredeploy.json`. Se <https://docs.microsoft.com/en-us/azure/templates/microsoft.insights/2015-05-01/components> for strukturen på denne komponenten. Legg også til en parameter til scriptet for navnet på komponenten. Vårt forslag er:

```
    {
      "name": "[variables('appInsightsNameWithEnvironment')]",
      "type": "Microsoft.Insights/components",
      "apiVersion": "2015-05-01",
      "location": "[parameters('location')]",
      "tags": {},
      "kind": "string",
      "properties": {
        "Application_Type": "web",
        "Flow_Type": "Bluefield",
        "Request_Source": "rest"
      }
    }
```

2. Fortsett i `azuredeploy.json`: her må du også legge til noen appSettings på website-ressursen din (i tillegg til de som allerede finnes), som peker til Application Insights-ressursen din.

```
          "appSettings": [
            ...eksisterende innstillinger her
            {
              "name": "APPLICATIONINSIGHTS_CONNECTION_STRING",
              "value": "[reference(concat('microsoft.insights/components/', variables('appInsightsNameWithEnvironment')), '2015-05-01').ConnectionString]"
            },
            {
              "name": "APPINSIGHTS_PORTALINFO",
              "value": "ASP.NETCORE"
            },
            {
              "name": "APPINSIGHTS_PROFILERFEATURE_VERSION",
              "value": "1.0.0"
            },
            {
              "name": "APPINSIGHTS_SNAPSHOTFEATURE_VERSION",
              "value": "1.0.0"
            },
```

3. Du må også legge inn en avhengighet mellom Web App-ressursen din og Application Insights-ressursen. Dette fordi at Application Insights må opprettes først, siden Web App har en referanse til denne.
Vår dependsOn på Web App ser slik ut:

```
      "dependsOn": [
        "[resourceId('Microsoft.Web/serverfarms', variables('servicePlanName'))]",
        "[resourceId('Microsoft.Insights/components', variables('appInsightsNameWithEnvironment'))]"
      ]
```

​
4. I `azuredeploy.json` må vi sørge for at appInsightsNameWithEnvironment blir en del av templaten. Legg til variabelen under nederst i variabel-seksjonen
​

```
    "appInsightsNameWithEnvironment": "[concat(parameters('webSiteName'),'-',parameters('environment'),'-ai')]"
```

5. Sjekk inn endringene dine, og vent til at bygget ditt har gått igjennom.
6. Lag en ny release og valider at komponenten blir opprettet.
​
Du er nå klar for å begi deg ut på neste leksjon.
