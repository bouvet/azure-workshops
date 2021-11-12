# Workshop: Serverless 

## Verktøy
#### Disse må du ha
* [Powershell](https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell?view=powershell-7.1) 
* [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
* [dotnet](https://dotnet.microsoft.com/download) (dotnet core 3.1 eller .NET 5)
* Text editor (f.eks. VS Code eller Visual Studio)
* Tilgang til Azureskolen subscription (sjekk [her](https://portal.azure.com))
* [Git](https://git-scm.com/downloads)

#### Kjekt å ha
* [Azure Storage Explorer](https://azure.microsoft.com/en-us/features/storage-explorer/)

## Kom i gang
Klon prosjektet med `git clone`. Vi skal et script som ligger i `azure-workshops/Workshop_4/Start` og det setter opp infrastruktur og deployer koden som ligger i Start folderen. Gjør følgende:
* Åpne Powershell (naviger til der azure-workshops ligger)
* `cd azure-workshops/Workshop_4/Start`
* `./start_workshop.ps1`
* Skriv inn navnet ditt og trykk enter
* Logg inn på bouvet brukeren din når nettleseren åpner
* Gå til portalen når scriptet er ferdig
  * Sjekk at din ressursgruppe eksisterer
  * Ved å skrive inn `$Env:Rg` vil ressursgruppen bli vist

## Test at koden er oppe og går
#### Test App Servicen 
1. Åpne [portalen](https://portal.azure.com)
1. Gå til App Servicen din
1. Klikk på urlen i oversikten
1. Last opp et bilde til applikasjonen
1. Vent litt og se at et bilde dukker opp

#### Test Function Appen
`ExampleFunctionHttpTrigger` er satt opp for å kunne teste og validere at Function Appen din er oppe og går. Dette er et http endepunkt som krever en query parameter. For å teste denne:
1. Åpne [portalen](https://portal.azure.com)
1. Gå til Function Appen din
1. Klikk på Functions på venstre side
1. Velg ExampleFunctionHttpTrigger
1. Klikk på Code + Test på venstre side
1. Klikk på Test/Run over json filen
1. Legg til en Query parameter med navn "blobName" og verdi lik navnet på et bildet du har lastet opp
   * Hvis bloben ikke eksisterer så får du en feilmelding i loggen. Last opp et bilde gjennom Web Appen og prøv så på nytt
1. Gå til Storage Account
1. Åpne container imagecontainer-mirror
1. Se at bildet har blitt lagt til der

### Oppgaven

`AzureWorkshopApp` er en bildeapp som lar deg laste opp bilder og se bildene du har lastet opp. Oppgaven din er å lage nye Functions som manipulerer bildene ved hjelp av ulike Function triggere. Eksempel når brukeren trykker på en knapp, eller etter et nytt bilde er lastet opp. Her er målet å bli kjent med ulike Function triggere, og hvordan man faktisk implementer de.  

**Ønsket sluttresultat er:**
* En `http trigger` Function (denne er laget for deg)
* En `blob trigger` Function som kutter bildet til kvadrater (ImageService.Square)
* En `output queue binding` i blob trigger Functionen 
* En `queue trigger` Function som gjør bildet grått (ImageService.GreyScale) trigget via blob trigger output binding

**Tilleggsfunksjonalitet man kan prøve seg på:**
* En `timer trigger` Function
* Ha to `queue trigger` Functions hvor en har output binding til neste queue trigger function
* En `Service Bus trigger` Function
* Andre triggers du vil eksperimentere med

Det anbefales å deploye ofte til Azure for å teste at alt fungerer som det skal.

![Architecture](./workshop4.png)

#### Deploy av FunctionApp

1. Åpne Powershell
1. Naviger til `Start/AzureWorkshop/AzureWorkshopFunctionApp` mappen
1. Kjør scriptet `./DeployFunction.ps1`

#### Deploy av AzureWorkshopApp

1. Åpne Powershell
1. Naviger til `Start/AzureWorkshop/AzureWorkshopApp` mappen
1. Kjør scriptet `./DeployApp.ps1`


### Litt om Function App koden
* `Constants.cs` har konstanter som representerer containere i Storage Accounten. 
* Det er satt opp Dependency Injection for `IBlobStorageService` og `IImageService`, så nye functions burde ha en constructor som tar imot disse på samme måte som `ExampleFunctionHttpTrigger.cs`.
* `BlobStorageService` inneholder logikk for å hente blobs som Stream og lagre en Stream i en blob. Opplasting av blobs er satt til å overskrive eksisterende blobs by default.
* `ImageService` har en del forskjellige metoder for å manipulere bilder. Denne kan utvides enkelt om man ønsker å gjøre mer spennende ting, anbefaler da at man fortsetter å bruke bitmap.

### Hvor starter jeg?

Åpne prosjektet `Start/AzureWorkshop/AzureWorkshopFunctionApp`. I Functions mappen finner du to Functions, et eksempel `ExampleFunctionHttpTrigger` og en du kan bruke som template `TemplateQueueTrigger`. Du skal lage nye Functions (se [Oppgaven](./README.md#Oppgaven) ovenfor) som manipulerer bildene som blir lastet opp. Dette kan gjøres manuelt, via VS Code eller Visual Studio. Se under:

**Legge til Functions manuelt**
* Lag en ny cs fil
* Kopier over fra `TemplateQueueTrigger.cs`
* Endre så navnet på klassen
* Endre FunctionName til å matche filnavn/klassenavn
* Legg inn kode i Run som manipulerer bildet i forhold til funksjonen du skal lage. 

**Legge til functions med Visual Studio (krever Azure development)**

* Høyreklikk på Functions mappen
* Trykk `Add > New Azure Function`
* Legg inn dependency injection selv (se `TemplateQueueTrigger.cs`)

**Legge til functions med VS code**

* Last ned `Azure Functions` extension
* Åpne mappen `AzureWorkshopFunctionApp` i ett eget VS Code vindu
* Gå til Azure ikonet som dukker opp på ikonmenyen på venstre side
* Med `Azure Functions` extension får du en tab som heter Functions
* I tabben skal mappen `Local Project` dukke opp (krever at kun mappen `AzureWorkshopFunctionApp` er åpen)
* Trykk på denne og initialiser den om det trengs, slik at `ExampleFunctionHttpTrigger` dukker opp under `Functions` mappen i tabben
* Legg til nye functions ved å trykke på lynikonet som er til høyre i tabheaderen
* Velg trigger type og navn
* Legg inn dependency injection selv (se `TemplateQueueTrigger.cs`)

#### Lokal testing
Opprett en `local.settings.json` fil og legg inn settings i den for å teste functions lokalt. For å kjøre det lokalt må AzureWebJobsStorage være satt (helst til en reell Storage Account ConnectionString)

Eksempel `local.settings.json` med lokal Development Storage Account (en virtuell Storage Account). Bytt ut `UseDevelopmentStorage=true;` med en reell Storage Account ConnectionString for å bruke Azure Storage Account.
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true;",
    "AzureWebJobsDashboard": ""
  }
}
```


### Står du fast?
I `komplett` mappen så vil man kunne se et eksempel på hvordan løsningen kan se ut. Dette er ikke en fasit, da man står helt fritt til hvordan man vil løse denne oppgaven.

> Google er din beste venn

