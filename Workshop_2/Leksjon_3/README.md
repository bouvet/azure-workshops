# Monitorering og telemetri
- Legg til en application insights instans i ARM-template, merk at også at ApplicationInsightsKey må legges inn som en setting på WEB-appen.
- Deploy infrastructure
- Sjekk, og se at komponenten er opprettet.

## Legge til Application Insights i Visual Studio
1. Åpne solution i Visual Studio
2. Åpne Package Manager Console og skriv `Install-Package Microsoft.ApplicationInsights.AspNetCore` (evt bruk GUI til å legge til NuGet-Pakken `Microsoft.ApplicationInsights.AspNetCore`)

Åpne filen `Program.cs` og modifiser koden slik at det nå står
```c#
    public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .UseApplicationInsights()
            .Build();
```
og endre `main`-metoden til
```c#
  public static void Main(string[] args)
  {
       CreateWebHostBuilder(args).Run();
  }
```

Åpne `_ViewImports.cshtml` og legg til følgende injection:
```
    @inject Microsoft.ApplicationInsights.AspNetCore.JavaScriptSnippet JavaScriptSnippet
```

Åpne deretter `_Layout.cshtml` og legg til følgende nederst i `<head>`
```html
    @Html.Raw(JavaScriptSnippet.FullScript)
```

Commit (skrive en bra commit-melding, f.eks `add ApplicationInsights telemetry`) og push slik at den nye koden blir deployet. 

Når release er ferdig, naviger til azure-websiten og generer litt trafikk. Bonuspoeng for alle som klarer å lage en exception 

_hint: last opp en `.txt-fil`, burde kræsje_



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
