# App Service

App Service er en av de mest brukte tjenesten i Azure, og 

Mere informasjon om App Services


## Opprette Web App (App Services)

1. Opprett en ressursgruppe (Resource Group). Du kan velge hvilken datasenter du vil det skal ligge i, men pga latency anbefales det
en av de Europeiske (Western eller Northern)
2. Opprett en App Service med navn <appservicenavn>. Når du skal velge App Service Plan oppretter du en i Free tier. Du bør også velge samme datasenter som du valgte i 1).
3. 


##  Deploy 

1. Åpne solution Lesson1/
2. Bygg prosjektet. 
3. Høyreklikk på prosjektet og velg "Publish". Logg så inn med 
4. Se at <>.azurewebsites.net
5. Legg merke til at IP-adressen nede ikke endres, selv om du trykker F5 flere ganger i nettleseren.

## Test av skalering 

1. Gå til din "App Service Plan"
2. Gå til til valget "Scale up", endre denne til en plan i "Standard tier". Man kan kun skalere opp med flere instanser ved å bruke Standard eller Premium tier. Disse koster mer enn Free/Shared/Basic tier.
3. Velg så "Scale out", og velg flere instanser enn 1 (f.eks. 3)
4. Trykk så gjentatte ganger på F5 i nettleseren og se at IP-adressen.
5. Sett så tilbake 1 instans og velg Free tier igjen, så slipper 

##

