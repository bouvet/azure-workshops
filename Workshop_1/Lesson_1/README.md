# Leksjon 1: Intro

I denne leksjonen skal du:

* Estimere kostnaden for å kjøre applikasjonen per måned. 
* Opprette en Web App med tilhørende App Service Plan
* Deploye første versjon av applikaskjonen.
* Teste ut skalering av applikasjonen.


## Kostnader i Azure

Bruk priskalkulatoren for Azure til å kalkulere systemet du har tenkt å utvikle i denne Workshoppen. 

Priskalkulatoren finner du her: 

Systemet består av:
* Key Vault - for enkelthets skyld regner at vi her med at et treff på websiden din er en operasjon.
* Storage (Lagring) - Block Blob Storage, LRS, Varm/hot
* App Service * 2 instanser, velg en VM i Standard tier

Lek gjerne med tallene og endre størrelsen på App Service, antallet instanser og se hvordan dette endrer kostnadsbildet. Gjør tilsvarende for Storage og Key Vault.

## App Service

App Service er en av de mest brukte tjenesten i Azure, og er en paraply-betegnelse for flere tjenester som bruker samme underliggende infrastruktur.

Mere informasjon om App Services finnes her: https://azure.microsoft.com/en-us/services/app-service/

## Opprette Web App (App Services)

1. Opprett en ressursgruppe (Resource Group). Du kan velge hvilken datasenter du vil det skal ligge i, men pga latency anbefales det en av de europeiske (Western eller Northern)
2. Opprett en App Service med navn <appservicenavn>. Dette navnet må være unikt i hele Azure, da den vil kunne nås fra <appservicenavn>.azurewebsites.net. Når du skal velge App Service Plan (også kalt Hosting Plan) oppretter du en i Free tier. Du bør også velge samme datasenter som du valgte i 1).
3. 


##  Deploy 

1. Åpne solution som ligger i mappen Start.
2. Bygg prosjektet. 
3. Høyreklikk på prosjektet og velg "Publish". Logg så inn med 
4. Se at <appservicenavn>.azurewebsites.net
5. Legg merke til at IP-adressen oppe til høyre ikke endres, selv om du trykker F5 flere ganger i nettleseren.

## Test av skalering 

1. Gå til din "App Service Plan"
2. Gå til til valget "Scale up", endre denne til en plan i "Standard tier". Man kan kun skalere opp med flere instanser ved å bruke Standard eller Premium tier. Disse koster mer enn Free/Shared/Basic tier.
3. Velg så "Scale out", og velg flere instanser enn 1 (f.eks. 3)
4. Trykk så gjentatte ganger på F5 i nettleseren og se at IP-adressen endrer seg, i og med at man har flere webservere som svarer på requesten.
5. Sett så tilbake 1 instans og velg Free tier igjen.



