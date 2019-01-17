# Leksjon 2: Infrastructure as Code (ARM-templates)

I denne leksjonen skal du lage build- og release-pipelines for infrastrukturen i prosjektet ditt.

## Bygge-pipeline 
Lag en ny byggedefinisjon i Azure DevOps

1. Lag en by build-definisjon
2. Velg "Empty template"
2. Definisjonen skal kun inneholde et steg "Publish build artifacts".
3. Velg så dette steget, og velg så stien til katalogen hvor  "Workshop_2/Komplett/AzureWorkshopInfrastruktur/AzureWorkshopInfrastruktur"
3. Trykk så på "Save and Queue" for å se at den kjører.

Nå har du laget 

## Release-pipeline


1. Lag en ressursgruppe som du ønsker å deploye løsningen din til. 
2. Lag så en egen release-pipeline for infrastruktur.
3. Her velger du "Empty job", og ikke noen ferdig template.
4. Velg artifact til venstre fra den build-pipelinen du laget i forrige oppgave, trykk Add.
4. Gi release-pipelinen din et navn, f.eks. "Infrastruktur" (på toppen av siden).
5. Gi steget "Stage 1" et nytt navn, og kall det "Test".
5. Åpne "Test"-steget (trykk på "Agent job") og legg til tasken "Azure Resource Group Deployment"
6. Konfigurer så dette steget ved å velge Azure subscription, og velg ressursgruppen du laget i sted 1) og
sett location til West eller North Europe.
7. Under "Linked artifacts", velg template og parameter fil som ble publisert fra byggestedet ditt.
4. Trykk så på "Save"
5. Trykk så på "+Release" eller "Create a release


## Legge til 
I neste leksjon skal du bruke Application Insights. For å gjøre dette må du legge til 
ARM-templates 

- Deploy til test-miljø for å teste.
- Legg til en komponent man trenger, deploy på nytt, og se at dette. 

- Lag en ny parameter-fil for et ny miljø, velg en større app service plan (som Production plane, f.eks. S1, som støtter slots, velg også antall instanser til 2) 
- Lage et nytt miljø i Azure DevOps, med å klone





