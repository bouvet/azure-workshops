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

- webSiteName
- storageAccountName (kun små bokstaver, ikke mellomrom eller spesialtegn)
- appInsightsName
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
1. Opprett en ressursgruppe ved å kjøre kommandoen ```az group create --name {dinRessursgruppe} --location {dinLocation}``` Gi ressursgruppen et selvvalgt navn. For location kan du f.eks. bruke `westeurope`  eller `norwayeast`.
1. Kjør kommandoen 
   ```powershell
   az deployment group create `
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

1. Høyreklikk på package.json i AzureWorkshopApp, og velg "Restore Packages"
2. Høyreklikk på prosjektet `AzureWorkshopApp` og trykk "Publish...".
3. Velg "Select Existing" under "App Service"-fanen.
4. Trykk så på "Create Profile".
5. Velg så riktig konto i høyre hjørne. Det kan være at du må velge "Add an account." og logge inn.
6. Velg så riktig subscription og ressursgruppe/app service som du opprettet i forrige oppgave.
7. Trykk på OK - dette vil starte deploy til Azure.
   ​
   Nå skal du ha en fungerende applikasjon i Azure. Hvis du forsøker å gå til applikasjonen vil du få en feilmelding om manglende connection string. Det skal vi fikse i neste steg.
   ​

#### _For brukere med Visual Studio Code_

Nå skal du deploye selve applikasjonen fra Visual Studio Code.

1. Åpne mappen `Start/AzureWorkshop` i VS Code
2. Installer Azure App Service extension i VS Code hvis du ikke har denne extensionen fra før
3. Klikk på View > Terminal (øverst i VS Code)
4. Naviger til `Start/AzureWorkshop/AzureWorkshopApp` i terminalen
5. Kjør `npm install`
6. Kjør kommandoen `dotnet publish --configuration Release`
7. På venstre side så skal du ha et Azure ikon, trykk på ikonet
8. Logg inn `Sign in to Azure...`
9. Høyreklikk på App Servicen du lagde når du deployet infrastrukturen (navnet står i `azuredeploy.parameters.json` filen)
10. Velg Deploy to Web App
11. Velg Browse
12. Naviger til `Start/AzureWorkshop/AzureWorkshopApp/bin/Release/`
13. Velg net6.0/publish-mappen

Nå skal du ha en fungerende applikasjon i Azure. Hvis du forsøker å gå til applikasjonen vil du få en feilmelding om manglende connection string. Det skal vi fikse i neste steg.

## Sikring av hemmeligheter
For å sikre hemmeligheter som passord og connection strings kan man bruke Azure KeyVault. For å sørge for at disse hemmelighetene ikke er lett tilgjengelig legges disse inn i et keyvault, som man begrenser tilgangen til.
(Denne oppgaven var bonusleksjon i WS1, hvis noen synes den ser kjent ut. Har du gjort den før kan du velge om du vil forsøke å gjøre endringene i ARM-templates i stedet for å bruke Azure CLI).

### _Azure CLI_
**Opprett keyvault**
```
az keyvault create  --name {dittKeyVault} --enable-rbac-authorization true --resource-group {dinRessursGruppe}
```
**Gi deg selv rettigheter til å administrere secrets**
For dette steget trenger du å finne to verdier. Det første er din userid, den finner du enklest ved å kjøre `az ad signed-in-user show`. 
Deretter trenger du Resource identifier (id) til keyvaultet. Den finner du ved å kjøre `az keyvault show --name {dittKeyVault}`. Den ser typisk slik ut: `/subscriptions/{subscriptionid}/resourceGroups/{dinRessursgruppe}/providers/Microsoft.KeyVault/vaults/{dittKeyVault}`

```
az role assignment create --role "Key Vault Administrator" --assignee {userid} --scope {resourceId}
```

**Finn connection string til Storage Account**
```
az storage account show-connection-string --name {dinStorageAccount} 
```
**Legg til secret i keyvault** 
Legg inn connection string til Storage Account som secret i keyvault. NB! Secreten må hete `AzureStorageConfig--ConnectionString` for at den skal plukkes opp av konfigurasjonssystemet til webapplikasjonen.
```
 az keyvault secret set --name AzureStorageConfig--ConnectionString --vault-name {dittKeyVault} --value {connectionString}
```

**Gi applikasjonen tilgang til å lese secrets**
For dette steget trenger du ObjektID'en til webapplikasjonens _Managed Service Identity_. Den finner du enklest ved å kjøre `az webapp show --name {dinWebApp} --resource-group {dinRessursgruppe}`, og lese av verdien identity->principalId. Du kan også gå til webapplikasjonen i portalen og lese ut verdien under "Identity"-fanen.  Verdien for resourceID  er den samme du fant i tidligere steg.
```
az role assignment create --role "Key Vault Secrets User" --assignee {principalId} --scope {resourceId}
```

**Oppdater applikasjonen**
Oppdater webapplikasjonen til å bruke KeyVault for config-settings.
Legg til nuget-pakkene `Azure.Identity` og `Azure.Extensions.AspNetCore.Configuration.Secrets`.

Endre Program.cs: 
```cs
using System;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
...
private const string KeyVaultEndpoint = "https://<navn-på-keyvault>.vault.azure.net/";
public static IWebHost CreateWebHostBuilder(string[] args) =>
      WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((ctx, builder) =>
            {
               builder.AddAzureKeyVault(new Uri(KeyVaultEndpoint), new DefaultAzureCredential());
            })
            .UseStartup<Startup>()
            .UseApplicationInsights().Build();
```

Deploy applikasjonen på nytt og sjekk at du får lastet opp og vist bilder.
   ​

## Microsoft Defender for Cloud (1)

Microsoft Defender for Cloud (tidl. Azure Security Center) er en tjeneste i Azure som overvåker tjenestene dine, og leter etter mulige konfigurasjoner som kan gjøre tjenestene
dine usikre.

I Basic tier så får du gratis anbefalinger på tiltak du kan gjøre for å forbedre sikkerheten i tjenestene dine.

I tillegg har den et Standard tier, som tilbyr utvidet overvåkning av tjenestene. Dette må settes opp per tjeneste og koster ekstra.
​
Microsoft Defender for Cloud bruker litt tid for å scanne tjenestene dine etter at de er opprettet, derfor skal du i denne øvelsen bare gjøre deg litt kjent med Microsoft Defender for Cloud:
​

- Logg inn i Azure-portalen (https://portal.azure.com).
- Finn Microsoft Defender for Cloud fra menyen på venstre.
- Klikk litt rundt i tjenesten og gjør deg litt kjent med hva som finnes her. Er tjenestene dine dukket opp, og er det kommet noen anbefalinger allerede?

> Det er ikke sikkert du får anbefalinger i Microsoft Defender for Cloud med en gang. Det tar gjerne litt tid og derfor skal vi komme tilbake til Microsoft Defender for Cloud i leksjon 3, hvor vi skal se om vi får utbedret noen sårbarheter.

​

## Oppsummering

​
I denne øvelsen har vi satt opp applikasjonen, fjernet åpen tilgang i Storage Account og undersøkt sårbarheter/anbefalinger i Microsoft Defender for Cloud.
