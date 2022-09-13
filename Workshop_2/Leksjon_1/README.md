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
1. Lage et nytt prosjekt i [Azure DevOps](https://dev.azure.com)
2. Lage App Services i [Azure](https://portal.azure.com)
3. Sette opp build pipeline
4. Sette opp release pipelines
5. Gjøre endringer til kildekoden
6. Deploye til alle miljøene

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
>- **Artifacts:** Om flere av prosjektene dine har avhengigheter til sentrale komponenter som du ikke vil dele med hele verden kan de publiseres til artifacts. Her er det støtte for både nuget og npm pakker som resten av prosjektene ditt kan ha avhengigheter til. 

Gå til Repos og initialisere med VisualStudio gitignore. Clone repoet ned til din egen maskin. For autentisering mot Azure DevOps kan du enten sette opp et access token eller en alternativ innlogging. Gå til [AzureWorkshop repoet](https://github.com/bouvet/azure-workshops) og hent filene. Dette kan du enten gjøre ved å clone det ned til egen maskin via git, eller laste ned som zip. Gå til `Workshop_2/Start` og kopier innholdet til ditt lokale Azure DevOps repo. Commit det og push det opp til Azure DevOps. 

## 2: Lag App Services i [Azure](https://portal.azure.com)
For å kunne deploye må vi ha noe å deploye til. Lag en web App Service for test, og en for prod i [Azure](https://portal.azure.com). Husk å bruke samme brukeren i Azure som i Azure DevOps. 

Det er som oftest lurt å lage separate ressursgrupper for forskjellige miljøer. Når det gjelder app service plan trenger vi ikke noen kraftige greier. Det holder med en F1 pricing tier. Anbefaler at dere gir ressursgruppene, app service planene, og app servicene et navn som gjør det lett å få oversikt over hvilke Azure ressurser som hører til hvilket miljø. Dette gjør det lettere å identifisere miljøene når vi skal sette opp build pipelinen. Eksempelvis for app servicene:
- **Test**: [NavnPåApp]-test
- **Prod**: [NavnPåApp]-prod

Fremgangsmåte:
1. Gå til [Azure](https://portal.azure.com)
2. Lag en resource group for hvert av miljøene, Valgfritt (anbefalt: west europe, north europe eller norway east/west)
3. Lag en app service for hvert av miljøene, som du kopler opp mot hvert sin resource group og hver sin service plan.

    * <b>Name:</b> Dette navnet må være unikt i hele Azure, da den vil kunne nås fra &lt;appservicenavn&gt;.azurewebsites.net. 
    * <b>Publish:</b> Code
    * <b>Runtime Stack:</b> .NET 6 (LTS)
    * <b>OS:</b> Windows
    * <b>Region:</b> Benytt samme region som ressursgruppen.
    * <b>App service plan:</b> Endre sku and size til Free F1 under dev/test.

4. Klikk på review and create. 

## 3: Opprett storage accounts i [Azure](https://portal.azure.com)
For at web applikasjonen skal fungere i begge miljøer, behøver vi en storage account for test, og en for prod. 
Storage accountene opprettes i samme ressursgruppe for web applikasjonen.

1. Gå til [Azure](https://portal.azure.com) 
2. Lag en storage account for hvert miljø, velg samme ressursgruppe som web applikasjonen. 
   * <b>Storage account name</b> Navnet må være unikt i hele azure, lengde fra 3-24 tegn og kan kun innholde små bokstaver og tall. Postfix storage accounten med `test`/ `prod`. Eksempel `ws2knuteltest`
   * <b>Region</b> Velg samme region som ressursgruppen.
   * <b>Performance</b> Standard
   * <b>Redundancy</b> LRS

3. Klikk på Review og create.
4. Gå til Opprettet storage account, og navigerer inn i Containers.
5. Opprett en ny Container som du kaller for: imagecontainer med private access level. 

## 4: Sett opp connection mellom Azure DevOps og Azure Portalen. 
Vi trenger å sette opp en tilgang for DevOps til ditt subscription i Azure. 
Dette kan gjøres med en Service connection. 
1. I Azure devops, klikker du på Project settings (tannhjul nede til venstre)
2. Under Pipelines, så klikker du på Service Connections.
3. Klikk på new Service Connection. Det vil dukke opp ett panel, her velger du Azure Resource Manager. Klikk så Next.
4. Velg Service Principal (Automatic), klikk next.
   1. Scope Level: Subscription.
   2. Subscription: Velg riktig subscription.
   3. Resource Group : La stå blankt. 
   4. Service connection name: Fyll dette ut, navnet må du huske til ett senere steg.
   5. Huk av Grant access permission to all pipelines
5. Klikk Save.

Nå har vi opprettet en Service connection som vi skal bruke for å deployere ressurser i senere steg. 

## 5: Sett opp build pipeline
Nå som du har opprettet et prosjekt i Azure DevOps og importert et Git repo kan vi sette opp en build pipeline for å automatisere bygging og testing av applikasjonen. Azure DevOps har to måter å sette opp en build pipeline på:
>- Via build pipeline designeren
>- Via YAML script
>
Microsoft anbefaler å sette opp et YAML script som definerer build pipelinen din. Fordelen med å definere build pipelinen din i et script er at man kan sjekke det inn i kildekoden. Azure DevOps vil lese YAML fila og sette opp pipelinen din som et steg før selve applikasjonen din kjøres gjennom den. På den måten kan man ikke bare endre selve applikasjonen ved en commit, men pipelinen også. Det blir i tillegg mulig å rulle tilbake selve pipelinen din om en feil skulle oppstå.

Den andre måten å gjøre det på er gjennom designeren. Det negative med denne fremgangsmetoden er at definisjonen av pipelinen din ikke er lagret i kildekoden din, med alle implikasjoner det gir. Microsoft ønsker at man skal bruke YAML og har nå gjort det ca like lett å bruke yaml som designeren (med pek og klikk).

>Azure DevOps gjør det forholdsvis enkelt å traversere mellom å bruke designeren og YAML filer om det skulle ønskes. Så man kan starte med designeren for så å konvertere pipelinen YAML om man skulle ønske det.

I Azure DevOps, gå til "*Pipelines*" => "*Pipelines*" => "*Create pipeline*". 
* Velg deretter "Azure Repos Git (YAML)". 
* Velg så det repoet vi importerte til Azure DevOps prosjektet vårt i steg 1 (det er det som er valgt for oss som default om vi kun har et repo i prosjektet vårt). 
* På neste steg kan vi velge å enten benytte en YAML fil som allerede finnes i repoet, starte med en minimalistisk YAML fil, eller starte fra en template.
* Velg templaten "ASP.NET CORE". Denne templaten gir oss alt vi trenger for å komme i gang. Må muligens klikke på Show more for å få opp alle templates. 
* Trykk så på "Save and Run", DevOps vil så gi deg muligheten til å commite yaml filen til repoet. 
* Trykk "Save and Run".

Nå har vi kommet til siden hvor vi kan se byggingen av koden. YAML-filen som devops gir når vi bygger .NET Core er ikke stor, den inneholder bare ett steg. Følg med på bygg jobben og se om noe går galt. 

Om du har gjort alt riktig så burde build steget feile og det er meningen. .NET Core YAML-filen har build steg uten at prosjekt er oppgitt, i filen oppgir vi ikke hvilket prosjekt vi skal bruke og vi har to mapper i repoet så den finner ingen csproj eller sln filer. Dette skal vi nå gjøre noe med.
* Gå til Pipelines
* Velg Pipelinen du lagde
* Trykk Edit
* Slett alt under steps: (burde være to linjer, en med 'script:' og en med 'displayname:')
* Legg til .Net Core task
* Velg publish under .Net Core tasken
* Huk vekk Publish web projects
* Under 'Path to project(s)' legger du inn 'AzureWorkshop/AzureWorkshop.sln'
* Under Arguments skal det stå --configuration $(buildConfiguration)
* Trykk Add
* Trykk Save og Save (commit to pipeline)
* Trykk Run

>Hvis du trykker på Job så kan du se på outputen til pipelinen din. Her burde alt gå grønt (eller ikke få farge) hvis alt går riktig, hvis noe går galt blir det rødt. 

Se gjennom loggene og gjør deg litt kjent. Vi skal nemlig utvide yaml filen med deploy og da kan det være at du støter borti problemer og loggene kan da brukes til å finne ut hva som gikk galt.

Vi skal nå legge inn deploy av Web Appen til Azure i YAML-filen. Gå til edit av pipelinen og gjør følgende
* Legg til tasken 'Azure App Service deploy'
* Velg ConnectionType Azure resource manager og Azure Subscription velger du Service connection du opprett i steg 4.
* Sett App Service type til Windows/Linux (dette må matche det man valgte når man opprettet Web Appen)
* Velg applikasjonen du lagde
* Legg til displayName med et fornuftig navn på steget (f.eks. Deploy Web App to Azure)
* Trykk Add
* Trykk Save og Save (commit to pipeline)
* Trykk Run

Nå kan du se på jobben og hvis alt ble grønt så kan du gå til Deploy Web App to Azure tasken og åpne linken til applikasjonen du lastet opp til azure. Alternativt så kan du finne urlen via portalen. Se at nettsiden nå funker. 
Hvis noe gikk galt i bygg eller deploy stegene så se gjennom loggene og prøv å fikse problemet før du ber om hjelp. 

Vi har nå laget en basic build og deploy pipeline, men for å få en fullstendig pipeline så pleier man ha en stødigere struktur enn det vi har laget til nå hvor alt er samme job og stage. Vi skal nå endre litt på strukturen og her må man være forsiktig med whitespace for man må fort endre på det manuelt.

* Gå til edit av pipeline
* Endre på steps til stages
* Nå må du legge til et par linjer før - task (se eksempel under)
* Legg til stage for build og deploy før hver sine respektive tasker
* Legg til en publish build artifact task etter dotnet core publish tasken
* I arguments til dotnet core publish tasken legger du til  --output $(Build.ArtifactStagingDirectory) (ikke fjern --configuration )
* Legg til en download build artifact task før app service deploy tasken i den andre stagen
* Sett artifact name til drop
* Sett destination directory til $(System.DefaultWorkingDirectory) (denne må matche med inputen 'packageForLinux' i app service deploy tasken)
* Lagre og kjør pipelinen

Resultatet burde ligne på dette:
```
stages:
  - stage: build
    jobs:
    - job: build
      steps:  
      - task: DotNetCoreCLI@2
        inputs:
          ****
      - task: PublishBuildArtifacts@1
        inputs:
          ****
  - stage: deploy
    jobs:
    - job: deploy
      steps:
      - task: DownloadBuildArtifacts@1
        inputs:
          ****
      - task: AzureRmWebAppDeployment@4
        inputs:
          ****
```

Pipelinen burde nå ha to stages, en build og en deploy og begge disse har hver sin job. Det vi nå kan gjøre er å bruke forskjellige stages og bruke environments til å styre publisering til dev og prod. Hvis noe har gått galt i bygg og deploy stegene så kan du se på azure-pipeline.yaml i komplett folderen for inspirasjon, men prøv å løse problemet selv først.

## 6: Environments 
Vi har nå en pipeline som har flere stages og flere kan legges inn. Det kan f.eks. legges inn en til for produksjon ved å bare kopiere deploy stagen og ha en som heter deployTest og en som heter deployProd. For å bruke environments med forskjellige regler som vi skal sette opp under så må vi derfor gjøre nettopp det. Gå til pipelines og editer pipelinen din slik at du nå får 3 stages: build, deployTest og deployProd (husk å endre på web appen du peker på for prod). Det går an å ha flere pipelines i samme YAML-fil eller som forskjellige YAML-filer, men dette går vi ikke gjennom nå.

La oss lage vårt første Environment. 
* Gå til Environments under Pipelines
* Trykk Create environment
* Skriv inn Test som Name (dette blir test miljøet vårt)
* Legg til en beskrivelse som passer
* La none stå som resources
* Trykk Create
* Gå ut av Test environmentet
* Trykk på New environment
* Repeter stegene over for Produksjon

Vi skal nå legge til brukergodkjenning for deploy til prod. Det gjør vi ved å gå inn i environmentet Prod/Produksjon. Oppe i høyre hjørnet er det en knapp med dre dots på, trykk her og så på "Approvals and checks". Velg så Approvals og legg til deg selv som Approver (dette kan gjøres gjennom + tegnet eller ved å trykke på Approvals under "Add your first check"). Etter å ha trykket på create så skal "Approvals and checks" siden nå inneholde en entry med "All approvers must approve". Nå må vi bare koble dette miljøet sammen med deployProd stagen i YAML-filen. Gå tilbake til edit av pipeline og gjør følgende:

* Finn deployProd Stagen (det er her vi skal gjøre endringer)
* Under jobs: så skal vi endre fra "- job: deploy" til "- deployment: deploy"
* Legg til "environment: Prod " under "- deployment" (dette må matche environment navnet til produksjon)
* Steps blir nå gule (de er feil sted)
* Under environment legg til "strategy:"
* Under strategy (ett hakk inn) legges "runOnce:"
* Legg så "deploy:" ett hakk inn under "runOnce:"
* Tab så resten av YAML-filen slik at steps kommer ett hakk inn for "deploy:"
* Lagre og kjør pipeline

Eksempel
```
  - stage: deploy_test
    jobs:
      - deployment: deploy
        environment: Test
        strategy: 
          runOnce:
            deploy:
              steps:
              - task: DownloadBuildArtifacts@1
                displayName: 'Download artifacts'
                inputs:
                  ****
              - task: AzureRmWebAppDeployment@4
                displayName: 'Deploy web app to azure'
                inputs:
                  ****
  - stage: deploy_prod
    jobs:
      - deployment: deploy
        environment: Prod
        strategy: 
          runOnce:
            deploy:
              steps:
              - task: DownloadBuildArtifacts@1
                displayName: 'Download artifacts'
                inputs:
                  ****
              - task: AzureRmWebAppDeployment@4
                displayName: 'Deploy web app to azure'
                inputs:
                 ****
```

Nå skal Prod kreve at du godkjenner deploy før den kjører gjennom. Via environment kan vi nå styre Prod og Test deployment. Gå til environments og velg Test eller Prod og se litt på mulighetene som finnes.


## 7: Legg til testing
Siste steget vi skal gjøre er å få lagt til en task for å kjøre gjennom tester. Testen som finnes i prosjektet skal feile og du kan rette på koden etter at pipelinen stopper opp på grunn av testen, for så å laste opp endringen og se at alt går gjennom. Som vanlig må vi til pipelinen og editere denne.

* Legg til en ny .NET Core task før PublishBuildArtifact
* Sett command til test
* Under 'Path to Project(s)' legger du 'AzureWorkshop/AzureWorkshopAppTests/AzureWorkshopAppTests.csproj'
* Legg til displayName for tasken
* Endre stage og jobb navnet fra build til buildAndTest
* Lagre 

Gå tilbake til pipelinen og se at testen nå stopper første stagen fra å kjøre gjennom. Fiks testen i prosjektet og last opp koden og se at det går gjennom. Nå har vi blandet build og test i samme stage og job, men det er ofte man gjør forskjellige tester basert på hvor langt i pipelinen man har kommet. Da lager man egne stages for f.eks. Unit test, integration test og smoke test og legger de inn som egne stages med DownloadArtifact og unzip task for så å kjøre gjennom, for integration tester vil man kanskje sette opp connections til andre tjenester i stagen eller kjøre mot test miljøet sitt. 

>En artifact er produktet av en build pipeline i form av en zip fil. Denne filen inneholder alt som ble lagt i mappen man publisher artifacten fra. Zip filen vil beholde mappestruktur og alle filer som default. Artifakten lastes opp på en server og blir tilgjengelig for nedlasting og andre stager kan da bruke current build og vil resolve hvor den henter artifakten fra selv. 

## 8: Endre trigger
For å forberede til Leksjon 2 så ønsker vi å legge inn en sjekk på triggeren i YAML-filen vår. Vi er kun interessert i endringer som ikke skjer i AzureWorkshopInfrastruktur. Alle endringer i den mappen hører til infrastruktur og trenger ikke å starte en bygg og deploy prosess. Dette er fordi vi skal gjøre en del endringer i neste leksjon og ønsker ikke å binde opp unødvendig byggetid. Åpne YAML-filen, enten lokalt eller gjennom Azure DevOps slik du har gjort i de tidligere oppgavene og erstatt 
```
trigger: 
- main
```
 med
```
trigger:
  branches: 
    include:
      - main
  paths:
    exclude:
      - AzureWorkshopInfrastruktur
```
Bytt ut main med master hvis du har master som branch og ikke main. Alternativt et annet branch navn hvis du har valgt å bruke det. Main er den nye standarden til git og master er den gamle, bare å bruke den du har.

## Mer? Lek deg litt
Prøv å gjøre endringer til applikasjonen og sjekk inn. Om du vil ha en utfordring kan du prøve å sette opp variabel substitution i appsettings.json på hvert av stegene dine. Får du til å postfikse tittelen i applikasjonen med miljøet du er i? 