# Leksjon 1

## Forberedelser
Workshop 2 handlet om oppsett av prosjektet i Azure DevOps. Dersom du har det oppsettet fra forrige workshop kan du hoppe over disse forberedelsene.

Har du ikke et fungerende oppsett starter vi med å sette opp en pipeline for applikasjonen og en for infrastruktur slik at vi kan oppdatere løsningen i Azure ved å committe kode til et repository. 

### Opprett prosjekt i Azure DevOps
Dersom du ikke har et eksisterende prosjekt må det opprettes. En mer detaljert beskrivelse finnes i  [Workshop 2]( https://github.com/bouvet/azure-workshops/tree/master/Workshop_2/Leksjon_1 ).

1. Gå til Azure DevOps og logg inn.
1. Lag en ny organisasjon om du ikke har en allerede.
1. Lag et nytt privat prosjekt med Git som version controll og Agile som work item process.

### Opprett repository for applikasjonen
Gå til Repos og initialiser et nytt repository, gjerne med VisualStudio gitignore. Clone repoet ned til din egen maskin. For autentisering mot Azure DevOps kan du enten sette opp et access token eller en alternativ innlogging. Gå til AzureWorkshop repoet og hent filene. Dette kan du enten gjøre ved å clone det ned til egen maskin via git, eller laste ned som zip. Gå til Workshop_3/Start og kopier innholdet til ditt lokale Azure DevOps repo. Commit det og push det opp til Azure DevOps.

### Konfigurer Pipelines
I rotkatalogen til det nye git-repositoriet finnes det nå to .yaml-filer Disse inneholder definisjonen for to pipelines som vi skal sette opp, en for infrastruktur og en for applikasjon.

#### Service connection
Før vi kan deploye må vi gi Azure DevOps tilgang til ressuser i Azure Subscription. Dette gjør vi ved å opprett en _Service Connection_. Fra Azure DevOps, velg `Project settings`, deretter `Service connections` under `Pipelines`.  Trykk `New service connection`, og velg `Azure Resource Manager som type. Gi service connection navnet "Azure subscription". Sjekk at Scope level er satt til "Subscription" og "Subscription" inneholder ditt Azure-subscription. La "ResourceGrop" stå tom.

#### Infrastruktur
Først må vi konfigurere navn på tjenestene som brukes av applikasjonen slik at dette blir unikt for din applikasjon.
Åpne `AzureWorkshopInfrastruktur/AzureWorkshopInfrastruktur.sln` i VisualStudio. Åpne filen `azuredeploy.parameters.json`. I denne fila må du gi nye navn til følgende parmetre:
* webSiteName
* storageAccountName
* appInsightsName
  
Disse variablene må ha navn som er globalt unike, så hvis du velger et navn som er i bruk vil deployment feile første gang. Ingen fare, da er det bare å endre navnene i denne fila.

Commit endringene og push til Azure DevOps.

Fra Azure DevOps, velg Pipelines. Trykk `Create pipeline`. Velg Azure Repos Git (YAML). 

I steget Select repsitory, velg det repositoriet du nettopp opprettet.

Neste steg er `Configure your pipeline`. Der velger du "Existing Azure Pipelines YAML File". Da får du opp et panel for å velge YAML-fil. Under path, velg `infra-pipeline.yaml`.

Siste steg er `Review your pipeline`. Her kan du se hvordan pipelinen er definert. Trykk "Run". 

Til slutt må vi rydde litt siden vi skal lage en pipeline til. Gå til `Pipelines` og velg den du nettopp opprettet. På kontekstmenyknappen [...] velger du "Rename" og gir pipelinen navnet "Infrastruktur"

Nå skal det være opprettet en Ressursgruppe som heter `AzureskolenTest`, som vi skal deploye applikasjonen til. Sjekk gjerne dette i [portalen](https://portal.azure.com)

#### Applikasjon
Når infrastrukturen er opprettet kan vi deploye applikasjonen. Gå til `Pipelines` og velg `New pipeline`.

Som forrige gang, velg "Azure Repos Git", velg repositoriet ditt og velg å konfigurere med en "Existing Azure Pipeline YAML file". Denne gangen velger du `azure-pipelines.yaml` under Path.

Under `Review your pipeline` må du nå angi noen Variable. Trykk "Variables" og angi følgende variable

| Navn         | Verdi                                   |
|--------------|-----------------------------------------|
| Subscription | Azure subscription                      |
| WebAppName   |"Navnet du anga i infrastrukturparametre"|

Deretter kjører du pipelinen ved å trykke "Run".

Til slutt litt opprydding igjen. Gå til `Pipelines` og velg den du nettopp opprettet. På kontekstmenyknappen [...] velger du "Rename" og gir pipelinen navnet "Applikasjon".

### Oppsummering 
Nå skal det være opprettet en ressursgruppe med alle Azure-tjenestene applikasjonen trenger, og applikasjonen skal være deployet til Azure. Du kan nå teste den ved å gå til https://"webappnavn".azurewebsites.net og laste opp et bilde.

## Azure Security Center

Azure Security Center er en tjeneste i Azure som overvåker tjenestene dine, og leter etter mulige konfigurasjoner som kan gjøre tjenestene
dine usikre.

I Basic tier så får du gratis anbefalinger på tiltak du kan gjøre for å forbedre sikkerheten i tjenestene dine.

I tillegg har den et Standard tier, som tilbyr utvidet overvåkning av tjenestene. Dette må settes opp per tjeneste og koster ekstra.

### App Service

Nå skal du se om du får noen anbefalinger fra Security Center på App Servicen din.

* Gå til `webappnavn` som du opprettet i forrige oppgave.
* Trykk så på `Security`, og du kommer til et skjermbilde som viser alerts.
* Se om du har noen sårbarheter som bør utbedres. Her vil du mest sannsynlig se en melding om at det anbefales å skru på slik at kun

Når du nå ser at den gir, så kan det være fristende å trykke `Remediate`, som vil fikse problemet med en gang. Men, i og med at vi 
praktiserer "Infrastructure as Code", så må vi gjøre. Trykk på `View remediation logic` for å se hva du må legge til ARM-templaten din:

* I Infrastruktur-prosjektet ditt, åpne azuredeploy.json.
* Editer filen, slik at dette er:
* Commit endringen din og push endringen din til Azure DevOps.
* Sjekk at Infrastruktur-prosjektet ditt blir bygd og deployet.

Det tar gjerne noen minutter fra du gjør en endring, til at endringen vises i Azure Security Center.

(dersom du har tid til overs, kan du gjøre øvelse for Storage Account'en din, og se om du får noen anbefalinger der).

### Advanced Threat Protection (Usikker på om denne skal være med)
Advanced Threat Protection er en tilleggstjeneste på storage account, slik at storage accounten din blir overvåket for angrep og unormal oppførsel. Dersom Security Center oppdager noe unormalt som den mener du bør se på, så vil du motta en epost med varsling om hva som har 
skjedd.

Gå til din Storage Account som ble opprettet, og trykk på `Advanced Security`. Trykk på `Enable Seucirty

Siden dette koster ekstra penger, og vi har vurdert til at dette ikke er noe som er nødvendig velger vi å ikke skru på denne i dette tilfelle.

Du kan lese mer om dette ved en senere anledning her:
[Storage advanced threat protection](https://docs.microsoft.com/en-us/azure/storage/common/storage-advanced-threat-protection?tabs=azure-portal)

### Oppsummering
I denne øvelsen har du stiftet kjennskap til Azure Security Center. Du har sett hvordan 
