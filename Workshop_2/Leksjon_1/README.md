# GitHub Devops

Denne leksjonen tar for seg devops. Vi bruker her GitHub som er en plattform som inneholder tjenester man trenger for å drive et moderne utviklingsprosjekt.

>GitHub er en av to plattformer Microsoft tilbyr for å implementere en devops pipeline. Azure Devops er den "klassiske" plattformen som fortsatt har mest funksjonalitet som understøtter store bedrifts prosjekt, men GitHub er plattformen Microsoft ønsker å gå videre med selv om Azure Devops blir ikke lagt ned på lenge. Vi bruker GitHub i denne leksjonen også fordi den er enklere å bruke som enkeltperson og derfor egner seg ypperlig til å teste på.

I denne leksjonen skal vi lage bygg og release pipeline for løsningen som ble laget i workshop 1. I grove trekk skal vi gjøre følgende:

1. Lage et nytt prosjekt i [GitHub](https://github.com/)
2. Sette opp pipeline med build og delivery skritt
3. Sette opp release, hvis du har et MSDN abonnement
4. Gjøre endringer til kildekoden
5. (Valgfritt) Lage App Services i [Azure](https://portal.azure.com)
6. Publisere til alle miljøene

>**Forutsetninger** for denne leksjonen er at du har både en egen github konto og en egen Azure subsription. Enten som en del av MSDN eller en demo Azure konto.

## Nytt repo i GitHub

Gå til [GitHub](https://github.com/) og logg inn.

Om du ikke har registrert deg på GitHub, opprett ny bruker lag en nytt repository. Du kan godt gjøre det offentlig siden det gir deg mer "kreditt" og repo er bare for å teste.

I tillegg til versjonskontroll med Git består GitHub av moduler for å drive utviklingsprosesser, som prosjekt, automatisering og overvåkning. Vi skal her konsentrere oss om å bruke automatisering til å sette opp en CI/CD pipeline. (I GitHub kan du automatisere mer enn bare bygging av pipeline.)

>### GitHub er strukturert som følger
>
>- **Repository:** Repository er der prosjektets filer og revisjonshistorikk er lagret. De støtter samarbeid ved å la flere bidragsytere jobbe på samme prosjekt, spore endringer og administrere versjoner.
>- **GitHub Actions:** GitHub Actions kan brukes som en CI/CD-plattform (Continuous Integration/Continuous Deployment) som automatiserer arbeidsflyter. Den lar deg bygge, teste og distribuere koden din direkte fra GitHub.
>- **Prosjekter:** GitHub Projects er et prosjektstyringsverktøy som integreres med depotene dine. Det hjelper deg med å organisere og prioritere arbeidet ditt ved hjelp av tavler i Kanban-stil.
>- **Overvåking:** GitHub inneholder overvåkingsverktøy for å spore statusen og ytelsen til arbeidsflytene og repositoriene dine. Dette inkluderer visning av logger, måledata og varsler.
>- **Artifacts:** @actions/artifact pakken i GitHub Actions brukes til å laste opp og laste ned artefakter i arbeidsflytene dine. Artefakter er filer eller samlinger av filer som genereres under en arbeidsflytkjøring, for eksempel kompilering, testresultater eller loggfiler. Denne pakken lar deg beholde disse filene etter at en jobb er fullført og dele dem mellom jobber i samme arbeidsflyt.
>- **@actions:** GitHub har et helt bibliotek med actions som du kan laste ned å bruke i en automatiseringsprosess du ønsker å opprette.

>### Komponenter i en GitHub arbeidsflyt
>
>1. **Workflow:** En arbeidsflyt er en automatiseringsenhet fra start til slutt, inkludert definisjonen av hva som utløser automatiseringen, hvilket miljø eller andre aspekter som bør tas i betraktning under automatiseringen, og hva som skal skje som et resultat av utløseren.
>2. **Job:** En jobb er en del av arbeidsflyten, og består av ett eller flere trinn. I denne delen av arbeidsflyten definerer malen trinnene som utgjør byggejobben.
>3. **Step:** Et trinn representerer én effekt av automatiseringen. Et trinn kan defineres som en GitHub-handling, eller en annen enhet, som å skrive ut noe til konsollen.
>4. **Action:** En handling er en del av automatiseringen som er skrevet på en måte som er kompatibel med arbeidsflyter. Handlinger kan skrives av GitHub, av åpen kildekode-fellesskapet, eller du kan skrive dem selv!

## Opprett nytt repo

Først la oss opprette en dafult action og se hvordan vi kan koble den til en arbeidsflyt

### Aktivitet: Opprett standard arbeidsflyt

1. Åpne din GitHub konto i en nettleser.
2. Opprett et nytt repo. Valgfritt: Legg på beskyttelse av main branch.
3. Gå til Actions fanen.
4. Klikk på New workflow.
5. Søk etter "Simple workflow" and klikk konfigurer.
6. Gi din arbeidsflyt et navn som, ci.yml.
7. Klikk Commit changes..., og velg å opprette en ny branch og gi den navnet ci. Ikke velg merge til main direkte.
8. Klikk Propose changes.
9. Klikk Create pull request.
10. Vent noen sekunder og velg å laste inn siden på nytt.
11. Sjekk framdrift og status på jobben

### Filstruktur

Åpne koden i ditt repository. Kan du finne den fila du nettopp opprettet?

### Lokalt miljø

1. Klon repoet ditt til ditt lokale miljø
2. Åpne yaml fila i VS Code.

### 2: Redigere Bicept

Åpne ci.yml fila du lagde ovenfor og endre på bicep koden. Du kan for eksempel liste ut dato og antall filer i repo: 

```yaml
      - name: Run a simple script
        run: |
          echo "Running a simple script"
          echo "Current date and time: $(date)"
          echo "List of files in the repository:"
          ls -la
```

Sjekk inn koden og lag en PR til deg selv. Se at pipeline kjører når du ber om PR.

## Konfigurere GitHub actions til publisere til Azure

### Lag en web app

Vi starter i Azure Entra ID før vi går over til GitHub.

>Azure subsription
>
>Bruk din MSDN subscription hvis du har. Hvis du ikke har kan du opprette en test bruker med en epost konto du ikke har brukt tidligere.

## 4: Sett opp forbindelse mellom GitHub og Azure Portalen

Vi trenger å sette opp en tilgang for GitHub Actions til din subscription i Azure. Dette kan gjøres med federated identity mellom GitHub og Azure Entra ID med OpenID Connect.

Vi skal bruke Azure Login-action med Open ID Connect (OIDC). Først må vi konfigurere en federated identity credential på en Microsoft Entra applikasjon.

### Opprett app registration i Azure

Logg inn i Microsoft Entra Admin Center med din MSDN eller Azure Demo konto, ikke Bouvet.

1. Gå til [Entra ID admin](https://entra.microsoft.com/)

#### App registration

- I Microsoft Entra admin center velg **App registration** under **Applications**
- Velg **New registration**.
- Gi applikasjonen et brukervennlig navn som: GitHub-devops.
- Velg single tenant API aksess.
- Hopp over de valgfrie alternativene.
- Klikk **registrer**.

#### Federated credetials

Gå inn på applikasjonen du nettopp registrerte: (GitHub-devops)

- Velg «Certificates and secrets».
- Klikk på «Federated Credentials» fanen
- Klikk på «Add credentials»

På **Add a credential** siden

- Under Federated credential scenario velg:
  - **Github actions deploying Azure resources**
- **Organization**: Din GitHub organisasjon
- **Repository**: Ditt repo
- **Entity type**: Her velger du hvilken entitet som skal være en del av hemmeligheten som styrer tilgang i Azure. For GitHub kan du velge: Environment, Branch, Pull request eller Tag.
- Velg **Branch** og skriv **Azure-skolen**. (Denne verdien skal vi bruke senere i oppgaven.)

>Kopier innholdet i feltet Subject identifier. Vi skal bruke det i GitHub actions. Dette blir subject claim i JWT, det som står her må stemme 100 % med det du skriver i GitHub actions. (Mer om det senere, enn så lenge ta vare på denne. Du kan alltids komme tilbake her for å kopiere den senere.)

- Klikk **Create**

### Legg til hemmeligheter til Github repo

Naviger til GitHub og åpne ditt repo. Vi skal nå legge til informasjon om vår applikasjons innstillinger inn som hemmeligheter i Github for å kunne sette opp integrasjon mellom Github og Entra ID.

Vi trenger:

- Azure CLient ID: Denne finner du på siden til din applikasjons registrering i Entra ID. (Kalt Github-devops tidligere.)
- Tenant ID finner du på samme app reg side.
- Subscription ID finner du ved å gå inn i Azure portalen.

Vi skal legge til sikkrhet på repo nivå: Gå til:
**Settings > Secrets and variables > Actions > New repository secret**.

Leg til disse tre hemmelighetene, en om gangen:

- AZURE_CLIENT_ID: Din application (client) ID
- AZURE_TENANT_ID: Din directory (tenant) ID
- AZURE_SUBSCRIPTION_ID: Din subscription ID

Klikk **Add Secret** etter hver gang.

Merk at du kan ikke se den hemmelighetene du har lagt inn i ettertid, men du kan slette den og legge til en ny.

## 5 Test forbindelsen

For å teste forbindelsen vi nettopp har satt opp skal vi skrive en liten BICEP fil.

1. Opprett en nye yaml fil i **workflows** kanalogen. Kall den noe slikt som azure-login.yaml.

```yaml
name: Azure Login

on:
  push:
    branches:
      - main  # Triggers on push to the main branch
  workflow_dispatch:  # Allows manual triggering

permissions:
  id-token: write
  contents: read

jobs:
  login:
    runs-on: ubuntu-latest
    steps:
    - name: Checkout
      uses: actions/checkout@v2

    - name: Azure Login
      uses: azure/login@v1
      with:
        client-id: ${{ secrets.AZURE_CLIENT_ID }}
        tenant-id: ${{ secrets.AZURE_TENANT_ID }}
        subscription-id: ${{ secrets.AZURE_SUBSCRIPTION_ID }}

    - name: Test Azure Connection
      run: |
        az account show
```

Sjekk inn fila på en barnch med samme navn som du oppgave tidligere. Hvis du fulgte teksten i oppgaven skal barnchen hete:

>Azure-skolen


## Jobber her

### Tilordne vår GitHub devops app en rolle

Vi starter med å gi GitHub actions tilgang til Azure ressurser. For å få det til må appen tilordnes en rolle (RBAC) på et område i Azure. Vi kan tilordne GitHubDevops rollen contibutor på vår subscription. (Hvis vi allerede har er ressursgruppe vi ønsker å bruke kunne vi lagt rollen på denne i stedet for hele subscription.)








1. Logg på [Azure-portalen](https://portal.azure.com/)
2. Velg omfangsnivået. Hvis du ikke har en ressursgruppe, klikk deg inn på  subscription.
3. Velg **Access control**  (IAM).
4. Velg **Add**, og velg deretter **Add role assignment**.
5. I  fanen **Role** velger du rollen du ønsker å tilordne applikasjonen i listen.
6. Velg **Next**.
7. På  **Member**-fanen, for **Assign access to**, velger du **User, group, or service principal**.
8. Velg **Select members**. Som standard vises ikke Microsoft Entra-programmer i de tilgjengelige alternativene. For å finne søknaden din, søk etter den etter navn.
9. Klikk **Select** knappen, og deretter **Review + assign**.







For å kunne deploye må vi ha noe å deploye til. Lag en web App Service for test, og en for prod i [Azure](https://portal.azure.com).

Det er som oftest lurt å lage separate ressursgrupper for forskjellige miljøer. Når det gjelder app service plan trenger vi ikke noen kraftige greier. Det holder med en F1 pricing tier. Anbefaler at dere gir ressursgruppene, app service planene, og app servicene et navn som gjør det lett å få oversikt over hvilke Azure ressurser som hører til hvilket miljø. Dette gjør det lettere å identifisere miljøene når vi skal sette opp build pipelinen. Eksempelvis for app servicene:

- **Test**: [NavnPåApp]-test
- **Prod**: [NavnPåApp]-prod

>Manuell opprettelse av Azure ressurser
>
> Merk at vi her oppretter Azure resurser dirkete i portalen. Dette er for å forenkle øvingsoppgaven. Det man heller vil ønske å gjøre i et utviklingsprojsket er å benytte Github actions automatisering til også å opprette ressursene i Azure ved hjelp av Bicep kode.

Fremgangsmåte:

1. Gå til [Azure](https://portal.azure.com)
2. Lag en resource group for hvert av miljøene, Valgfritt (anbefalt: west europe, north europe eller norway east/west)
3. Lag en app service for hvert av miljøene, som du kopler opp mot hvert sin resource group og hver sin service plan.

    - **Name:** Dette navnet må være unikt i hele Azure, da den vil kunne nås fra &lt;appservicenavn&gt;.azurewebsites.net.
    - **Publish:** Code
    - **Runtime Stack:** .NET 8 (LTS)
    - **OS:** Linux eller Windows
    - **Region:** Benytt samme region som ressursgruppen.
    - **App service plan:** Endre sku and size til Free F1 under dev/test.

4. Klikk på review and create.

### 3: Opprett storage accounts i [Azure](https://portal.azure.com)

For at web applikasjonen skal fungere i begge miljøer, behøver vi en storage account for test, og en for prod.
Storage accountene opprettes i samme ressursgruppe for web applikasjonen.

>Fordelen med å ha alle ressurser i samme ressursgruppe er at det gjør det enklere å administrere ressursene som applikasjonen bruker. Man kan f.eks. slette alle ressurene som applikasjonen bruker samtidig ved å slette selve ressursgruppen.

1. Gå til [Azure](https://portal.azure.com)
2. Lag en storage account for hvert miljø, velg samme ressursgrupper som web applikasjonen.
   - **Storage account name** Navnet må være unikt i hele azure, lengde fra 3-24 tegn og kan kun innholde små bokstaver og tall. Postfix storage accounten med `test`/ `prod`. Eksempel `ws2knuteltest`
   - **Region** Velg samme region som ressursgruppen.
   - **Performance** Standard
   - **Redundancy** LRS (3 kopier eksisterer kun i samme datasenter.)

3. Klikk på Review og create.
4. Gå til Opprettet storage account, og navigerer inn i Containers.
5. Opprett en ny Container som du kaller for: imagecontainer med private access level.











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
>
>- Via build pipeline designeren
>- Via YAML script
>
Microsoft anbefaler å sette opp et YAML script som definerer build pipelinen din. Fordelen med å definere build pipelinen din i et script er at man kan sjekke det inn i kildekoden. Azure DevOps vil lese YAML fila og sette opp pipelinen din som et steg før selve applikasjonen din kjøres gjennom den. På den måten kan man ikke bare endre selve applikasjonen ved en commit, men pipelinen også. Det blir i tillegg mulig å rulle tilbake selve pipelinen din om en feil skulle oppstå.

Den andre måten å gjøre det på er gjennom designeren. Det negative med denne fremgangsmetoden er at definisjonen av pipelinen din ikke er lagret i kildekoden din, med alle implikasjoner det gir. Microsoft ønsker at man skal bruke YAML og har nå gjort det ca like lett å bruke yaml som designeren (med pek og klikk).

>Azure DevOps gjør det forholdsvis enkelt å traversere mellom å bruke designeren og YAML filer om det skulle ønskes. Så man kan starte med designeren for så å konvertere pipelinen YAML om man skulle ønske det.

I Azure DevOps, gå til "*Pipelines*" => "*Pipelines*" => "*Create pipeline*".

- Velg deretter "Azure Repos Git (YAML)".
- Velg så det repoet vi importerte til Azure DevOps prosjektet vårt i steg 1 (det er det som er valgt for oss som default om vi kun har et repo i prosjektet vårt).
- På neste steg kan vi velge å enten benytte en YAML fil som allerede finnes i repoet, starte med en minimalistisk YAML fil, eller starte fra en template.
- Velg templaten "ASP.NET CORE". Denne templaten gir oss alt vi trenger for å komme i gang. Må muligens klikke på Show more for å få opp alle templates.
- Trykk så på "Save and Run", DevOps vil så gi deg muligheten til å commite yaml filen til repoet.
- Trykk "Save and Run".

Nå har vi kommet til siden hvor vi kan se byggingen av koden. YAML-filen som devops gir når vi bygger .NET Core er ikke stor, den inneholder bare ett steg. Følg med på bygg jobben og se om noe går galt.

Om du har gjort alt riktig så burde build steget feile og det er meningen. .NET Core YAML-filen har build steg uten at prosjekt er oppgitt, i filen oppgir vi ikke hvilket prosjekt vi skal bruke og vi har to mapper i repoet så den finner ingen csproj eller sln filer. Dette skal vi nå gjøre noe med.

- Gå til Pipelines
- Velg Pipelinen du lagde
- Trykk Edit
- Slett alt under steps: (burde være to linjer, en med 'script:' og en med 'displayname:')
- Legg til Npm Task
- Velg `Install` som command. og Working Folder: `AzureWorkshop/AzureWorkshopApp`
- Legg til .Net Core task
- Velg publish under .Net Core tasken
- Huk vekk Publish web projects
- Under 'Path to project(s)' legger du inn 'AzureWorkshop/AzureWorkshop.sln'
- Under Arguments skal det stå --configuration $(buildConfiguration)
- Trykk Add
- Trykk Save og Save (commit to pipeline)
- Trykk Run

>Hvis du trykker på Job så kan du se på outputen til pipelinen din. Her burde alt gå grønt (eller ikke få farge) hvis alt går riktig, hvis noe går galt blir det rødt.

Se gjennom loggene og gjør deg litt kjent. Vi skal nemlig utvide yaml filen med deploy og da kan det være at du støter borti problemer og loggene kan da brukes til å finne ut hva som gikk galt.

Vi skal nå legge inn deploy av Web Appen til Azure i YAML-filen. Gå til edit av pipelinen og gjør følgende

- Legg til tasken 'Azure App Service deploy'
- Velg ConnectionType Azure resource manager og Azure Subscription velger du Service connection du opprett i steg 4.
- Sett App Service type til Windows/Linux (dette må matche det man valgte når man opprettet Web Appen)
- Velg applikasjonen du lagde
- Legg til displayName med et fornuftig navn på steget (f.eks. Deploy Web App to Azure)
- Trykk Add
- Trykk Save og Save (commit to pipeline)
- Trykk Run

Nå kan du se på jobben og hvis alt ble grønt så kan du gå til Deploy Web App to Azure tasken og åpne linken til applikasjonen du lastet opp til azure. Alternativt så kan du finne urlen via portalen. Se at nettsiden nå funker.
Hvis noe gikk galt i bygg eller deploy stegene så se gjennom loggene og prøv å fikse problemet før du ber om hjelp.

Vi har nå laget en basic build og deploy pipeline, men for å få en fullstendig pipeline så pleier man ha en stødigere struktur enn det vi har laget til nå hvor alt er samme job og stage. Vi skal nå endre litt på strukturen og her må man være forsiktig med whitespace for man må fort endre på det manuelt.

- Gå til edit av pipeline
- Endre på steps til stages
- Nå må du legge til et par linjer før - task (se eksempel under)
- Legg til stage for build og deploy før hver sine respektive tasker
- Legg til en publish build artifact task etter dotnet core publish tasken
- I arguments til dotnet core publish tasken legger du til  --output $(Build.ArtifactStagingDirectory) (ikke fjern --configuration )
- Legg til en download build artifact task før app service deploy tasken i den andre stagen
- Sett artifact name til drop
- Sett destination directory til $(System.DefaultWorkingDirectory) (denne må matche med inputen 'packageForLinux' i app service deploy tasken)
- Lagre og kjør pipelinen

Resultatet burde ligne på dette:

```yaml
stages:
  - stage: build
    jobs:
    - job: build
      steps: 
      - task: Npm@1
        inputs: 
          ***** 
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

- Gå til Environments under Pipelines
- Trykk Create environment
- Skriv inn Test som Name (dette blir test miljøet vårt)
- Legg til en beskrivelse som passer
- La none stå som resources
- Trykk Create
- Gå ut av Test environmentet
- Trykk på New environment
- Repeter stegene over for Produksjon

Vi skal nå legge til brukergodkjenning for deploy til prod. Det gjør vi ved å gå inn i environmentet Prod/Produksjon. Oppe i høyre hjørnet er det en knapp med dre dots på, trykk her og så på "Approvals and checks". Velg så Approvals og legg til deg selv som Approver (dette kan gjøres gjennom + tegnet eller ved å trykke på Approvals under "Add your first check"). Etter å ha trykket på create så skal "Approvals and checks" siden nå inneholde en entry med "All approvers must approve". Nå må vi bare koble dette miljøet sammen med deployProd stagen i YAML-filen. Gå tilbake til edit av pipeline og gjør følgende:

- Finn deployProd Stagen (det er her vi skal gjøre endringer)
- Under jobs: så skal vi endre fra "- job: deploy" til "- deployment: deploy"
- Legg til "environment: Prod " under "- deployment" (dette må matche environment navnet til produksjon)
- Steps blir nå gule (de er feil sted)
- Under environment legg til "strategy:"
- Under strategy (ett hakk inn) legges "runOnce:"
- Legg så "deploy:" ett hakk inn under "runOnce:"
- Tab så resten av YAML-filen slik at steps kommer ett hakk inn for "deploy:"
- Lagre og kjør pipeline

Eksempel

```yaml
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

- Legg til en ny .NET Core task før PublishBuildArtifact
- Sett command til test
- Under 'Path to Project(s)' legger du 'AzureWorkshop/AzureWorkshopAppTests/AzureWorkshopAppTests.csproj'
- Legg til displayName for tasken
- Endre stage og jobb navnet fra build til buildAndTest
- Lagre

Gå tilbake til pipelinen og se at testen nå stopper første stagen fra å kjøre gjennom. Fiks testen i prosjektet og last opp koden og se at det går gjennom. Nå har vi blandet build og test i samme stage og job, men det er ofte man gjør forskjellige tester basert på hvor langt i pipelinen man har kommet. Da lager man egne stages for f.eks. Unit test, integration test og smoke test og legger de inn som egne stages med DownloadArtifact og unzip task for så å kjøre gjennom, for integration tester vil man kanskje sette opp connections til andre tjenester i stagen eller kjøre mot test miljøet sitt.

>En artifact er produktet av en build pipeline i form av en zip fil. Denne filen inneholder alt som ble lagt i mappen man publisher artifacten fra. Zip filen vil beholde mappestruktur og alle filer som default. Artifakten lastes opp på en server og blir tilgjengelig for nedlasting og andre stager kan da bruke current build og vil resolve hvor den henter artifakten fra selv.

## 8: Endre trigger

For å forberede til Leksjon 2 så ønsker vi å legge inn en sjekk på triggeren i YAML-filen vår. Vi er kun interessert i endringer som ikke skjer i AzureWorkshopInfrastruktur. Alle endringer i den mappen hører til infrastruktur og trenger ikke å starte en bygg og deploy prosess. Dette er fordi vi skal gjøre en del endringer i neste leksjon og ønsker ikke å binde opp unødvendig byggetid. Åpne YAML-filen, enten lokalt eller gjennom Azure DevOps slik du har gjort i de tidligere oppgavene og erstatt

```yaml
trigger: 
- main
```

 med

```yaml
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
