# Leksjon 2: Infrastructure as Code (ARM-templates)

Infrastructure as Code (IaC) er at man oppretter og vedlikeholder IT-infrastrukt i form av maskinlesbare definisjons-filer, i stedet
for manuell oppsett eller at man bruker programmer for å provisjonere infratrukturen sin. 

ARM-templates er Microsofts løsning for IoC i Azure. Det aller fleste tjenester kan provisjoneres via ARM.

I denne leksjonen skal du lage build- og release-pipelines for infrastrukturen din. I tillegg skal du gjøre endringer på infrastrukturen ved å editere ARM-templates og så redeploye disse.

## Bygge-pipeline 
Lag en ny byggedefinisjon i Azure DevOps. Den vil være veldig kort, fordi det eneste den trenger å gjøre er å tilgjengeliggjøre ARM-templaten din til neste steg i kjeden (release-pipeline).

1. Lag en by build-definisjon (Trykk på Pipelines->Builds->New).
2. Velg git-repoet du laget i forrige sted
3. Velg "Empty job", og gi definisjonen et passende navn. Resten kan stå slik de står.
4. Definisjonen skal kun inneholde et steg/task "Publish build artifacts". 
5. Legg inn et filter som gjør at bygget trigges bare når endringer under AzureWorkshopInfracture 
6. Velg så dette steget, og velg så stien til katalogen hvor din ARM-template finnes("Workshop_2/Start/AzureWorkshopInfrastruktur/AzureWorkshopInfrastruktur")
7. Trykk så på "Save and Queue" for å se at den kjører.

Når du har fått build-pipelinen din til å kjøre, sjekk at bygge-artefakten din inneholder ARM-templaten.

## Release-pipeline

For å deploye infrastrukturen trenger å opprette en Release-pipeline som tar et bygg og deployer det ut til et miljø.

1. Lag en ressursgruppe i portalen som du ønsker å deploye løsningen din til. 
2. Lag så en egen release-pipeline for infrastruktur (Pipelines->Releases-New)
3. Her velger du "Empty job", og ikke noen ferdig template.
4. Gi steget "Stage 1" et nytt navn, og kall det f.eks. "Test". Dette vil være navnet på miljøet ditt.
4. Gi release-pipelinen din et navn, f.eks. "Infrastruktur" (på toppen av siden).
5. Velg artifakt til venstre fra den build-pipelinen du laget i forrige oppgave, trykk "Add an artifact". Velg her den byggepipelinen du opprettet i forrige steg. Her kan du også velge om du vil spesifisere hvilken bygg du vil bruke for hver release, eller om den skal foreslå den siste bygg. Trykk så Add.
6. Trykk så på "Pre-deployment conditions" for Test-miljøet ditt. Her kan du velge om du ønsker at det skal bli deployet automatisk når du oppretter en release, eller om du må gjøre dette manuelt.
7. Åpne så Stage-Task (ved å trykk på linken i miljøet) og legg til tasken "Azure Resource Group Deployment". 
8. På Action lar du det bare stå på "Create or update resource group". Konfigurer så dette steget ved å velge Azure subscription, velg ressursgruppen du laget i steg 1) og sett location til samme sted som ressursgruppen.
9. Under "Template", velg template og parameter fil som ble publisert fra byggestedet ditt.
10. Trykk så på "Save" og "Ok".
11. Trykk så på "+Release". Dersom du har valgt manuell deploy, så må du starte denne.


### Redeploy av miljø

For å teste at ting faktisk blir opprettet på en konsistent, skal du nå slette og redeploye.

1. Slett en ressurs i ressursgruppen din (f.eks.Storage Accounten).
2. Kjør Redploy av releasen og se at det blir opprettet igjen.



### Endring av miljø
Du ønsker å gjøre det mulig å sette størrelsen på App Service Plan'en til forskjellige størrelse basert på om det er et test-miljø eller
et produksjonsmiljø. For å gjøre dette må du legge inn et 

1. Editer azuredeploy.json. Legg til en parameter til scriptet for å sette SKU. Referer så til denne parameteren lenger i nede i scriptet der SKU (Stock Keeping Unit, bare en unik kode for det produktet du velger) blir satt på App Service Plan.
2. Editer så azuredeploy.parameters.json og legg inn parameteren du nettopp laget. Sett denne verdien til en f.eks. D1.
3. Check så inn koden, lag et nytt bygg og kjør en release.

Se så at SKU har oppdatert seg på App Service Planen din.

## Legge til ny ressurs

I neste leksjon skal du bruke Application Insights for å overvåke løsning. For å gjøre dette må du legge til selve ressursen i miljøet ditt.

1. Editer azuredeploy.json. Se https://docs.microsoft.com/en-us/azure/templates/microsoft.insights/2015-05-01/components for strukturen på denne komponenten. Legg også til en parameter til scriptet for navnet på komponenten.
2. Editer azuredeploy.parameters.json, og legg til parameter som setter navnet på Application Insights-instansen din.
3. Sjekk inn endringene dine, og vent til at 
4. Lag en ny release og valider at komponenten blir opprettet. 

Du er nå klar for å begi deg ut på neste leksjon.
