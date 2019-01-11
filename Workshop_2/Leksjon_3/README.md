# Monitorering og telemetri
- Legg til en application insights instans i ARM-template, merk at også at ApplicationInsightsKey må legges inn som en setting på WEB-appen.
- Deploy infrastructure
- Sjekk, og se at komponenten er opprettet.


- Genenerer litt trafikk mot websiden.(Eks. Powershell som automatisk genererer en del trafikk, også noen feil f.eks. )
- Se at ting, og finn exceptions etc.
- Se på application map.

Custom Events.
- Bruk TelemetryClient til å logge en custom-event, f.eks. størrelsen på filen som ble lastet opp.

Log Analytics queries:
- Gå inn i log analytics queries, gjør seg litt kjent med det.
- Finn frem exceptions etc.
- Lag en custom query i Analytics for å hente ut custom event du laget med TelemeteryClient.

- Lage et nytt dashboard. Pin noen standard-metrikker fra fast.
- Gå inn i Analytics igjen, bruk query du laget tidligere (ligger i history), lag en chart utav det, og pin den til dashboardet ditt.
