# Workshop: Serverless 

## Function App

I `Start` mappen finner du en påbegynt Function App under `Start/AzureWorkshop/AzureWorkshopApp`, med tilhørende infrastruktur definert i `start/AzureWorkshopInfrastruktur/AzureWorkshopInfrastruktur`. Denne Function Appen er ufullstendig og oppgaven din er å utvide den. Før vi går i gang så må vi sette opp litt ressurser i Azure. Forklaringene kommer kun til å vise Azure CLI måten man deployer på. Om du heller vil bruke portalen og Visual Studio så er dette også greit.

Det er lurt å Lese hele README før du går i gang med å kode. Det kommer en del viktige tips og workshoppen er ganske åpen. Etter du har lest kjapt i gjennom står du fritt til å lage din egen plan for å komme i mål!  


### Anbefalte verktøy

* [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
* Visual studio eller VS code
* [Azure Storage Explorer](https://azure.microsoft.com/en-us/features/storage-explorer/)

### Deploy infrastruktur til Azure 

1. Last ned [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) og installer. 
1. Åpne en terminal/powershell og sjekk at Azure CLI fungerer ved å logge inn med kommandoen `az login` og følg stegene deretter (åpner som oftest en nettleser med Microsoft pålogging).

Hvis alt har gått bra skal du ha fått en liste over subscriptions du har tilgang til. Videre steg for å deploye infrastruktur til Azure

1. Velg Azureskolen subscription `az account set --subscription "Azureskolen"`
   - Hvis du ikke har Azureskolen subscription tilgjengelig, be om tilgang eller bruk et annet subscription du har tilgjengelig
1. Lag så en Resource Group ved å kjøre kommandoen `az group create --location westeurope --name {YourResourceGroup}`
   - Bytt ut `YourResourceGroup` med et navn på din ressursgruppe
1. Naviger til `Start/AzureWorkshopInfrastruktur/AzureWorkshopInfrastruktur` mappen
   - Her finner du `azuredeploy.json `og `azuredeploy.parameters.json`
1. Åpne `azuredeploy.parameters.json` i en tekst editor
1. Legg inn verdier for parameterne som ikke er satt (husk å lagre og at Azure ofte krever unike ressursnavn)
1. Kjør så kommandoen `az deployment group create --name {ExampleDeployment} --resource-group {YourResourceGroup} --template-file .\azuredeploy.json --parameters '@azuredeploy.parameters.json'  `
   - Bytt ut `ExampleDeployment` og `YourResourceGroup` med et navn på deploymenten (f.eks. infrastrukturDeployment) og resource groupen du lagde ovenfor 
1. Hvis alt gikk gjennom så skal du få en json output med ressursene som ble laget. Dersom det ikke er tilfellet legg til `--debug` flagget i kommandoen. Om du da finleser output fra kommandoen, så finner du trolig ut at et av ressursnavnene ikke er unikt
1. Gå gjerne til portalen og sjekk at ressursene ligger i ressurs gruppen din



### Komme i gang

For å komme i gang anbefaler vi at du deployer både Web Appen og Function Appen. Disse skal da fungere, men ikke gjøre noe mer enn de gjorde etter Workshop 1 (dvs du kan laste opp bilder og se de).


#### Deploy av AzureWorkshopApp

1. Åpne en terminal/powershell
1. Naviger til `Start/AzureWorkshop/AzureWorkshopApp` mappen
1. Kjør kommandoen `dotnet publish -c Release`
   - Dette lager en release av koden
1. Zip filene i mappen `./bin/Release/netcoreapp3.1/publish/` 
   - Powershell `Compress-Archive -Path .\bin\Release\netcoreapp3.1\publish\* -DestinationPath .\code.zip` (legg til -Force for å overskrive)
   - Terminal (Linux/Mac) `zip -r code.zip ./bin/Release/netcoreapp3.1/publish/*`
1. Deploy ved å kjøre `az functionapp deployment source config-zip -g {YourResourceGroup} -n {YourAppServiceName} --src code.zip` 
1. Hvis du får tilbake `Deployment endpoint responded with status code 202` så er applikasjonen lastet opp og klar til å testes

#### Deploy av FunctionApp

1. Åpne en terminal/powershell
1. Naviger til `Start/AzureWorkshop/AzureWorkshopFunctionApp` mappen
1. Kjør kommandoen `dotnet publish -c Release`
   - Dette lager en release av koden
1. Zip filene i mappen `./bin/Release/netcoreapp3.1/publish/` 
   - Powershell `Compress-Archive -Path .\bin\Release\netcoreapp3.1\publish\* -DestinationPath .\code.zip` (legg til -Force for å overskrive)
   - Terminal (Linux/Mac) `zip -r code.zip ./bin/Release/netcoreapp3.1/publish/*`
1. Deploy ved å kjøre `az functionapp deployment source config-zip -g {YourResourceGroup} -n {YourAppServiceName} --src code.zip` 
1. Hvis du får tilbake `Deployment endpoint responded with status code 202` så er applikasjonen lastet opp og klar til å testes

`ExampleFunctionHttpTrigger` er satt opp for å kunne teste og validere at Function Appen din er oppe og går. Dette er et http endepunkt som krever en query parameter. Hvis du vil teste så gå til test siden i portalen:
1. Åpne [portalen](portal.azure.com)
1. Gå til Function Appen din
1. Klikk på Functions på venstre side
1. Velg ExampleFunctionHttpTrigger
1. Klikk på Code + Test på venstre side
1. Klikk på Test/Run over json filen
1. Legg til en Query parameter med navn "blobName" og verdi lik navnet på et bildet du har lastet opp
   - Hvis bloben ikke eksisterer så får du en feilmelding i loggen. Last opp et bilde gjennom Web Appen og prøv så på nytt

#### Endre på koden

1. Åpne `AzureWorkshopApp` i VS Code eller Visual Studio. De fleste endringene skal gjøres i `AzureWorkshopFunctionApp`, men et par småting må fikses i `AzureWorkshopApp`.
1. Åpne `ExampleFunctionHttpTrigger.cs`, denne filen gir deg et eksempel på hvordan en Function ser ut og hvordan en trigger brukes. 
Oppgaven din er å lage nye functions som gjør endringer på bilder som sendes inn. 

Endringene som må gjøres i `AzureWorkshopApp` er å 
1. Kommentere inn knappene i `Views/Home/Index.cshtml`. Bildene som vises i frontend hentes fra en Blob container i en Storage account. Knappene gjør at man kan bytte Blob container source. 
1. I `ImagesController.cs` kan man kommentere inn en kodelinje som legger en melding med filnavn på en kø. Hvis man vil sende JSON objekter så er det også mulig. 

### Litt om Function App koden
* `Constants.cs` har konstanter som representerer containere i Storage Accounten. Her er det bare å legge til nye hvis man har lyst til å ha andre containere
* Det er satt opp Dependency Injection for `IBlobService` og `IImageService`, så nye functions burde ha en constructor som tar imot disse på samme måte som `ExampleFunctionHttpTrigger.cs`
* `BlobService` inneholder logikk for å hente blobs som Stream og lagre en Stream i en blob. Opplasting av blobs er satt til å overskrive eksisterende blobs by default
* `ImageService` har en del forskjellige metoder for å manipulere bilder. Denne kan utvides enkelt om man ønsker å gjøre mer spennende ting, anbefaler da at man fortsetter å bruke bitmap

> <b>Functions kan lages på flere forskjellige måter. Det er hovedsakelig bare vanlige C# filer (.cs) som får autogenerert litt JSON filer når man bygger og deployer koden. Du kan velge å lage Functions manuelt eller via VS Code / Visual Studio. Bruk `TemplateQueueTrigger.cs` til å verifisere at det er satt opp riktig, det er lett at man får noen feil når man autogenerer kode og det autogenereres uten Dependency Injection.</b>

**Legge til functions manuelt** 

I koden finnes det en Function template som heter `TemplateQueueTrigger.cs`. Den kan man bruke til å lage andre functions som BlobTrigger, TimerTrigger og lignende. 

**Legge til functions med Visual Studio**

* Høyreklikk på Functions mappen
* Trykk `Add > New Azure Function`

**Legge til functions med VS code**

* Last ned `Azure Functions` extension
* Åpne mappen `AzureWorkshopFunctionApp` i ett eget VS Code vindu
* Gå til Azure ikonet som dukker opp på ikonmenyen på venstre side
* Med `Azure Functions` extension får du en tab som heter Functions
* I tabben skal mappen `Local Project` dukke opp (krever at kun mappen `AzureWorkshopFunctionApp` er åpen)
* Trykk på denne og initialiser den om det trengs, slik at `ExampleFunctionHttpTrigger` dukker opp under `Functions` mappen i tabben
* Legg til nye functions ved å trykke på lynikonet som er til høyre i tabheaderen
* Velg trigger type og navn
* Husk å legge til `IImageService` og `IBlobStorageService` i Function konstruktøren om du trenger det 

Man kan opprette en `local.settings.json` fil og legge settings inn i den for å teste functions lokalt. For å kjøre det lokalt må AzureWebJobsStorage være satt (helst til en reell Storage Account ConnectionString)

Eksempel `local.settings.json` med lokal Development Storage Account (en virtuell Storage Account)
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true;",
    "AzureWebJobsDashboard": ""
  }
}
```

### Oppgaven


`AzureWorkshopApp` er en bildeapp som lar deg se bilder og laste opp bilder. Oppgaven din er å lage nye Functions som manipulerer bildene ved hjelp av ulike Function triggers. Eksempel når brukeren trykker på en knapp eller etter et nytt bilde er lastet opp. Her er målet å bli kjent med ulike Function triggers, og hvordan man faktisk implementer de.  

Ønsket sluttresultat er:
* En `http trigger` Function (denne er laget for deg)
* En `blob trigger` Function som kutter bildet til kvadrater (ImageService.Square)
* En `output queue binding` i blob trigger Functionen 
* En `queue trigger` Function som gjør bildet grått (ImageService.GreyScale) trigget via blob trigger output binding

Tilleggsfunksjonalitet man kan prøve seg på:
* En `timer trigger` Function
* Ha to `queue trigger` Functions hvor en har output binding til neste queue trigger function
* En `Service Bus trigger` Function
* Andre triggers du vil eksperimentere med

Det anbefales å deploye ofte til Azure for å teste at alt fungerer som det skal.

![Architecture](./workshop4.png)

### Deploy kode til Azure
Når du har oppdatert koden din og ønsker å deploye på nytt så kan du godt bruke Visual Studio eller VS Code, men hvis du vil gjøre det med Azure CLI, slik du gjorde det ved første deploy, så gjør du følgende:  
1. Åpne en terminal/powershell
1. Naviger til `Start/AzureWorkshop` mappen
1. Gå til `AzureWorkshopApp` eller `AzureWorkshopFunctionApp` (ettersom hva du vil deploye)
1. Kjør kommandoen `dotnet publish -c Release`
   - Dette lager en release av koden
1. Zip filene i mappen `./bin/Release/netcoreapp3.1/publish/` 
   - Powershell `Compress-Archive -Path .\bin\Release\netcoreapp3.1\publish\* -DestinationPath .\code.zip` (legg til -Force for å overskrive)
   - Terminal (Linux/Mac) `zip -r code.zip ./bin/Release/netcoreapp3.1/publish/*`
1. Deploy ved å kjøre `az functionapp deployment source config-zip -g {YourResourceGroup} -n {YourAppServiceName} --src code.zip` 
   - Denne kommandoen fungerer for både Function App og App Service
1. Hvis du får tilbake `Deployment endpoint responded with status code 202` så er applikasjonen lastet opp og klar til å testes


### Står du fast?
I `komplett` mappen så vil man kunne se et eksempel på hvordan løsningen kan se ut. Dette er ikke en fasit, da man står helt fritt til hvordan man vil løse denne oppgaven.

> Google er din beste venn

