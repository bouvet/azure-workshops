# Leksjon 2: Infrastructure as Code (ARM-templates)
​
Infrastructure as Code (IaC) er at man oppretter og vedlikeholder IT-infrastruktur i form av maskinlesbare definisjons-filer, i stedet for manuell oppsett eller at man bruker programmer for å provisjonere infratrukturen sin. ARM-templates er Microsofts løsning for IaC i Azure. Det aller fleste tjenester i Azure kan provisjoneres via ARM-templates. I denne leksjonen skal du lage build- og release-pipelines for infrastrukturen din. I tillegg skal du gjøre endringer på infrastrukturen ved å editere ARM-templates og så redeploye disse.
​
## Klargjør parameter-filer for test-miljøet
​
Parameter-filen i ARM-templates (kalt *azuredeploy.{environment}.parameters.json* i dette prosjektet), gjør det mulig å spinne opp et sett med Azure ressurser med ulik konfigurasjon til forskjellige miljøer. For hvert miljø kan du legge inn parameters, eksempel hvis du vil oppskalere produksjonsmiljøet i forhold til test. Det er også vanlig å definere navn på ressurser her. 
​
1. Gjenbruk ressursgruppen du brukte for test-miljøet. Slett Web App'en og App Service Plan du opprettet i den gruppen. Merk deg navnet på Web App'en din, da du skal gjenbruke denne i steg 3. 
2. Kopier `azuredeploy.parameters.json` til `azuredeploy.test.parameters.json`. Dette vil være parameter-filen din for test.
3. I `azuredeploy.test.parameters.json`, sett inn 3 unike navn i *value* feltene. Dette bestemmer navnet ressursene dine får - bruk samme navn på App service som før. Gi komponentene navn slik at man kan se hvilket miljø de tilhører.
4. Valider templaten din ved å gjøre en test-deploy ved å høyre-klikke på prosjektet og velg Deploy. 
a. Velg riktige filer og rett ressursgruppe i Azure. 
b. Publish. (Dersom du får spørsmål om navn på ressursgruppe., kan det være noen problemer med Visual Studio-versjonen din, og da må du fylle ut dette manuelt). Hvis du ikke har visual studio eller av en annen grunn ikke kan publishe direkte kan du gå rett på å sette opp build-pipeline.
5. Hvis du får feil i steget over er sannsynligvis ett av ressursnavnene dine ulovlig.   
6. Undersøk om ressursene dine ble opprettet i ressursgruppen din. Hvis ikke, gjør steg 4. på nytt eller spør om hjelp. 
7. Slett ressursene som ble opprettet i ressurgruppen.
8. Lag en commit og push den til ditt DevOps-repo.
​
## Build-pipeline 
Lag en ny byggedefinisjon i Azure DevOps. Den vil være veldig kort, fordi det eneste den trenger å gjøre er å tilgjengeliggjøre ARM-templaten din til neste steg i kjeden (release-pipeline).
​
1. Lag en ny build-definisjon (Trykk på Pipelines->Pipelines->"Azure Repos Git (YAML)").
2. Velg git-repoet du laget i forrige sted.
3. Velg "ASP. NET Core" helt øverst, og gi YAML-filen et passende navn (f.eks. azure-pipelines-infra.yml).
4. Fjern alt under "steps:" (to linjer).
5. Legg til "Publish Build Artifacts
6. I "Path to publish" endres $(Build.ArtifactStagingDirectory) til $(System.DefaultWorkingDirectory)
7. Lagre og commit til git

