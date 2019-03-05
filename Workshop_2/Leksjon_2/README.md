# Leksjon 2: Infrastructure as Code (ARM-templates)

Infrastructure as Code (IaC) er at man oppretter og vedlikeholder IT-infrastruktur i form av maskinlesbare definisjons-filer, i stedet
for manuell oppsett eller at man bruker programmer for å provisjonere infratrukturen sin. 

ARM-templates er Microsofts løsning for IaC i Azure. Det aller fleste tjenester i Azure kan provisjoneres via ARM-templates.

I denne leksjonen skal du lage build- og release-pipelines for infrastrukturen din. I tillegg skal du gjøre endringer på infrastrukturen ved å editere ARM-templates og så redeploye disse.

## Klargjør parameter-filer for test-miljøet

Parameter-filen i ARM-templates er en fil du bruker for å legge inn ting som er forskjellig fra f.eks. miljø til miljø. 

1. Gjenbruk ressursgruppen du brukte for test-miljøet. Slett Web App'en og App Service Plan du opprettet i den gruppen. Merk deg navnet på Web App'en din, da du skal gjenbruke denne i steg 3. 
2. Kopier azuredeploy.parameters.json til azuredeploy.test.parameters.json. Dette vil være parameter-filen din for test.
3. Editer så den nye filen ved å sette inn dine unike navn på de forskjellige tjenestene, og gi Web App det samme. Gi komponentene navn slik at man kan se på de hvilket miljø de tilhører.
4. Valider templaten din ved å gjøre en test-deploy ved å høyre-klikke på prosjektet og velg Deploy. Velg riktige filer og rett ressursgruppe i Azure. Publish. (Dersom du får spørsmål om navn på ressursgruppe., kan det være noen problemer med Visual Studio-versjonen din, og da må du fylle ut dette manuelt).
5. Sjekk inn og push oppdateringen din i git.
6. Slett ressursene som du opprettet i ressurgruppen. Disse skal bli lagt inn av Azure DevOps.

## Bygge-pipeline 
Lag en ny byggedefinisjon i Azure DevOps. Den vil være veldig kort, fordi det eneste den trenger å gjøre er å tilgjengeliggjøre ARM-templaten din til neste steg i kjeden (release-pipeline).

1. Lag en ny build-definisjon (Trykk på Pipelines->Builds->+New->New Build pileline).
2. Velg git-repoet du laget i forrige sted.
3. Velg "Empty job", og gi definisjonen et passende navn. Resten kan stå slik de står.
4. Trykk så på +-tegnet ved "Agent job 1". Definisjonen skal kun inneholde et steg/task "Publish Build Artifacts" (ikke forveksle med "Publish Pipeline Artifact").
5. Velg så dette steget, og velg så stien til katalogen hvor din ARM-template finnes
("Workshop_2/Komplett/AzureWorkshopInfrastruktur/AzureWorkshopInfrastruktur")
5. Dersom du ønsker, kan du sette opp continuous ingeration(CI) slik at bygg blir trigget automatisk når man sjekker inn ny kode i git. Du ønsker da å legge inn et filter som gjør at bygget bare trigges når endringer under AzureWorkshopInfracture. Dette gjøres under fanen Triggers, hvor du velger "Enable continuous integration", og setter opp path-filters til å peke til katalogen med dine ARM-templates. Katalogen som angis må være hele stien, og ha vanlige slasher: Eks. "/AzureWorkshopInfrastruktur/AzureWorkshopInfrastruktur". Hvis du ikke setter opp CI, så må du manuelt trigge bygg når du skal bygge.
7. Trykk så på "Save and Queue" for å se at den kjører.

Når du har fått build-pipelinen din til å kjøre, sjekk at bygge-artefakten din inneholder ARM-templaten. Dette gjøres ved å gå inn på selve bygget, og så trykk på "Artifacts" øverst i høyre hjørne.

## Release-pipeline

For å deploye infrastrukturen trenger å opprette en Release-pipeline som tar et bygg og deployer det ut til et miljø. I denne øvelsen skal du kun deploye infrastrukturen for test-miljøet.

