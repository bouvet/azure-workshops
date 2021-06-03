# Leksjon 1

​
I denne workshoppen skal vi gjøre om bilde-applikasjonen vår til bilde-applikasjon som krever innlogging. Vi skal også se
på flere tiltak vi kan gjøre for å gjøre applikasjonen mere sikker.
​
I denne leksjonen skal vi:
​

- Forberede og deploye applikasjonen til Azure.
- Fjerne tilganger i storage account, slik at bilde-Containeren vår ikke er åpen for public adgang.
- Gjøre oss litt kjent med Azure Security Center.
  ​

## Første deploy av applikasjonen

​
Start med å klone ut prosjektet med GIT.
​
`git clone https://github.com/bouvet/azure-workshops.git`
​
Gå så inn i `azure-workshops/Workshop_3/Start` katalogen, og her ligger prosjektet du skal jobbe videre med.
​

### Deploy av infrastruktur

​
Først må vi konfigurere navn på tjenestene som brukes av applikasjonen slik at dette blir unikt for din applikasjon. Åpne filen `azuredeploy.parameters.json` i mappen `Start/AzureWorkshopInfrastruktur/AzureWorkshopInfrastruktur`. I denne fila må du gi nye navn til følgende parametere:
​

* webSiteName
* storageAccountName (kun små bokstaver, ikke mellomrom eller spesialtegn)
* appInsightsName
  ​
  
   Disse variablene må ha navn som er globalt unike, så hvis du velger et navn som er i bruk vil deployment feile første gang. Ingen fare, da er det bare å endre navnene i denne fila.
   

#### _For å deploye infrastrukturen med Visual Studio_

1. Høyreklikk på prosjektet, og velg "Deploy".
2. Velg "New..."
3. Velg "Add an account.." og logg inn med brukeren din (trial).
4. Velg "Create New..." under Resource Group og gi ressursgruppen din et passende navn, samt velg f.eks. "West Europe" under Resource Group Location.
5. Trykk på Deploy.
   ​
   Følg så med om deploy av applikasjonen går greit. Det kan være at navnet du har valgt er opptatt, da er det bare å prøve på nytt.
   ​

#### _For å deploye infrastrukturen med Azure CLI_
1. Åpne Powershell/Terminal
1. Naviger til `Start/AzureWorkshopInfrastruktur/AzureWorkshopInfrastruktur`
1. Logg inn ved å kjøre `az login`
1. Bytt subscription hvis du trenger det med kommandoen `az account set --subscription "{subscription name or id}"`
1. Kjør kommandoen 
   ```powershell
   az deployment group create `
   --name {ExampleDeployment} `
   --resource-group {dinRessursGruppe} `
   --template-file .\azuredeploy.json `
   --parameters '@azuredeploy.parameters.json' 
   ```
1. Hvis noe feiler så prøv å endre på navnene du bruker i `azuredeploy.parameters.json` 

> Hvis du Azure CLI sier at det ikke finnes en az deployment group kommando så må du oppdatere Azure CLI ([Link](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli))

### Deploy av applikasjon


#### _For brukere med Visual Studio​_
Nå skal du deploye selve applikasjonen fra Visual Studio. Åpne `AzureWorkshop/AzureWorkshop.sln` i Visual Studio (du kan gjerne ha infrastruktur-prosjektet opp).
​

1. Høyreklikk på prosjektet `AzureWorkshopApp` og trykk "Publish...".
2. Velg "Select Existing" under "App Service"-fanen.
3. Trykk så på "Create Profile".
4. Velg så riktig konto i høyre hjørne. Det kan være at du må velge "Add an account." og logge inn.
5. Velg så riktig subscription og ressursgruppe/app service som du opprettet i forrige oppgave.
6. Trykk på OK - dette vil starte deploy til Azure.
   ​
   Nå skal du ha en fungerende applikasjon i Azure. Du kan jo prøve å laste opp et bilde for se at det fungerer.
   ​

#### _For brukere med Visual Studio Code_
Nå skal du deploye selve applikasjonen fra Visual Studio Code. 

