# Leksjon 1

I denne leksjonen skal du:

* Estimere kostnaden for å kjøre applikasjonen per måned. 
* Opprette en Web App med tilhørende App Service Plan
* Deploye første versjon av applikasjonen.
* Teste ut skalering av applikasjonen.


## Kostnader i Azure

Bruk priskalkulatoren for Azure til å kalkulere systemet du har tenkt å utvikle i denne Workshoppen. 

Priskalkulatoren finner du her: https://azure.microsoft.com/en-us/pricing/calculator/

Systemet består av:
* Key Vault - for enkelthets skyld regner at vi her med at et treff på websiden din er en operasjon.
* Storage (Lagring) - Block Blob Storage, LRS, Varm/hot tier
* App Service * 2 instanser, velg en App Service Plan i Standard tier

Lek gjerne med tallene og endre størrelsen på App Service, antallet instanser og se hvordan dette endrer kostnadsbildet. Gjør tilsvarende for Storage og Key Vault.

## App Service

App Service er en av de mest brukte tjenestene i Azure, og er en paraply-betegnelse for flere tjenester som bruker samme underliggende infrastruktur.

Mere informasjon om App Services finnes her: https://azure.microsoft.com/en-us/services/app-service/

## Opprette Web App (App Services)

For å opprette ressurser med portalen i Azure, så går du til https://portal.azure.com og logger inn med din Microsoft konto. Opprett ressurser ved å trykke "Create a resource" i venstre hjørne.

1. Opprett en ressursgruppe (Resource Group). Du kan velge hvilken datasenter du vil det skal ligge i, men pga latency anbefales det en av de europeiske. (Western eller Northern). Test gjerne med Azure Speed Test for å kunne ta en avgjørelse: http://www.azurespeed.com/
2. Opprett en Web App med navn <appservicenavn>. Dette navnet må være unikt i hele Azure, da den vil kunne nås fra <appservicenavn>.azurewebsites.net. Velg samme ressursgruppe som du valgte i 1). Når du skal velge App Service Plan kan du opprette en i Free tier (merket F under Dev/Test fanen). Du bør også velge samme datasenter som du valgte i 1).
 

##  Deploy 

I denne øvelsen skal du deploye start-versjonen av web-applikasjonen. Dette er en "fungerende" applikasjonen, men har ikke funksjonaliteten for å laste opp bilder.

1. Åpne solution som ligger i mappen Workshop_1/Start.
2. Bygg prosjektet. 
3. Høyreklikk på prosjektet og velg "Publish". Logg så inn med 
4. Se at <appservicenavn>.azurewebsites.net
5. Legg merke til at IP-adressen oppe til høyre ikke endres, selv om du trykker F5 flere ganger i nettleseren.

## Test av skalering 

I denne øvelsen skal du teste å skalere opp (kraftigere App Service Plan/"VM") og skalere ut (flere instanser av App Service Plan/"VM") Web-applikasjonen din for å kunne takle mer last, samt ha redundanse med flere servere.

1. Gå til din "App Service Plan" som du laget i forrige oppgave.
2. Gå til til valget "Scale up", endre denne til en plan i "Standard tier". Man kan kun skalere opp med flere instanser ved å bruke Standard eller Premium tier. Disse koster mer enn Free/Shared/Basic tier.
3. Trykk så gjentatte ganger på F5 i nettleseren og se at IP-adressen har endret seg siden forrige oppgave. Dette fordi applikasjonen er flyttet til en større App Service Plan og fått en ny IP-adresse.
4. Velg så "Scale out", og velg flere instanser enn 1 (f.eks. 3). Vent så til den er ferdig å skalere ut (visuelle indikatorer på toppen av siden).
5. Trykk så gjentatte ganger på F5 i nettleseren og se om IP-adressen endrer seg. Grunnen til at denne ikke endrer seg er fordi ARR Affinity er slått på, og dette gjør at requests fra samme nettleser blir tildelt samme server. Test gjerne med en annen type browser (hvis du har installert), og se om denne får en annen IP-adresse.
6. I menyen for Web App'en din. Gå inn på Application Settings, og skru ARR Affinity til Off.
7. Trykk så gjentatte ganger på F5 for å se om IP-adressen nå endrer seg mellom hver gang.
6. Sett så tilbake 1 instans og velg Free tier igjen.



