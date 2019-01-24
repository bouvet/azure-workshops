# Azure DevOps

Denne leksjonen tar for seg Azure DevOps. Azure DevOps er en platform som inneholder mesteparten, om ikke alt av tjenester man trenger for å drive et moderne utviklingsprosjekt. Dette inkluderer:
- Wiki
- Kildekontrollsystem
- Task tracking system
- Bygg og release pipeline
- Test rammeverk
- Innebygget nuget feeds


>Det er lett å forvirre Azure og Azure DevOps. **Merk** at dette er to separate tjenester. Azure tar for seg infrastruktur som hoster software, mens Azure DevOps støtter opp under selve utviklingsprosessen av softwaren. 

I denne leksjonen skal vi lage bygg og release pipeline for løsningen som ble laget i workshop 1. 

1. Lag et nytt prosjekt i [Azure DevOps](https://dev.azure.com)
2. Sett opp build pipeline
3. Lag en web App Service for test, QA, og prod i [Azure](https://portal.azure.com)
4. Sett opp release pipeline mot miljøene

## 1: Lag et nytt prosjekt i Azure DevOps

Gå til [Azure DevOps](https://dev.azure.com) og logg inn. 

>Om du ikke har registrert deg på Azure DevOps kan du gjøre det via samme linken. **Det anbefales at du benytter samme MS-konto på Azure DevOps som du gjør i Azure**. Dette er for å slippe autoriseringskonfigurasjon for de forskjellige brukerene dine på tvers av Azure DevOps og Azure. 

Lag en ny organisasjon om du ikke har en allerede. 

>En organisasjon er det øverste nivået i Azure DevOps. Herfra kan man legge til brukere, administrere rettigheter, administrere prosjekter, legge til integrasjoner (eks slack), med mer.

Lag et nytt privat prosjekt med Git som version controll og Agile som work item process.

>Et prosjekt inneholder tjenester man trenger for å drive et moderne utviklingsprosjekt. Prosjektene i Azure DevOps er strukturert som følger:
>- **Overview:** Herfra kan man lage dashboards for å vise nøkkelinformasjon om prosjektet ditt. For eksempel om miljøene, pipelines, tasks, eller tester. Det er i tillegg her wikien til prosjektet ditt lever.
>- **Boards:** Dette er hvor taskene til prosjektet ditt er. Herfra kan du sette opp en backlog, organisere sprinter etc.
>- **Repos:** Det er her versjonskontroll repoene dine befinner seg. Herfra kan du få en oversikt over slike ting som filer, brancher og pull requests. 
>- **Pipelines:** Herfra kan du se, og konfigurere build og release pipelinene dine. Du kan lett få en oversikt over hvilke pipelines som gikk bra, hvilke som ikke gikk bra og hvorfor. 
>- **Test Plans:** Herfra kan prosjektet ditt koordinere og planlegge testing av applikasjonen din. Om build pipelinen din har et test steg kan det konfigureres til å rapportere testens resultat hit. 
>- **Artifacts:** Om flere av prosjektene dine har avhengigheter til sentrale komponenter som du ikke vil dele med hele verden kan de publiseres til artifacts. Dette er Azure DevOps interne nuget package feed som resten av prosjektene ditt kan ha avhengigheter til. 

Gå til Repos og importer [AzureWorkshop repoet](https://github.com/bouvet/azure-workshops).

## 2: Sett opp build pipeline
Nå som du har opprettet et prosjekt i Azure DevOps og importert et Git repo kan vi sette opp en build pipeline for å automatisere bygging og testing av applikasjonen. Azure DevOps har to måter å sette opp en build pipeline på:
- Via build pipeline designeren
- Via YAML script

Microsoft anbefaler å sette opp et YAML script som definerer build pipelinen din. Dette er et forholdsvis nytt konsept hvor ikke alt er støttet enda. Fordelen med å definere build pipelinen din i et script er at man kan sjekke det inn i kildekoden. Azure DevOps vil lese YAML fila og sette opp pipelinen din som et steg før selve applikasjonen din kjøres gjennom den. På den måten kan man ikke bare endre selve applikasjonen ved en commit, men pipelinen også. Det blir i tillegg mulig å rulle tilbake selve pipelinen din om en feil skulle oppstå.

Den andre måten å gjøre det på er gjennom designeren. Det neggative med denne fremgangsmetoden er at definisjonen av pipelinen din ikke er lagret i kildekoden din, med alle implikasjoner det gir. Det fine med å bruke designeren er at man minimerer terskelen for å sette opp en pipeline for en uerfaren DevOps-er. Designeren er en fin måte å oppdage hvilke steg som finnes, og hvilke innstillinger som finnes til hvert av stegene. 

Azure DevOps gjør det forholdsvis enkelt å traversere mellom å bruke designeren og YAML filer om det skulle ønskes. Så man kan starte med designeren for så å konvertere pipelinen YAML om man skulle ønske det.

Ettersom vi er (eller later som vi er) uerfarene DevOps-ere skal vi opt-out for YAML pipelines i dette leksjonen. Trykk på portrettet ditt oppe i høyre hjørne i Azure DevOps og trykk på "*Preview features*". Slå av "*New YAML pipeline creation experience*". 







## Kildekontroll

### Opprettelse / import av git repository.
- Sjekk ut koden fra github
- Importere koden i git-repo i Azure DevOps.


### Build-pipeline med tester
Opprette bygge-pipeline, se at bygg bygges, mens at det feiler i test.
Fix test. Sjekk inn og se at bygget passer.