1. Åpne mappen `Start/AzureWorkshop` i VS Code
2. Installer Azure App Service extension i VS Code
1. Klikk på View > Terminal (øverst i VS Code)
1. Naviger til `Start/AzureWorkshop/AzureWorkshopApp` i terminalen
1. Kjør kommandoen `dotnet publish --configuration Release`
1. På venstre side så skal du nå ha et Azure ikon, trykk på ikonet
1. Logg inn `Sign in to Azure...`
1. Høyreklikk på App Servicen du lagde når du deployet infrastrukturen (navnet står i `azuredeploy.parameters.json` filen)
1. Velg Deploy to Web App
1. Velg Browse
1. Naviger til `Start/AzureWorkshop/AzureWorkshopApp/bin/Release/`
1. Velg netcoreapp3.1 mappen

   Nå skal du ha en fungerende applikasjon i Azure. Du kan jo prøve å laste opp et bilde for se at det fungerer.

## Sikring av bilder i Storage Account

​
Frem til nå har applikasjonen brukt en container i storage account med public-access. Dette gjør at hele bildekatalogen er åpen for hele verden. Med de endringene vi gjør i applikasjonen, ønsker vi ikke dette lenger.
​

1. Editer `azuredeploy.json` i infrastruktur-prosjektet, og under konfigurasjon av Storage Account, bytt ut linjen
   `"publicAccess": "Container"` med `"publicAccess": "None"`.
2. Deploy prosjektet på nytt, ved å gå gjennom de samme stegene du gjorde for infrastruktur deployment. 

   Hvis du tester applikasjon din i Azure nå, vil du se at bildene ikke vil vises.

   For å kunne gi de brukerne som skal se bilder tilgang til bilder, skal vi bruke Shared Access Signature Tokens (SAS-token.) SAS-token er et token som gir en tidsbegrenset tilgang (lese, slette osv) til en ressurs (blob, container etc.) i storage account. Vi ønsker å gi kun lese-tilgang til bildene, samt at tilgangen skal være tidsbegrenset.
   ​
3. Editer filen `Services/StorageService.cs` filen i AzureWorkshopApp-filen. Vi har laget flere TODO-kommentarer som beskriver endringene som skal gjøres.
4. Publiser prosjektet på nytt (høyreklikk på prosjektet og trykk "Publish")
   ​
   Nå skal du igjen se bildene, og hvis du høyreklikker på bildet og ser URL'en, så ser du at SAS-tokenet er lagt til på slutten. Du tenker
   kanskje at alle har fortsatt tilgang, men det skal vi gjøre noe med i leksjon 2.
   ​

## Azure Security Center (1)

​
Azure Security Center er en tjeneste i Azure som overvåker tjenestene dine, og leter etter mulige konfigurasjoner som kan gjøre tjenestene
dine usikre.

I Basic tier så får du gratis anbefalinger på tiltak du kan gjøre for å forbedre sikkerheten i tjenestene dine.


I tillegg har den et Standard tier, som tilbyr utvidet overvåkning av tjenestene. Dette må settes opp per tjeneste og koster ekstra.
​
Azure Security Center bruker litt tid for å scanne tjenestene dine etter at de er opprettet, derfor skal du i denne øvelsen bare gjøre deg litt kjent med Azure Security Center:
​

- Logg inn i Azure-portalen (https://portal.azure.com).
- Finn Azure Security Center fra menyen på venstre.
- Klikk litt rundt i tjenesten og gjør deg litt kjent med hva som finnes her. Er tjenestene dine dukket opp, og er det kommet noen anbefalinger allerede?

> Det er ikke sikkert du får anbefalinger i Azure Security Center med en gang. Det tar gjerne litt tid og derfor skal vi komme tilbake til Azure Security Center i leksjon 3, hvor vi skal se om vi får utbedret noen sårbarheter.
  
  ​

## Oppsummering

​
I denne øvelsen har vi satt opp applikasjonen, fjernet åpen tilgang i Storage Account og undersøkt sårbarheter/anbefalinger i Azure Security Center.