​
Når du har fått build-pipelinen din til å kjøre, sjekk at bygge-artefakten din inneholder ARM-templaten. Dette gjøres enten gjennom loggen på jobben eller siden som viser jobben som ble kjørt (under "Related" skal det stå "1 published"). Vi har nå publisert hele mappe strukturen, vi kunne selvfølgelig ha bare publisert infrastruktur mappen også ved å endre på "Path to publish".
​
## Release-pipeline
​
For å deploye infrastrukturen trenger å opprette en Release-pipeline som tar et bygg og deployer det ut til et miljø. I denne øvelsen skal du kun deploye infrastrukturen for test-miljøet. Vi lager en Release pipeline utenom YAML-filen for å vise en annen måte å ha release på. Dette kunne selvfølgelig også blitt gjort gjennom YAML.
​
1. Lag en egen release-pipeline for infrastruktur (Pipelines->Releases->New).
2. Her velger du "Empty job" helt øverst, og ikke noen ferdig template.
3. Gi steget "Stage 1" et nytt navn, og kall det f.eks. "Test". Dette vil være navnet på miljøet ditt.
4. Gi release-pipelinen din et navn, f.eks. "Infrastruktur" (på toppen av siden).
5. Velg artefakt til venstre fra build-pipelinen du lagde i forrige oppgave, trykk "Add an artifact". Velg byggepipelinen du opprettet i forrige steg. Her kan du også velge om du vil spesifisere hvilket bygg du vil bruke for hver release, eller om den skal bruke siste bygget hver gang.
6. Trykk så på "Pre-deployment conditions" for Test-miljøet ditt. Her kan du velge om du ønsker at det skal bli deployet automatisk når du oppretter en release, eller om du må gjøre dette manuelt. Dette er liten oval runding til venstre for Test-boksen med et lyn og en person på.
7. Åpne så Tasks->"Navnet på din stage (f.eks. Test)" (menyen øverst står på pipeline, bytt til Tasks) og legg til tasken "ARM Template Deployment". 
8. Velge Azure subscription, ressursgruppen du har laget og sett location til samme sted som ressursgruppen.
9. Åpne opp "Template" settings
10. I template feltet, finn stien til `azuredeploy.json`.
11. I template parameters feltet, finn stien til `azuredeploy.test.parameters.json`
12. Trykk så på "Save" og "Ok".
13. Trykk så på "Create Release" og lag en release
14. Gå inn i releasen
15. Klikk på deploy 
​
Verifiser at ressursene dine blir deployet ut til ressursgruppen din.
​
### Redeploy av miljø
For å teste at ting faktisk blir opprettet på konsistent vis, skal du nå slette og redeploye.
​
1. Slett en ressurs i ressursgruppen din (f.eks. Storage Accounten).
2. Kjør Redploy av releasen og se at det blir opprettet igjen (gå inn på selve releasen og trykk på miljøet, og velg Redeploy)
​
### Endring av miljø
Du ønsker å gjøre det mulig å sette størrelsen på App Service Planen til forskjellige størrelse basert på om det er et test-miljø eller et produksjonsmiljø. For å gjøre dette må du legge inn et nytt steg i release pipelinen. Dette kan du gjøre ved å legge til en ny stage. Trykk "+ Add" og "New Stage" og velg "Empty Job" øverst. Repeter så det du gjorde for Test stagen for Prod (endre navn, gå til tasks->prod og legg til "Arm Template Deployment" og velg riktige filer, subscription, resource group og location). Hvis du ikke har en template fil for prod. Så kan du lage en ny ved å kopiere `azuredeploy.test.parameters.json` og kalle den `azuredeploy.prod.parameters.json`, bytt ut navn på 'webSiteName' og 'storageAccountName' og commit og push endringene dine. Gå tilbake til å editere på release-pipelinen og legg til riktige verdier i "Arm Template Deployment" tasken for Prod stagen (bygg må bli ferdig før du kan velge `azuredeploy.prod.parameters.json` som parameter fil). Nå har du to stages i release pipelinen og disse kan nå ha forskjellige verdier via `azuredeploy.test.parameters.json` og `azuredeploy.prod.parameters.json`. 
​
1. Editer så `azuredeploy.parameters.test.json` 
1. Legg inn parameter for `appServiceSku`. 
1. Sett denne verdien til en f.eks. F1, slik at den overskriver parameteren D1 som er satt som default verdi.
2. Sjekk så inn koden, lag et nytt bygg og kjør en release og deploy (hvis du ikke har satt på automatisk deploy).
​
Se så at SKU har oppdatert seg på App Service Planen din via *portal.azure.com*.
​
## Legge til ny ressurs
​
I neste leksjon skal du bruke Application Insights for å overvåke løsning. For å gjøre dette må du legge til selve ressursen i miljøet ditt.
​
1. Editer `azuredeploy.json`. Se https://docs.microsoft.com/en-us/azure/templates/microsoft.insights/2015-05-01/components for strukturen på denne komponenten. Legg også til en parameter til scriptet for navnet på komponenten. Vårt forslag er: 
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
2. Fortsett i `azuredeploy.json`: her må du også legge til noen appSettings på website-ressursen din (i tillegg til de som allerede finnes), som peker til Application Insights-ressursen din.
```
          "appSettings": [
            ...eksisterende innstillinger her
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
​
4. I `azuredeploy.json` må vi sørge for at det ny parameteret appInsightsName blir en del av templaten. Legg til objektet under nederst i parameters-seksjonen
​
```
    ,"appInsightsName": {
      "type": "string",
      "metadata": {
        "description": "The app insights name."
      }
    }
```
​
5. Legg til parameteret under i `azuredeploy.parameters.test.json` for å sette navnet på Application Insights-instansen din.
```
    "appInsightsName": {
      "value":  "my-great-insights" 
    } 
```
​
6. Sjekk inn endringene dine, og vent til at bygget ditt har gått igjennom.
7. Lag en ny release og valider at komponenten blir opprettet. 
​
Du er nå klar for å begi deg ut på neste leksjon.
