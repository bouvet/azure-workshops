# Leksjon 1

I denne workshoppen skal vi gjøre om bilde-applikasjonen vår til bilde-applikasjon som krever innlogging. Vi skal også se
på flere tiltak vi kan gjøre for å gjøre applikasjonen mere sikker.

I denne leksjonen skal vi:

- Forberede og deploye applikasjonen til Azure.
- Fjerne tilganger i storage account, slik at bilde-Containeren vår ikke er åpen for public adgang.
- Gjøre oss litt kjent med Azure Security Center og gjøre noen tiltak.

## Første deploy av applikasjonen

Start med å klone ut prosjektet med GIT.

`git clone https://github.com/bouvet/azure-workshops.git`

Gå så inn i `azure-workshops/Workshop_3/Start` katalogen, og her ligger prosjektet du skal jobbe videre med.

### Deploy av infrastruktur

Først må vi konfigurere navn på tjenestene som brukes av applikasjonen slik at dette blir unikt for din applikasjon.
Åpne `AzureWorkshopInfrastruktur/AzureWorkshopInfrastruktur.sln` i VisualStudio. Åpne filen `azuredeploy.parameters.json`. I denne fila må du gi nye navn til følgende parametere:

- webSiteName
- storageAccountName (kun små bokstaver, ikke mellomrom eller spesialtegn)
- appInsightsName

Disse variablene må ha navn som er globalt unike, så hvis du velger et navn som er i bruk vil deployment feile første gang. Ingen fare, da er det bare å endre navnene i denne fila.

For å deploye applikasjonen

1. Høyreklikk på prosjektet, og velg "Publish".
2. Velg "New..."
3. Velg "Add an account.." og logg inn med brukeren din (trial).
4. Velg "Create New..." under Resource Group og gi ressursgruppen din et passende navn, samt velg f.eks. "West Europe" under Resource Group Location.
5. Trykk på Deploy.

Følg så med om deploy av applikasjonen går greit. Det kan være at navnet du har valgt er opptatt, da er det bare å prøve på nytt.

### Deploy av applikasjon

Nå skal du deploye selve applikasjonen fra Visual Studio.

Åpne `AzureWorkshop/AzureWorkshop.sln` i Visual Studio (du kan gjerne ha infrastruktur-prosjektet opp).

1. Høyreklikk på prosjektet `AzureWorkshopApp` og trykk "Publish...".
2. Velg "Select Existing" under "App Service"-fanen.
3. Trykk så på "Create Profile".
4. Velg så riktig konto i høyre hjørne. Det kan være at du må velge "Add an account." og logge inn.
5. Velg så riktig subscription og ressursgruppe/app service som du opprettet i forrige oppgave.
6. Trykk på OK - dette vil starte deploy til Azure.

Nå skal du ha en fungerende applikasjon i Azure. Du kan jo prøve å laste opp et bilde for se at det fungerer.

## Sikring av bilder i Storage Account

Frem til nå har applikasjonen brukt en container i storage account med public-access. Dette gjør at hele bildekatalogen er åpen
for hele verden. Med de endringene vi gjør i applikasjonen, ønsker vi ikke dette lenger.

1. Editer azuredeploy.json i infrastruktur-prosjektet, og under konfigurasjon av Storage Account, bytt ut linjen
   `"publicAccess": "Container"` med `"publicAccess": "None"`.
2. Deploy prosjektet på nytt, ved å høyreklikke på prosjektet og velg "Deploy" og velg profilen du opprettet tidligere.

Hvis du tester applikasjon din i Azure nå, vil du se at bildene ikke vil vises.

For å kunne gi de brukerne som skal se bilder tilgang til bilder, skal vi bruke Shared Access Signature Tokens (SAS-token.) SAS-token er et token som gir en tidsbegrenset tilgang (lese, slette osv) til en ressurs (blob, container etc.) i storage account. Vi ønsker å gi kun lese-tilgang til bildene, samt at tilgangen skal være tidsbegrenset.

1. Editer filen `Services/StorageService.cs` filen i AzureWorkshopApp-filen. Vi har laget TODO-kommentarer som beskriver endringene
   som skal gjøres.
2. Publiser prosjektet på nytt (høyreklikk på prosjektet og trykk "Publish")

Nå skal du igjen se bildene, og hvis du høyreklikker på bildet og ser URL'en, så ser du at SAS-tokenet er lagt til på slutten. Du tenker
kanskje at alle har fortsatt tilgang, men det skal vi gjøre noe med i leksjon 2.

## Azure Security Center

Azure Security Center er en tjeneste i Azure som overvåker tjenestene dine, og leter etter mulige konfigurasjoner som kan gjøre tjenestene
dine usikre.

I Basic tier så får du gratis anbefalinger på tiltak du kan gjøre for å forbedre sikkerheten i tjenestene dine.

I tillegg har den et Standard tier, som tilbyr utvidet overvåkning av tjenestene. Dette må settes opp per tjeneste og koster ekstra.

### App Service

Nå skal du se om du får noen anbefalinger fra Security Center på App Servicen din.

- Logg inn i Azure-portalen (https://portal.azure.com)
- Gå til `webappnavn` som du opprettet i forrige oppgave.
- Trykk så på `Security`, og du kommer til et skjermbilde som viser alerts.
- Se om du har noen sårbarheter som bør utbedres. Her vil du mest sannsynlig se en melding om at det anbefales å skru på https slik at det kun går over sikker kanal.

Når du nå ser at den gir, så kan det være fristende å trykke `Remediate`, som vil fikse problemet med en gang. Men, i og med at vi
praktiserer "Infrastructure as Code", så må vi gjøre endringen permanent i scriptene være. Trykk på `View remediation logic` for å se hva du må legge til ARM-templaten din:

- I Infrastruktur-prosjektet ditt, åpne azuredeploy.json.
- Editer filen, slik at dette er i samsvar med hva 'View remediation logic' viste, hvis det er ingen forslag så trenger du ikke gjøre noe.
- Redeploy Infrastruktur-prosjektet.

Det tar gjerne noen minutter fra du gjør en endring, til at endringen vises i Azure Security Center. Men, du kan sjekke at applikasjonen
kun tillatter https uansett.

(Dersom du har tid til overs, kan du gjøre samme øvelse for Storage Account'en din, og se om du får noen anbefalinger der).

### Advanced Threat Protection

Advanced Threat Protection er en tilleggstjeneste på storage account, slik at storage accounten din blir overvåket for angrep og unormal oppførsel. Dersom Security Center oppdager noe unormalt som den mener du bør se på, så vil du motta en epost med varsling om hva som har
skjedd.

Gå til din Storage Account som ble opprettet, og trykk på `Advanced Security`. Siden dette koster ekstra penger, og vi har vurdert til at dette ikke er noe som er nødvendig velger vi å ikke skru på denne i dette tilfelle.

Du kan lese mer om dette ved en senere anledning her:
[Storage advanced threat protection](https://docs.microsoft.com/en-us/azure/storage/common/storage-advanced-threat-protection?tabs=azure-portal)

## Oppsummering

I denne øvelsen har vi satt opp applikasjonen, fjernet åpen tilgang i Storage Account og undersøkt sårbarheter/anbefalinger i Azure Security Center.
