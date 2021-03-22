# Azure Security Center

Nå skal du se om du får noen anbefalinger fra Security Center på App Servicen din.

- Logg inn i Azure-portalen (https://portal.azure.com).
- Gå til `webappnavn` som du opprettet i leksjon 1.
- Trykk så på `Security`, og du kommer til et skjermbilde som viser alerts.
- Se om du har noen sårbarheter som bør utbedres. Her vil du mest sannsynlig se en melding om at det anbefales å skru på https slik at det kun går over sikker kanal.

Når du nå ser at den gir, så kan det være fristende å trykke `Remediate`, som vil fikse problemet med en gang. Men, i og med at vi
praktiserer "Infrastructure as Code", så må vi gjøre endringen permanent i scriptene være. Trykk på `View remediation logic` for å se hva du må legge til ARM-templaten din:

- I Infrastruktur-prosjektet ditt, åpne azuredeploy.json.
- Editer filen, slik at dette er i samsvar med hva 'View remediation logic' viste, hvis det er ingen forslag så trenger du ikke gjøre noe.
- Redeploy Infrastruktur-prosjektet.

Det tar gjerne noen minutter fra du gjør en endring, til at endringen vises i Azure Security Center. Men, du kan sjekke at applikasjonen
kun tillatter https uansett.

(Dersom du har tid til overs, kan du gjøre samme øvelse for Storage Account'en din, og se om du får noen anbefalinger der).

## Azure Defender for Storage

Azure Defender for Storage er en tilleggstjeneste på storage account, slik at storage accounten din blir overvåket for angrep og unormal oppførsel. Dersom Security Center oppdager noe unormalt som den mener du bør se på, så vil du motta en epost med varsling om hva som har
skjedd.

Gå til din Storage Account som ble opprettet, og trykk på `Security`. Siden dette koster ekstra penger, og vi har vurdert til at dette ikke er noe som er nødvendig velger vi å ikke skru på denne i dette tilfelle.

Du kan lese mer om dette ved en senere anledning her:
[Azure Defender for Storage](https://docs.microsoft.com/en-us/azure/storage/common/storage-advanced-threat-protection?tabs=azure-portal)
