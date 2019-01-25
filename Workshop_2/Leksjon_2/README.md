# Leksjon 2: Infrastructure as Code (ARM-templates)




I denne leksjonen skal du lage build- og release-pipelines for infrastrukturen i prosjektet ditt. 

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
2. Lag så en egen release-pipeline for infrastruktur ()
3. Her velger du "Empty job", og ikke noen ferdig template.
4. Velg artifact til venstre fra den build-pipelinen du laget i forrige oppgave, trykk Add.
4. Gi release-pipelinen din et navn, f.eks. "Infrastruktur" (på toppen av siden).
5. Gi steget "Stage 1" et nytt navn, og kall det "Test". I 
5. Åpne "Test"-steget (trykk på "Agent job") og legg til tasken "Azure Resource Group Deployment"
6. Konfigurer så dette steget ved å velge Azure subscription, velg ressursgruppen du laget i steg 1) og sett location til samme sted som ressursgruppen.
7. Under "Linked artifacts", velg template og parameter fil som ble publisert fra byggestedet ditt.
4. Trykk så på "Save"
5. Trykk så på "+Release" eller "Create a release"

### Redeploy

1. Slett hele ressursgruppen din.
2. Kjør Redploy av releasen og se at det blir opprettet igjen.

### Endring av miljø
Du ønsker å gjøre det mulig å sette størrelsen på App Service Plan'en til forskjellige størrelse basert på om det er et test-miljø eller
et produksjonsmiljø. For å gjøre dette må du legge inn et 

1. Editer azuredeploy.json. Legg til en parameter til scriptet for å sette SKU. Referer så til denne parameteren lenger i nede i scriptet der SKU blir satt på App Service Plan.
2. Editer så azuredeploy.parameters.json og sett. Sett så 



## Legge til ny ressurs

I neste leksjon skal du bruke Application Insights for å overvåke løsning. For å gjøre dette må du legge til selve ressursen i miljøet ditt.

1. Editer azuredeploy.json. Se https://docs.microsoft.com/en-us/azure/templates/microsoft.insights/2015-05-01/components for strukturen på denne komponenten. Legg også til en parameter til scriptet for navnet på komponenten.
2. Editer azuredeploy.parameters.json, og legg til parameter som setter navnet på Application Insights-instansen din.
3. Sjekk inn endringene dine, og vent til at 
4. Lag en ny release og valider at komponenten blir opprettet. 


- Lag en ny parameter-fil for et ny miljø, velg en større app service plan (som Production plane, f.eks. S1, som støtter slots, velg også antall instanser til 2) 
- Lage et nytt miljø i Azure DevOps, med å klone





