# Workshop: Serverless 
------------------------------
## Function App

I Start folderen finner du en påbegynt Function App (under AzureWorkshopApp). Denne Function Appen er ufullstendig og det blir din oppgave å utvide den. 
Før vi går i gang så må vi sette opp litt ressurser i Azure. Forklaringene kommer kun til å vise Azure CLI måten man deployer på. Om du heller vil bruke portalen og visual studio så er dette også greit.
Last ned [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) og installer. 
Les hele README filen før du går i gang med å kode.

### Deploy infrastruktur til Azure 
Åpne en terminal/powershell og sjekk at Azure CLI fungerer ved å logge inn med kommandoen az login og følg stegene deretter (åpner som oftest en nettleser med microsoft pålogging).
Hvis alt har gått bra skal du ha fått en liste over subscriptions du har tilgang til.
Videre steg for å deploye infrastruktur til Azure:
1. Velg Azureskolen subscription `az account set --subscription "Azureskolen"`
   - Hvis du ikke har Azureskolen subscription tilgjengelig, be om tilgang eller bruk et annet subscription du har tilgjengelig
1. Lag så en Resource Group ved å kjøre kommandoen `az group create --location westeurope --name {YourResourceGroup}`
   - Bytt ut YourResourceGroup med et navn på din ressursgruppe
1. Naviger til Start/AzureWorkshopInfrastruktur/AzureWorkshopInfrastruktur mappen
   - Her skal azuredeploy.json og azuredeploy.parameters.json ligge
1. Åpne azuredeploy.parameters.json i en tekst editor
1. Legg inn verdier for parameterne som ikke er satt (husk å lagre)
1. Kjør så kommandoen `az deployment group create --name {ExampleDeployment} --resource-group {YourResourceGroup} --template-file .\azuredeploy.json --parameters '@azuredeploy.parameters.json'  `
   - Bytt ut ExampleDeployment og YourResourceGroup med et navn på deploymenten (f.eks. infrastrukturDeployment) og resource groupen du lagde ovenfor 
1. Hvis alt gikk gjennom så skal du få en json output med ressursene som ble laget
1. Gå gjerne til portalen og sjekk at ressursene ligger i ressurs gruppen din

### Kode oppgave
Åpne AzureWorkshopApp i VS Code eller Visual Studio. De fleste endringene som må gjøres vil være i AzureWorkshopFunctionApp, men et par småting må fikses i AzureWorkshopApp.
Åpne ExampleFunctionHttpTrigger.cs, denne filen gir deg et eksempel på hvordan en Function ser ut og hvordan en trigger brukes. 
Det som blir din oppgave blir å lage nye functions som gjør endringer på bilder som sendes inn. 

Endringene som må gjøres i AzureWorkshopApp er å kommentere inn muligheten til å vise bilder fra andre containere i Index.cshtml (Views/Home/Index.cshtml). I ImagesController kan man kommentere inn en kodelinje som legger en melding med filnavn på en kø. Hvis man vil sende JSON objekter så er det også mulig. Andre ting man kan gjøre er å kalle en HttpTrigger Function via en HttpClient, her er det egentlig bare å passe på at man har nok kall mot functionene man lager.

Ønsket sluttresultat er:
* En http trigger function
* En blob trigger function
* En queue trigger function
* En output queue binding (dette legges i en av de andre functionene)

Tilleggsfunksjonalitet man kan prøve seg på:
* En timer trigger function
* Ha queue trigger med queue binding output som trigger en ny function
* Service Bus trigger (dette må settes opp selv)
* Eller en annen trigger

### Litt om Function App koden
Constants.cs har konstanter som representerer containere i Storage Accounten. Her er det bare å legge til nye hvis man har lyst til å ha andre containere
Det er satt opp Dependency Injection for IBlobService og IImageService, så nye functions burde ha en constructor som tar imot disse på samme måte som eksempel funksjonen.
BlobService har implementasjon for å hente blobs som Stream og lagre en Stream til en blob. Opplasting av filer er satt til å skrive over eksisterende som default. 
ImageService har en del forskjellige metoder for å gjøre ting med bilder. Denne kan utvides enkelt om man ønsker å gjøre mer spennende ting, anbefaler da at man fortsetter å bruke bitmap.

Anbefaler å bruke Azure Functions extension i VS Code, i Visual Studio så vil du kunne høyreklikke på Functions mappen og klikke på Add > New Azure Function...
For å kunne legge til functions så må man åpne folderen AzureWorkshopFunctionApp i VS Code. Når det er gjort legger du inn nye functions ved å gå til Azure Extensionen på venstre side. Hvis du har Azure Functions extension installert så skal du ha en tab som heter Functions. Det vil da under tabben finnes en mappe som heter Local Project, trykk på denne og evt initialiser hvis den trenger det. Hvis det listes Functions og ExampleFunctionHttpTrigger vises så kan du legge til nye functions ved å trykke på et lyn med et pluss på (finnes til høyre for Functions). Velg så trigger type, navn og gjør endringene får å få Dependency Injection til å fungere på samme måte som i eksempel funksjonen. 

Man kan legge til en local.settings.json fil og legge settings inn i den for å teste functions lokalt. Hvis du vil kjøre det lokalt så må AzureWebJobsStorage være satt (helst til en reell Storage Account ConnectionString)
Eksempel med lokal Development Storage Account (en virituall Storage Account)
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true;",
    "AzureWebJobsDashboard": ""
  }
}
```

### Deploy kode til Azure
Hvis du vil deploye gjennom Visual Studio eller VS Code så kan det enkelt gjøres. Denne guiden vil bare vise hvordan du deployer ved å bruke Azure CLI
1. Åpne en terminal/powershell
1. Naviger til AzureWorkshop mappen (under Start)
1. Gå til AzureWorkshopApp eller AzureWorkshopFunctionApp (ettersom hva du vil deploye)
1. Kjør kommandoen `dotnet publish -c Release`
   - Dette lager en release av koden
1. Naviger til ./bin/Release/netcoreapp3.1/publish/
1. Zip filene i denne mappen 
   - Powershell `Compress-Archive -Path .\* -DestinationPath .\functionapp.zip `
   - Terminal (Linux/Mac) `zip -r functionapp.zip ./*`
1. Deploy ved å kjøre `az functionapp deployment source config-zip -g {YourResourceGroup} -n {YourAppServiceName} --src functionapp.zip` 
   - Denne kommandoen fungerer for både Function App og App Service
1. Hvis du får tilbake `Deployment endpoint responded with status code 202` så er applikasjonen lastet opp og klar til å testes


### Står du fast?
I komplett mappen så vil man kunne se et eksempel på hvordan løsningen kan se ut. Dette er ikke en fasit, da man står helt fritt til hvordan man vil løse denne oppgaven.

> Google er din beste venn

