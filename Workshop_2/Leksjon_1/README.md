# Azure DevOps

Denne leksjonen tar for seg Azure DevOps. Azure DevOps er en platform som inneholder mesteparten, om ikke alt av tjenester man trenger for å drive et moderne utviklingsprosjekt. Dette inkluderer:
- Wiki
- Kildekontrollsystem
- Task tracking system
- Bygg og release pipeline
- Test rammeverk
- Innebygget nuget feeds


>Det er lett å forvirre Azure og Azure DevOps. **Merk** at dette er to separate tjenester. Azure tar for seg infrastruktur som hoster software, mens Azure DevOps støtter opp under selve utviklingsprosessen av softwaren. 

I denne leksjonen skal vi lage bygg og release pipeline for løsningen som ble laget i workshop 1. I grove trekk skal vi gjøre følgende: 
1. Lag et nytt prosjekt i [Azure DevOps](https://dev.azure.com)
2. Lag App Services i [Azure](https://portal.azure.com)
3. Sett opp build pipeline
4. Sett opp release pipelines
5. Gjør endringer til kildekoden
6. Deploy til alle miljøene

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

Gå til Repos og initialisere med VisualStudio gitignore. Clone repoet ned til din egen maskin. For autentisering mot Azure DevOps kan du enten sette opp et access token eller en alternativ innlogging. Gå til [AzureWorkshop repoet](https://github.com/bouvet/azure-workshops) og hent filene. Dette kan du enten gjøre ved å clone det ned til egen maskin via git, eller laste ned som zip. Gå til `Workshop_2/Start` og kopier innholdet til ditt lokale Azure DevOps repo. Commit det og push det opp til Azure DevOps. 

## 2: Lag App Services i [Azure](https://portal.azure.com)
For å kunne deploye må vi ha noe å deploye til. Lag en web App Service for test, og en for prod i [Azure](https://portal.azure.com). Husk å bruke samme brukeren i Azure som i Azure DevOps. 

Det er som oftest lurt å lage separate ressursgrupper for forskjellige miljøer. Når det gjelder app service plan trenger vi ikke noen kraftige greier. Det holder med en F1 pricing tier. Anbefaler at dere gir ressurs gruppene, app service planene, og app servicene et navn som gjør det lett å få oversikt over hvilke Azure ressurser som hører til hvilket miljø. Dette gjør det lettere å identifisere miljøene når vi skal sette opp build pipelinen. Eksempelvis for app servicene:
- **Test**: [NavnPåApp]Test
- **Prod**: [NavnPåApp]

Fremgangsmåte:
1. Gå til [Azure](https://portal.azure.com)
2. Lag en resource group for hvert av miljøene, med hver sin service plan 
4. Lag en app service for hvert av miljøene, som du kopler opp mot hvert sin resource group og hver sin service plan.

    * <b>Name:</b> Dette navnet må være unikt i hele Azure, da den vil kunne   nås fra &lt;appservicenavn&gt;.azurewebsites.net. 
    * <b>Publish:</b> Code
    * <b>Runtime Stack:</b> .NET core 3.1 (LTS)
    * <b>OS:</b> Windows
    * <b>Region:</b> Valgfritt

## 3: Sett opp build pipeline
>Nå som du har opprettet et prosjekt i Azure DevOps og importert et Git repo kan vi sette opp en build pipeline for å automatisere bygging og testing av applikasjonen. Azure DevOps har to måter å sette opp en build pipeline på:
>- Via build pipeline designeren
>- Via YAML script
>
>Microsoft anbefaler å sette opp et YAML script som definerer build pipelinen din. Fordelen med å definere build pipelinen din i et script er at man kan sjekke det inn i kildekoden. Azure DevOps vil lese YAML fila og sette opp pipelinen din som et steg før selve applikasjonen din kjøres gjennom den. På den måten kan man ikke bare endre selve applikasjonen ved en commit, men pipelinen også. Det blir i tillegg mulig å rulle tilbake selve pipelinen din om en feil skulle oppstå.
>
>Den andre måten å gjøre det på er gjennom designeren. Det negative med denne fremgangsmetoden er at definisjonen av pipelinen din ikke er lagret i kildekoden din, med alle implikasjoner det gir. Det fine med å bruke designeren er at man minimerer terskelen for å sette opp en pipeline for en uerfaren DevOps-er. Designeren er en fin måte å oppdage hvilke steg som finnes, og hvilke innstillinger som finnes til hvert av stegene. 
>
>Azure DevOps gjør det forholdsvis enkelt å traversere mellom å bruke designeren og YAML filer om det skulle ønskes. Så man kan starte med designeren for så å konvertere pipelinen YAML om man skulle ønske det.

I Azure DevOps, gå til "*Pipelines*" => "*Pipelines*" => "*Create pipeline*". 
Velg deretter "Use the classic editor to create a pipeline without YAML." nederst på siden.

Velg så det repoet vi importerte til Azure DevOps prosjektet vårt i steg 1 (det er det som er valgt for oss som default om vi kun har et repo i prosjektet vårt).

På neste steg kan vi velge å starte fra et template, en tom jobb, eller opprette en YAML fil. Vi velger templaten "*Azure Web App for ASP.NET*". Denne templaten gir oss alt vi trenger for å komme i gang.

>Nå har vi kommet til siden hvor vi kan sette opp bygg stegene våre via en YAML-fil. Her ser vi hva templaten vi valgte i forige steg inneholder. 

Under "*pipeline*", sett solution til `AzureWorkshop/AzureWorkshop.sln`.

>For vår del inneholder repoet vårt kun en solution og kunne strengt tatt hatt defaulten som henter alle solutions i repoet. Men det er alltid en god ide å være spesifikk på hva man vil bygge, i tilfelle man vil legge til flere solutions i fremtiden, hvilket vi kommer til å gjøre senere.

Finn Azure subscriptionen din under dropdownen på samme side og autentiser deg. Etter du har autentisert deg, velg test miljøet vi satt opp i steg 2.

Under "*VsTest - testAssemblies*" => "*Test files*" sjekk at det står følgende:
```
**\$(BuildConfiguration)\**\*tests*.dll
!**\obj\**
```

>Templaten vi får fra Azure DevOps er litt for grådig i hvilke filer den skal kjøre tester på. Her forteller vi at vi har lyst til å teste alle dll'er som har "tests" i navnet sitt.

Under "*VsTest - testAssemblies*" => "*Control Options*" slå av "*Enabled*".

>Dette vil skru av testing steget, men slapp av, vi skal skru det på igjen om ikke så lenge. 😊

Under "*Azure Web App Deploy*" => "*App Type*" velg Web App on Windows

Nå kan du trykke "*Save & Queue*", lene deg tilbake og se at du har en pipeline som bygger koden din, og stapper den ut på en App Service i Azure. Når bygg agenten er ferdig å kjøre kan du gå browse App Servicen din og se at applikasjonen ligger der, klar til bruk. 

Men nå har vi jo releasa appen vår gjennom bygg pipelinen vår. Hva er vitsen med release pipelines? Å release gjennom bygg er vell og bra for små applikasjoner. Men når du trenger litt mer kontroll over releasene dine kan du trenge en release pipeline.

Før vi gjør det kan du skru på testene dine igjen ved å editere build pipelinen din. Hvordan navigerer du deg til edit build pipeline og skrur på testene igjen? Left as an exercise to the developer. 

I tillegg kan du skru av "*Azure App Service Deploy*" steget ettersom vi skal deploye i release pipeline i steden for build pipeline snart. 

Når du har skrudd på testene igjen klikk "*Save & Queue*". Nå vil Azure DevOps lete etter en ledig agent for å kjøre bygg jobben igjen, og om alt er gjort riktig vil bygget feile på testingen. Hvorfor? Fordi noen glemte å kjøre testene lokalt før de sjekket inn, og det er din jobb å fikse det! 😈

>Jeg vet hva du tenker, og svaret er nei! Nei, du fikser det ikke med å skru av testene igjen din latsabb! 😆 Dette er steg 5 mat. 

## 4: Sett opp release pipelines
Gå til "*Pipelines*" => "*Releases*" => "*New pipeline*" => "*Add an artifact*". Her velger vi build pipelinen vi lagde i forige steg som source. Default version skal være "*Latest from the build pipeline default branch with tags*". Trykk "*Add*".

>En artifact er produktet av en build pipeline i form av en zip fil. Denne filen inneholder alt som trengs for å deploye applikasjonen til en host. I dette steget har vi valgt artifacten som ble produsert av bygget vi satt opp tidligere. 

Etter artifacten er lagt til, trykk på lyn ikonet over artifakten og slå på "*Continuous deployment trigger*". 

>Denne instillingen trigger release pipelinen vår hver gang en ny artifakt er klar for release.

Trykk "*Add a stage*", velg "*Azure App Service deployment*" templaten, og kall den nye stagen for "Test". 

Kopier test staget du akkurat lagde og kall den for "Prod". Trykk på "*Pre-deployment conditions*" (ikonet med et lyn og en person) på prod stagen. Enable "*Pre-deployment approvals*" og sett deg selv som approver. 

Trykk på "*Tasks*" tabben øverst i bildet og velg "*Test*". Koble denne stagen opp mot test App Servicen på samme måte som vi gjorde i steg 3. Gjør det samme for Prod.

Du må gjerne gi release pipelinen et navn ved å redigere det øverst i bildet. Når du er ferdig, trykk "*Save*" øverst i høyre hjørne og lagre release pipelinen under root mappa. 

Vi har nå satt opp en release pipeline som går mot forskjellige miljøer i Azure. I tillegg må du approve en release før den går fra test til Prod.

Nå er det på tide å se om alt snurrer. Men først... Hadde vi ikke en test som feila? 😏

## 5: Gjør endringer til kildekoden
Åpne `[DittLokaleRepo]\AzureWorkshop\AzureWorkshop.sln` og fiks testen.

Når testen er fikset og koden er sjekket inn burde build pipelinen starte en ny jobb som bygger og tester den nye commiten. Når bygget er ferdig burde release pipelinen merke at en ny artifact er klar for deploy og trigge en ny deploy mot test miljøet. For at releasen skal deployes på Prod må de godkjennes av deg først.

## 6: Deploy til alle miljøene
Approve release mot Prod og se at applikasjonen kjører i begge miljøene dine. 

## Mer? Lek deg litt
Prøv å gjøre endringer til applikasjonen og sjekk inn. Om du vil ha en utfordring kan du prøve å sette opp variabel substitution i appsettings.json på hvert av stagene dine. Får du til å postfikse tittelen i applikasjonen med miljøet du er i? 