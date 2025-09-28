
# Leksjon 2: Infrastructure as Code - Github actions

​Infrastruktur som kode (IaC) er en praksis som lar deg administrere og klargjøre skyressursene dine gjennom kode, noe som gjør infrastrukturen mer konsekvent, repeterbar og skalerbar. Ved å bruke IaC kan du versjonskontrollere infrastrukturen din, redusere manuelle feil og effektivisere distribusjonsprosessene dine, noe som til slutt fører til mer effektive og pålitelige skyoperasjoner. I denne veiledningen lærer du hvordan du automatiserer infrastrukturdistribusjonene dine ved hjelp av GitHub Actions, et kraftig CI/CD-verktøy integrert med GitHub.

I tillegg skal vi utforske Bicep, et domenespesifikt språk (DSL) for deklarativ distribusjon av Azure-ressurser, noe som forenkler redigeringsopplevelsen og forbedrer lesbarheten til infrastrukturkoden din.

## Ett repo, to workflows

Vi har nå en pipeline for å bygge og distribuere applikasjonen og en for å opprette ressurser i Azure. Vi legger begge pipeline i samme repo, men det er også mulig å legge dem i forskjellige repo. Vi legger også samme trigger på begge workflows slik at både applikasjonskode og infrastruktur er synkronisert. Men for å forhindre at vi deployer infrastructur hver gang vi gjør en merge på applikasjonskode, legger vi på et filter på hvilken folder som det er gjort endringer i. Det vil si at infrastruktur vil bare bli distribuert når det er gjort endringer i en fil under folderen **infrastruktur**.
​
>**Forutsetninger** for denne leksjonen er at du har både en egen github konto og en egen Azure subsription. Enten som en del av MSDN eller en demo Azure konto.

## Start prosjekt

>Hopp over dette punktet hvis du allerede har en fork fra leksjon 1, samt konfigurert tilgang til Azure for Github actions.

Se **leksjon 1** for hvordan du gjør dette i mer detalj, men hopp over skrittet i leksjon 1 med å lage en web applikasjon.

- **Fork** repo fra Bouvet sin github konto og klon det ned på din egen pc. (Med fork lager du en egen kopi av repo i den Github konto og kan jobbe på den uten å bli forstyrret av andre deltakere.)
- Opprett ressursgrupper i Azure.
- Legg til test og produksjonsmiljø i ditt Github repo.
- Konfigurer tilgang til Azure for Github actions. (Siden vi bruker samme miljø i begge leksjoner trenger du ikke å legge til nye tilganger hvis du har gjort det tidligere.)
  - Legg til app registration i Azure
  - Gi appen tilganger den trenger til ressurser i Azure.
  - Opprett federated credetials i Azure
  - Legg til hemmeligheter i Github secrets på repo nivå.

> Slutt på hopp-over avsnitt. Fortsett her hvis du har gjort det overstående i leksjon 1.

### Infrastruktur bicep

Vi starter med å lage en Bicep mal som vi skal bruke til å opprette infrastruktur i Azure.

Lag en ny folder som du kaller for bicep. I den oppretter du en ny fil for bicept template. Denne malen vil vi bryte opp i flere deler for gjenbruk, men vi starter med **main.bicep**.

- Opprett **bicep** folder i rot på prosjektet ditt. **\Workshop_2\Start\AzureWorkshopInfrastruktur**
- Kall den **main.bicep**. Skriv inn følgende kode:

