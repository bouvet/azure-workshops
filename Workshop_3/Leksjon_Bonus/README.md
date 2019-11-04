# Sikre infrastruktur (PA)

- Pass på at container er private access. Dette finner du under spesifikk container sin Access level.
-- Finn Storage Accounten i portalen. Trykk containers, finn containeren du vil endre på og trykk på den. Ovenfor alle filene vil det være et par valg, trykk på change access level og velg Private.
- Gå tilbake til Storage Accounten
- På venstre side så skal det finnes et valg som heter Firewalls and Virtual Networks.
- Her inne legger du til et virituelt nettverk. Hvis du ikke har et fra før så kan du lage det gjennom "Add new virtual network" valget. Trykk her og følg oppsettet. Husk å trykke Save etter å ha laget VNet
- Test så at applikasjonen din ikke fungerer lengre. Hvorfor ikke? Du har jo sas token?
- Hvis du fulgte litt godt med så så du kanskje valget med Firewall og legg til din IP-addresse? Huk av for den og trykk lagre på nytt. Nå skal applikasjonen din fungere igjen.

- Gå til vnet og se litt rundt på valg.
- Her kan man parre med andre nettverk, legge til devices, opprette/endre subnets osv.
- Lag så en Network Security Group, som du så legger på VNettet du opprettet ovenfor.
- Dette kan gjøres ved å søke etter Network Security Group i søkefeltet eller via + tegnet på venstre side i menyen.
- Opprett så en Network Security Group, pass på at du oppretter i samme ressursgruppe og region som det virituelle nettverket du lagde. 
- Etter at NSG er opprettet, gå til ressursen.
- Gå til subnets (på venstre side i ressursbildet) og trykk Associate.
- Her velger du det virituelle nettverke du valgte og subnettet som Storage Accounten ligger på.
- Nå har du muligheten til å velge hvem som har tilgang til alle ressursene som ligger i det subnettet.
- F.eks. kan du gjøre det mulig å kontakte Storage Accounten fra andre tjenester som ligger i vnettet, f.eks. virituelle maskiner
- Ettersom det tar litt tid før endringene du gjør i NSGen faktisk blir aktive, så anbefales det at man venter så mye som 20 minutter før du prøver en annen regel.
- For en applikasjon sånn som dette, så ville det vært lurt å ikke bruke virituelt nettverk på Storage kontoen, men det finnes mange scenarioer der du ikke vil at dataene dine skal kunne komme på utsiden av dine applikasjoner og da er slike restriksjoner veldig fine å bruke. Da er det bare å sette opp VNET og NSG og så få på plass reglene man trenger for sin arkitektur.




----------------------------------------------- 
- Opprette VNET,
- Flytte App Service til VNET.
- Opprette Service Endpoint til Web App.
- Sikre at kun Web App har tilgang til Storage Account.
- Filene må vel streames ut i stedet for å tilby SAS-tokens.
-- Hvis Storage Account flyttes inn i vnet må det skrives om en del i service laget og i cshtml filen.
- NSG, policies.
- Public
- Gjøres i Portal, og ikke arm templates



