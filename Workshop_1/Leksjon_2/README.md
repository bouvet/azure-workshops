# Leksjon 2: App Services

I denne leksjonen skal du:

* Opprette en Web App med tilhørende App Service Plan.
* Deploye første versjon av applikasjonen.
* Teste ut skalering av applikasjonen.

## App Service

App Service er en av de mest brukte tjenestene i Azure, og er en paraply-betegnelse for flere tjenester som bruker samme underliggende infrastruktur.

Mere informasjon om App Services finnes her: https://azure.microsoft.com/en-us/services/app-service/

## Opprette Web App (App Services)

For å opprette ressurser med portalen i Azure, så går du til https://portal.azure.com og logger inn med din Microsoft konto. Opprett ressurser ved å trykke "Create a resource" i venstre hjørne.

1. Opprett en ressursgruppe (Resource Group). Du kan velge hvilken datasenter du vil det skal ligge i, men pga. nett-forsinkelse anbefales det en av de europeiske  (Western Europe eller Northern Europe). Test gjerne med Azure Speed Test for å kunne ta en avgjørelse: http://www.azurespeed.com/
2. Opprett en Web App med navn &lt;appservicenavn&gt; (her velger du selv). Dette navnet må være unikt i hele Azure, da den vil kunne nås fra &lt;appservicenavn&gt;.azurewebsites.net. Velg samme ressursgruppe som du opprettet i 1). Når du skal velge App Service Plan kan du opprette en i Free tier (merket F under Dev/Test fanen), men dersom du har Trial-abonnement så må du velge en App Service Plan som koster penger (f.eks. i B-serien). Du bør også velge samme datasenter som du valgte i 1).
 

##  Deploy 

I denne øvelsen skal du deploye start-versjonen av web-applikasjonen. Dette er en "fungerende" applikasjonen, men har ikke funksjonaliteten for å laste opp bilder.

1. Åpne solution som ligger i mappen Workshop_1/Start.
2. Bygg prosjektet. 
3. Høyreklikk på prosjektet og velg "Publish". Logg så inn med Microsoft-kontoen som er tilknyttet Azure-abonnementet ditt.
4. Sett opp publishing profilen slik at den deployer til den web-applikasjonen du satte opp tidligere. Deploy.
5. Se at <appservicenavn>.azurewebsites.net serverer applikasjonen.
6. Legg merke til at IP-adressen oppe til høyre ikke endres, selv om du trykker F5 flere ganger i nettleseren.

## Test av skalering 

I denne øvelsen skal du teste å skalere opp (kraftigere App Service Plan/"VM") og skalere ut (flere instanser av App Service Plan/"VM") Web-applikasjonen din for å kunne takle mer last, samt ha redundans med flere servere.

1. Gå til <appservicenavn>.azurewebsites.net og merk deg IP-adressen i høyre hjørne.
2. Gå til din "App Service Plan" i portalen som du lagde i forrige oppgave.
3. Gå til til valget "Scale up", endre denne til en plan i "Standard tier". Man kan kun skalere opp med flere instanser ved å bruke Standard eller Premium tier. Disse koster mer enn Free/Shared/Basic tier.
4. Dersom du refresher applikasjonen, samtidig som du ser på IP-adressen i høyre hjørne, så vil du se at den endrer seg én gang. Dette fordi applikasjonen er flyttet til en større App Service Plan/"VM" og fått en ny IP-adresse.
4. Velg så "Scale out", og velg flere instanser enn 1 (f.eks. 3). Vent så til den er ferdig å skalere ut (visuelle indikatorer på toppen av siden).
5. Selv om du trykker gjentatte ganger på F5 i nettleseren på websiden din, så vil du se at IP-adressen ikke endrer seg, selv om vi nå har flere servere som vi potensielt kan få svar fra. Grunnen til at denne ikke endrer seg er fordi ARR Affinity er slått på, og dette gjør at requests fra samme nettleser blir tildelt samme server. Test gjerne med en annen type browser (hvis du har installert), og se om denne får en annen IP-adresse.
6. I menyen for Web App'en din, gå inn på Application Settings og skru ARR Affinity til Off, slik at vi ikke lenger kun får svar fra en server. Trykk så gjentatte ganger på F5 for å se om IP-adressen nå endrer seg mellom hver gang.
7. Sett så tilbake 1 instans og velg Free tier igjen.