```json
// This is the main Bicep file that orchestrates the deployment of our Azure resources.
// Bicep is a Domain Specific Language (DSL) that represents your Azure infrastructure as code.

// Parameter declarations with decorators for validation
// @allowed decorator restricts the possible values to a predefined set
@allowed([
  'test'
  'production'
])
param environment string

// Parameters use decorators to enforce naming conventions and provide metadata
// @minLength and @maxLength ensure the name meets Azure's requirements
// @description provides documentation for the parameter
@minLength(2)
@maxLength(36)
@description('The name of the web site.')
param webSiteName string = 'app-${uniqueString(resourceGroup().id)}'

// Storage accounts have strict naming rules - only lowercase letters and numbers
@minLength(3)
@maxLength(20)
@description('The name of the Azure Storage account. May contain numbers and lowercase letters only!')
param storageAccountName string = 'stazsklimages'

// The SKU of the App Service Plan
@description('The SKU of the App Service Plan')
param appServiceSku string = 'F1'

// The location in which the Azure Storage resources should be deployed.
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

### Infrastruktur moduler

I main.bicep malen refererer vi til to moduler som vi ønsker å gjenbruke for test og produksjons miljøer. Vi starter med å opprette en ny folder under bicep folderen for å samle modulene der. (Vi har bare to, men det å bruke foldere blir fort nyttig i større prosjekt.)

- I bicep folderen lager du en ny folder som du kaller for **modules**.
- Opprett en fil i modul folderen som du kaller for **storage.bicep**.
- Og en fil som du kaller for **webapp.bicep**.

#### Storage

Her er bicep koden for mal for å opprette en blob storage i Azure.

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

#### Web app

Her er bicep koden for mal for å opprette en web app i Azure.

```json
// This is a Bicep module that deploys an Azure Web App along with its required App Service Plan
// Parameters allow us to make our template reusable across different environments
param location string // The Azure region where resources will be deployed
param appServicePlanName string // Name for the App Service Plan (the hosting plan)
param webAppName string // Name for the Web App that will be created
param skuName string // The SKU name defines the pricing tier size (e.g., F1, B1, S1)
param skuTier string // The tier type (e.g., Free, Basic, Standard)
param tags object = {} // Optional resource tags with a default empty object

// App Service Plan resource - this is the hosting infrastructure for your web app
// Think of it as the virtual machine or container that will run your application
resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: appServicePlanName
  location: location
  tags: tags
  kind: 'app' // Specifies this is for a regular web app (not a function app or container)
  sku: {
    name: skuName // Defines the compute resources available
    tier: skuTier // Defines the feature set available
  }
}

// Web App resource - this is your actual web application
// It runs on the App Service Plan defined above
resource webApp 'Microsoft.Web/sites@2022-03-01' = {
  name: webAppName
  location: location
  properties: {
    serverFarmId: appServicePlan.id // Links the web app to the App Service Plan
    httpsOnly: true // Security best practice: forces HTTPS for all traffic
  }
}

// Outputs allow other templates to reference properties of the deployed resources
output webAppName string = webApp.name // The name of the created web app
output webAppHostName string = webApp.properties.defaultHostName // The default URL where the web app will be available
```

### Github actions - yaml

For å deploye bicep malen vi nettop har skrevet trenger vi en Github action. Vi starter med å skrive yaml kode for å validere bicep malen vi nettopp har skrevet for å validere at den er korrekt. La oss opprette yaml filer for validering.

- Opprett **\\.github\workflows** folder i prosjektet ditt. **\Workshop_2\Start\AzureWorkshopInfrastruktur**
- Legg til en ny yaml fil for github action. Kall den **infrastructure.yaml**
  
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

### Deployment

Vi ønsker å gjenbruke så mye av yaml skriptet som vi kan så vi oppretter en en yaml fil som bare kaller en modul med parameter for test og produksjon.

- Slett innholdet i **infrastructure.yml** og erstatt det med koden under:
  
```yaml
# This workflow handles the deployment of Azure infrastructure using Bicep templates

# Workflow name - appears in GitHub Actions UI
name: Infrastructure Deployment

# Trigger conditions for the workflow
on:
  push:
    # Only run on main branch updates
    branches: [ main ]
    # Only trigger when these paths are modified
    paths:
      - 'bicep/**'        # Any changes to Bicep files
      - '.github/workflows/infrastructure.yml'  # Changes to this workflow
  # Allows manual trigger from GitHub Actions UI
  workflow_dispatch:

# Required permissions for the workflow
permissions:
  # Read access to repo contents
  contents: read
  # Write access to OpenID Connect token for Azure authentication
  id-token: write
  # Read access to GitHub Actions
  actions: read