1. Lag en egen release-pipeline for infrastruktur (Pipelines->Releases->New).
2. Her velger du "Empty job", og ikke noen ferdig template.
3. Gi steget "Stage 1" et nytt navn, og kall det f.eks. "Test". Dette vil være navnet på miljøet ditt.
4. Gi release-pipelinen din et navn, f.eks. "Infrastruktur" (på toppen av siden).
5. Velg artefakt til venstre fra den build-pipelinen du laget i forrige oppgave, trykk "Add an artifact". Velg her den byggepipelinen du opprettet i forrige steg. Her kan du også velge om du vil spesifisere hvilken bygg du vil bruke for hver release, eller om den skal foreslå den siste bygg. Trykk så Add.
6. Trykk så på "Pre-deployment conditions" for Test-miljøet ditt. Her kan du velge om du ønsker at det skal bli deployet automatisk når du oppretter en release, eller om du må gjøre dette manuelt.
7. Åpne så Stage-Task (ved å trykk på linken i miljøet) og legg til tasken "Azure Resource Group Deployment". 
8. På Action lar du det bare stå på "Create or update resource group". Konfigurer så dette steget ved å velge Azure subscription, velg ressursgruppen du har laget og sett location til samme sted som ressursgruppen.
9. Under "Template", velg template og parameter fil som ble publisert fra byggestedet ditt.
10. Trykk så på "Save" og "Ok".
11. Trykk så på "+Release". Dersom du har valgt manuell deploy, så må du starte denne.

Se nå at releasen blir deployet ut til test-miljøet ditt.

### Redeploy av miljø
For å teste at ting faktisk blir opprettet på en konsistent, skal du nå slette og redeploye.

1. Slett en ressurs i ressursgruppen din (f.eks. Storage Accounten).
2. Kjør Redploy av releasen og se at det blir opprettet igjen (gå inn på selve releasen og trykk på miljøet, og velg Redeploy)

### Endring av miljø
Du ønsker å gjøre det mulig å sette størrelsen på App Service Plan'en til forskjellige størrelse basert på om det er et test-miljø eller
et produksjonsmiljø. For å gjøre dette må du legge inn et 

1. Editer så azuredeploy.parameters.test.json og legg inn parameter for _appServiceSku_. Sett denne verdien til en f.eks. D1, slik at den overskriver parameteren F1 som er satt som default verdi.
2. Sjekk så inn koden, lag et nytt bygg og kjør en release.

Se så at SKU har oppdatert seg på App Service Planen din.

## Legge til ny ressurs

I neste leksjon skal du bruke Application Insights for å overvåke løsning. For å gjøre dette må du legge til selve ressursen i miljøet ditt.

1. Editer azuredeploy.json. Se https://docs.microsoft.com/en-us/azure/templates/microsoft.insights/2015-05-01/components for strukturen på denne komponenten. Legg også til en parameter til scriptet for navnet på komponenten. Vår forslag er: 
```
    {
      "name": "[parameters('appInsightsName')]",
      "type": "Microsoft.Insights/components",
      "apiVersion": "2015-05-01",
      "location": "[parameters('location')]",
      "tags": {},
      "kind": "string",
      "properties": {
        "Application_Type": "web",
        "Flow_Type": "Bluefield",
        "Request_Source": "rest"
      }
    }
```
2. Du må også legge til noen appSettings på website-ressursen din (i tillegg til de som allerede finnes), som peker til Application Insights-ressursen din.
```
          "appSettings": [
            {
              "name": "APPINSIGHTS_INSTRUMENTATIONKEY",
              "value": "[reference(resourceId('Microsoft.Insights/components', parameters('appInsightsName')), '2014-04-01').InstrumentationKey]"
            },
            {
              "name": "APPINSIGHTS_PORTALINFO",
              "value": "ASP.NETCORE"
            },
            {
              "name": "APPINSIGHTS_PROFILERFEATURE_VERSION",
              "value": "1.0.0"
            },
            {
              "name": "APPINSIGHTS_SNAPSHOTFEATURE_VERSION",
              "value": "1.0.0"
            },
```
3. Du må også legge inn en avhengighet mellom Web App-ressursen din og Application Insights-ressursen. Dette fordi at Application Insights må opprettes først, siden Web App har en referanse til denne. 
Vår dependsOn på Web App ser slik ut:
```
      "dependsOn": [
        "[resourceId('Microsoft.Web/serverfarms', variables('servicePlanName'))]",
        "[resourceId('Microsoft.Insights/components', parameters('appInsightsName'))]"
      ]
```
4. Editer azuredeploy.parameters.test.json, og legg til parameter som setter navnet på Application Insights-instansen din.
5. Sjekk inn endringene dine, og vent til at bygget ditt har gått igjennom.
6. Lag en ny release og valider at komponenten blir opprettet. 

Du er nå klar for å begi deg ut på neste leksjon.
