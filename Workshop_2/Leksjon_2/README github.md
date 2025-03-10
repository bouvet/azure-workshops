
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
- Legg til en ny yml fil. Kall den noe med **iac-** så du lettere kjenner den igjen.
  
```yaml
name: IaC deploy

permissions:
  id-token: write
  contents: read

on:
  push:
    branches:
      - main
  pull_request:
    branches:
      - main
  workflow_dispatch:

jobs:
    deploy-infrastructure:
      runs-on: ubuntu-latest
      steps:
        - name: Checkout
          uses: actions/checkout@v2
  
        - name: Azure login
          uses: azure/login@v1
          with:
            creds: ${{ secrets.AZURE_CREDENTIALS }}
  
        - name: Deploy with Azure CLI
          run: |
            az group create --name ${{ env.RESOURCE_GROUP }} --location ${{ env.LOCATION }}
            az deployment group create \
              --resource-group ${{ env.RESOURCE_GROUP }} \
              --template-file main.bicep \
              --parameters main.parameters.json
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