# Jobs to be executed
jobs:
  # First job: Deploy to test environment
  deploy-test-infra:
    name: Deploy Test Infrastructure
    # Reuses another workflow file for actual deployment
    uses: ./.github/workflows/deploy-azure.yml
    with:
      # Passes 'test' as environment type
      releaseType: 'test'
    # Inherits secrets from the calling workflow
    secrets: inherit

  # Second job: Deploy to production environment
  deploy-prod-infra:
    name: Deploy Production Infrastructure
    # Ensures test deployment succeeds before running prod
    needs: deploy-test-infra
    # Reuses same deployment workflow as test
    uses: ./.github/workflows/deploy-azure.yml
    with:
      # Passes 'production' as environment type
      releaseType: 'production'
    # Inherits secrets from the calling workflow
    secrets: inherit
```

#### Depløoy modul

Opprett en ny yaml fil på samme nivå som **infrastructure.yml**. (For å forenkle debugging legger vi på triggere på denne yaml fila. Hvis du har triggere i en yaml fil må denne ligge på rot-nivå for at Github actions skal 'se' den.)

- Opprett filen **deploy-azure.yml**
- Denne fila innholder mya av det vi tidligere skrev for å validere bicep malen vi skrev, nå gjør vi validering i begge miljøer.

```yaml
# This workflow handles the deployment of infrastructure to Azure using Bicep

# Workflow name that appears in GitHub Actions UI
name: Deploy to Azure

# Environment variables available to all jobs and steps
env:
  # Dynamic environment name based on input parameter
  ENVIRONMENT_NAME: ${{inputs.releaseType}}
  # Azure region where resources will be deployed
  AZURE_LOCATION: 'norwayeast'
  # Resource group name with dynamic suffix based on release type
  RESOURCE_GROUP: 'rg-azskolen-${{inputs.releaseType}}'

# Defines when the workflow will run
on:
  # Manual trigger from GitHub Actions UI
  workflow_dispatch: 
  # Allows this workflow to be called by other workflows
  workflow_call:
    inputs:
      releaseType:
        description: 'Where to release (test or prod)?'
        type: string
        required: true
        default: 'test'

# Required permissions for the workflow
permissions:
  contents: read      # Permission to read repository contents
  id-token: write    # Permission to request OIDC tokens
  actions: read      # Permission to read Actions data

# Jobs to be executed
jobs:
    validate-bicep:
      name: Validate Bicep
      # Specifies the runner environment
      runs-on: ubuntu-latest
      steps:
        # Checks out repository code to the runner
        - name: Checkout
          uses: actions/checkout@v4
  
        # Authenticates with Azure using OIDC
        - name: Azure Login
          uses: azure/login@v2
          with:
            client-id: ${{ secrets.AZURE_CLIENT_ID }}
            tenant-id: ${{ secrets.AZURE_TENANT_ID }}
            subscription-id: ${{ secrets.AZURE_SUBSCRIPTION_ID }}

        # Validates Bicep syntax and builds ARM template
        - name: Bicep lint
          uses: azure/cli@v1
          with:
            inlineScript: |
              set -e  # Exit on any error
              # Compiles Bicep to ARM template
              echo "Running Bicep build ..."
              az bicep build --file bicep/main.bicep
              # Checks Bicep code for style and syntax issues
              echo "Running Bicep lint..."
              az bicep lint --file bicep/main.bicep

        # Validates the Bicep template against the resource group
        - name: Bicep Validate
          if: success()
          uses: Azure/cli@v1
          with:
            inlineScript: |
              set -e
              echo "Validating Bicep template: rs-grp ${{env.RESOURCE_GROUP}}"
              az deployment group validate \
                --resource-group ${{ env.RESOURCE_GROUP }} \
                --template-file ./bicep/main.bicep \
                --parameters environment=${{ env.ENVIRONMENT_NAME }} location=${{ env.AZURE_LOCATION }}
        
        # Performs a What-If deployment to preview changes
        - name: What-If Deployment
          uses: azure/arm-deploy@v1
          with:
            scope: resourcegroup
            resourceGroupName: ${{ env.RESOURCE_GROUP }}
            template: ./bicep/main.bicep
            parameters: environment=${{ env.ENVIRONMENT_NAME }} location=${{ env.AZURE_LOCATION }}
            deploymentMode: Validate
            additionalArguments: --what-if
                
        # Deploys the Bicep template to Azure
        - name: Deploy Bicep
          if: github.event_name == 'push' && github.ref == 'refs/heads/main'
          id: deploy
          uses: Azure/arm-deploy@v1
          with:
            scope: resourcegroup
            resourceGroupName: ${{ env.RESOURCE_GROUP }}
            template: ./bicep/main.bicep
            parameters: 
              environment=${{ env.ENVIRONMENT_NAME }}
              location=${{ env.AZURE_LOCATION }}
            failOnStdErr: false
