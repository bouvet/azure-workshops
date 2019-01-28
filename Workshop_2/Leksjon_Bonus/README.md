# Bonus leksjon
Her en noen ekstra-oppgaver dersom du skulle bli raskt ferdig.

## Flere miljøer med infrastruktur

Sett opp release-pipelines for qa og produksjon i infrastruktur-prosjektet. 

## Slot-deployment i produksjon

Slots er en feature i Web Apps som gjør at du kan deploye til en annen slot for å teste den nye versjonen din, for så å swappe denne inn når du har fått testet denne.

Se om du kan få til å sette opp dette i produksjon. Da må du både oppdatere ARM-templates slik at en slot oppdateres, og i tillegg oppdatere deployment-steget av koden i produksjon til å deploye til en slot i stedet for hoved-websiten.
