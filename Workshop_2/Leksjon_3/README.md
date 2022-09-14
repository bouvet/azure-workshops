# Monitorering og telemetri
Vi har nå lagt til en Application Insights instans i ARM templaten. I forrige leksjon sørget vi for at ApplicationInsightsKey ble lagt inn som appSettings i web applikasjonen. Dette kan du verifisere i portalen ved å gå til app service --> configuration --> application settings. 
​
## Legg til Application Insights
For Visual Studio
1. Åpne `AzureWorkshopApp.sln`
2. Åpne Package Manager Console og skriv `Install-Package Microsoft.ApplicationInsights.AspNetCore -Version 2.21.0` (det kan hende at du må bruke GUI til å legge til NuGet-Pakken `Microsoft.ApplicationInsights.AspNetCore`)
​

For Visual Studio Code
1. Åpne Terminal vindu
2. Naviger til riktig mappe (Start/AzureWorkshop/AzureWorkshopApp)
3. Kjør kommandoen `dotnet add .\AzureWorkshopApp.csproj package --version 2.21.0 Microsoft.ApplicationInsights.AspNetCore`

Åpne filen `Startup.cs` og endre ConfigureServices til
<font size="4">

```C#
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();
    services.AddOptions();
    services.AddApplicationInsightsTelemetry(); // Legg til denne
    services.AddScoped<IStorageService, StorageService>();
    services.Configure<AzureStorageConfig>(Configuration.GetSection("AzureStorageConfig"));
}
      
```
</font>
​
Åpne `_ViewImports.cshtml` og legg til følgende injection:

```html
    @inject Microsoft.ApplicationInsights.AspNetCore.JavaScriptSnippet JavaScriptSnippet
```
​
Åpne deretter `_Layout.cshtml` og legg til følgende helt i toppen av filen 
```html
    @using Microsoft.ApplicationInsights.AspNetCore
```
og følgende nederst i `<head>`
```html
    @Html.Raw(JavaScriptSnippet.FullScript)
```
​
Commit (skrive en bra commit-melding, f.eks. `add ApplicationInsights telemetry`) og push slik at den nye koden blir deployet. 
​
Når release er ferdig, naviger til azure-websiten og generer litt trafikk. Bonuspoeng for alle som klarer å lage en exception 
​
​
## Se på telemetri
- Gå til portal.azure.com
- Gå til ApplicationInsights-instansen
- Landingssiden aka "Overview" viser grafer med _failed requests, server response time, server requests_ med mer
- Dersom du lykkes med å generere en feil i forrige steg bør det være mulig å se denne under "Investigate -> Failures"
​
### Log Analytics
- I Application Insights ressursen i Azure portalen, gå til menyen `Monitoring` og velg `Logs`. 
- Kryss ut popup hvis den dukker opp
- Du vil nå ha mulighet til å skrive inn queries mot ApplicationInsights-loggene (på venstre side er den oversikt over tabeller)
​
Prøv å hent ut exceptions ved å skrive inn følgende og trykke _Run_:
Det skal ikke være noen her helt ennå.
```html
    exceptions | search "error"
```
Se https://docs.microsoft.com/en-us/azure/azure-monitor/log-query/get-started-portal for mer info om hvordan LogAnalytics kan brukes
​
## Lage egne events
Ved å lage våre egne events kan vi følge handlingene brukerne våre tar. Det gjør oss i stand til å kunne bekrefte eller avkrefte hypotesene vi har laget oss om hvordan en endring vil påvirke bruken av systemet. Dette er verdifull feedback som vi kan bruke til å forbedre utviklingsprosessen vår.
​
Først må vi konfigurere `ImagesController.cs` til å lage egendefinert telemetri
- Åpne filen `ImagesController.cs`
- Endre konstruktør og lokale variabler til følgende:
<font size="4">

```c#
        private readonly IStorageService _storageService;
        private readonly TelemetryClient _telemetryClient;
​
        public ImagesController(IStorageService storageService, TelemetryClient telemetryClient)
        {
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
            _telemetryClient = telemetryClient;
        }
```
</font>

- Gå til metoden "Upload" og endre `foreach`-løkken til gjøre et TrackEvent-kall med `_telemetryClient`:
<font size="4">

```c#
         foreach (var formFile in files)
            {
                if (FileFormatHelper.IsImage(formFile))
                {
                    if (formFile.Length > 0)
                        using (var stream = formFile.OpenReadStream())
                        {
                            if (await _storageService.UploadFileToStorage(stream, formFile.FileName))
                            {
                                _telemetryClient.TrackEvent("UPLOADED_FILE", new Dictionary<string, string>
                                    {
                                        { "FILE_NAME", formFile.FileName},
                                        { "CONTENT_LENGTH", formFile.Length.ToString()}
                                    });
                                return new AcceptedResult();
                            }
                        }
                }
                else
                {
                    return new UnsupportedMediaTypeResult();
                }
            }
```
</font>

- Sjekk inn endringene dine, og vent til pipelinene har kjørt.
- Last opp et bilde
- Gå til portal.azure.com og ApplicationInsights
- Velg _Usage -> Events_ for å se eventen som ble laget
- Gå til `Logs` og kjør query på _customEvents_ for å se all info om eventen
```html
customEvents | where name  == "UPLOADED_FILE"
```
- Trykk "Chart" for å se resultatet som en graf
- Denne grafen kan vises på dashboardet i azure-portalen ved å trykke "Pin"