```

## Managed identity

Det er flere måter å autoriser tilgang fra en web app. Det er mulig å gi web applikasjonen nøklene til storage account, noe som ikke er anbefalt da man ikke lengre har kontroll på hvem som har tilgang til storage. Man kan opprette et delt tilgangstoken (SAS) hvor man har større kontroll over hva det gis tilgang til og for hvor lenge. Ulempen med denne er at det er en manuell prosess som uløper og krever manuell inngripen for å fornye. Den anbefalte måten å gi tilgang er å bruke **managed identity**. Her gir Azure 'automatisk' tilgang når de tilgangene er satt i Entra Id. Du som utvikler trenger ikke å tenke på å forny token når alt er satt opp. La osss sette opp managed identity i Github actions.

Først legger vi til at storage benytter managed identity:

```json
  kind: 'StorageV2'
  identity: {
    type: 'SystemAssigned'
  }
```

Så må vi opprette en ny assignment modul i storage.bicep

```json
resource roleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(storageAccount.id, 'Storage Blob Data Contributor')
  scope: storageAccount
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'ba92f5b4-2d11-453d-a403-e96b0029c9fe') // Storage Blob Data Contributor
    principalId: storageAccount.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

```

## Del to. Organsiere Github pipeline i to flyter

Så langt har vi skrevet all yaml og bicept mer eller mindre rett fram uten å tenke så mye på organisering og struktur. Det har vært bra for å få en følelse av hvordan skrive yamle og bicep maler. Nå skal vi strukturere IaC prosjektet vårt for å gjøre det mer oversiktelig, øke gjenbruk og sikkerhet ved å skille mellom applikasjonskode og infrastruktur.

Vi skal nå opprette infrastructur for å kjøre web app vi publiserte i leksjon 1. I den leksjonen opprettet vi infrastrukturen manuelt i portalen. Nå skal vi gjøre det med bicept template og yaml skript i en Github Action pipeline.

### Rydde opp i Azure

- Gjenbruk ressursgruppe fra leksjon 1. (For å gjøre det litt enklere for oss selv i denne leksjonen oppretter vi ikke ressursgruppen med bicep.)
- Slett web app og Appservice du opprettet i leksjon 1.

### Omorganisere IaC prosjektet vårt

Vi skal nå opprette to workflows, en for å bygge og distribuere applikasjonskode og en for å opprette infrastruktur i Azure. Ved å dele i to arbidsflyter kan vi både begrense hvor ofte infrastruktur malene blir publisert ved å legge på filter og konfigurere ulik sikkerhet for distribusjon av kode og opprettelse av infrastruktur.

- Opprett to nye foldere hvor under **.github\worksflows**.
- Lag en for infrastruktur yaml filer. Kall den for **infrastructure**.
- Opprett en for applikasjons skript. Kall denne for **application**.
- I applikasajonsfolderen skap en ny fil. Kall den for **application.yml**.
- I infrastrukturfolderen opprett en ny fil og kall den for **infrastructur.yml**.
- I applikasjonfila kopierer du inn all yaml vi tidligere har skrevet som omhandler pålogging i Azure, bygg, test og distribusjon av applikasjonkode.
- Gjør tilsvarende for infrastuktur. I tellegg må vi opprette ny bicep maler for å kunne opprette infrastrastruktur for å kunne kjøre applikasjonen fra leksjon 1.
 
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

### Klargjør parameter-filer for test-miljøet og prod

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
