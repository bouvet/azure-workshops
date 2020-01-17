# Leksjon 2: Azure AD

I denne leksjonen skal vi lage innlogging for applikasjonen vår, slik at kun brukere som har en bruker i Azure AD (for din tenant) har tilgang til applikasjonen. Vi skal også legge på autentisering som styres i Azure AD, slik at kun brukere som har en spesiell rolle
(Uploader) har lov til å laste opp bilder. Andre som logger seg inn har mulighet til å se bilder,
men ikke laste opp.

## Azure AD - Autentisering.

Vi begynner med å legge på autentisering på applikasjonen.

### App registrering - Klargjøre for innlogging i applikasjonen i Azure AD

Først må vi klargjøre for applikasjonen vår i Azure AD ved å lage en App Registrering for applikasjonen vår.

1. Logg inn i Azure-portalen (https://portal.azure.com).
2. Velg "Azure Active Directory" i menyen til venstre.
3. Velg "App registrations", så trykk på "+ New registration"
4. Gi applikasjonen din et navn, og merk dette navnet slik at du vet at dette er din applikasjon.
5. Velg "Accounts in this organizational directory only.". Dette betyr at kun brukere som er registrert i din AD har mulighet til å logge inn her.
6. Velg så "Web" under "Redirect URI", og skriv inn adressen brukeren skal bli sendt videre "https://\<webappname>.azurewebsites.net/signin-oidc". Dette vil være OpenID Connect endepunktet som Azure AD vil sende deg videre etter at du har blitt autentisert. 
7. Trykk register.
8. På venstre side trykker du på Authentication og under Advanced settings "Logout URL", legg inn "https://\<webappname>.azurewebsites.net/signout-oidc".
9. Du må også krysse av for "ID token" under authentication.
10. Trykk Save
11. Ta vare på "Application (client) ID" og "Directory (tenant) ID" som står på oversiktssiden "Overview". Du trenger denne senere.

(Dersom du ønsker å debugge lokalt, må du også legge inn "https://localhost:51350/signin-oidc" som "Redirect URI "og "https://localhost:51350/signout-oidc" som "Logout URL".
Ideelt bør man opprette en egen App Registration for lokal debugging, men for dette test-formålet, og for å spare tid gjør vi ikke det her)

### Konfigurasjon

Vi må sette noen konfigurasjonsverdier i applikasjonen vår.
Åpne filen AzureWorkshopInfrastruktur/AzureWorkshopInfrastruktur/azuredeploy.json. Søk etter "Microsoft.Web/sites" i dette objectet under "properties/siteConfig/appSettings" legger du til:

```
            {
              "name": "AzureAd:Instance",
              "value": "https://login.microsoftonline.com/"
            },
            {
              "name": "AzureAd:TenantId",
              "value": "<Directory (tenant) ID>"
            },
            {
              "name": "AzureAd:ClientId",
              "value": "<Application (client) ID>"
            },
            {
              "name": "AzureAd:CallbackPath",
              "value": "/signin-oidc"
            },
            {
              "name": "AzureAd:SignedOutCallbackPath",
              "value": "/signout-oidc"
            }

```

Commit og push endringen. Se at Azure DevOps automatisk pusher endringen ut.

(Dersom du skulle trenge å logge deg på lokalt, så må de samme verdiene settes i appsettings.json
Editer filen AzureWorkshop/AzureWorkshopApp/appsettings.json legg inn konfigurasjon for AzureID. Fyll inn verdiene for TenantID og ClientID som du fikk tak i forrige oppgave.)

### Legg til autentisering

Nå når er det på tide å legge til funksjonaliteten til AzureWorkshop prosjektet. 

Først må du legge til Microsoft.AspNet.Authentication.AzureAD.UI nuget-pakke (Viktig: velg versjon 2.1.1, siden vi bruker .NET Core 2.1) som har funksjonalitet for autentisering mot Azure AD.

I denne workshoppen har vi valgt å legge inn kodeendringer som kommentarer som må kommenteres inn/ut for å få den funksjonaliten. Alle endringer har TODO: foran, slik at man lett
kan finne dem. Alle filstier som må endres: 

1. Startup.cs: Legg inn lasting av middleware for autentisering og cookies.
2. Views/Shared/\_Layout.cshtml: Legg inn inkludering av et partial view som har login- og logout-grensesnitt.
3. Controllers/HomeController.cs: Legg til Authorize-attributt som krever at man må være logget inn, og videresender til Azure AD for autentisering hvis ikke.
4. Controllers/ImageController.cs: Legg til Authorize-attributt på controlleren for å kreve innlogging også her.

Du kan nå teste innlogging, samt opplasting av bilder.

## Autorisasjon - legg til roller

Autorisasjon er hva en autentisert bruker har lov til å gjøre. Nå skal vi sette opp applikasjonen slik at kun noen brukere for
lov til å laste opp bilder, mens alle som er innlogget får se bildene.

Vi ønsker å implementere rollebasert autorisasjon i applikasjonen, slik at kun en rolle (Uploader)
skal ha mulighet til å laste opp bilder. Her skal vi bruke AppRoles, som er en innebygged funksjon i Azure AD.

Først må du legge til rollen du ønsker Azure AD skal returnere dersom brukeren som autentiserer
seg har denne rollen. Legge til rolle i manifestet for applikasjonen.

1. Gå til Azure-portalen (https://portal.azure.com) og gå så til menyen for Active
   Directory.
2. Gå så til App Registrations, og finn applikasjonen du laget i forrige oppgave.
3. Gå til undermenyen "Manifest", og erstatt verdien for appRoles ([])med denne:
   `[{ "allowedMemberTypes": [ "User" ], "description": "Uploaders have access to upload images.", "displayName": "Uploader", "id": "7d957fab-2c16-48aa-b4d8-d9d3a219c19d", "isEnabled": true, "lang": null, "origin": "Application", "value": "Uploader" }]`
4. Trykk save.

Dette vil lage rollen "Uploader" og returnere dette i id-tokenet (dersom man er av denne rollen) når man autentiserer seg mot denne applikasjonen.

### Kun Uploader skal ha mulighet til å laste opp bilder

Nå ønsker du gjøre slik at det kun er brukere som har rollen Uploader mulighet til å laste opp bilder.

1. Views/Index.cshtml: Gjør slik at upload-boksen skjules for brukere som ikke er Uploader.
2. Controllers/ImageController.cs: Du må også sperre selve metoden Upload(), slik at kun brukere som er Uploader har lov til å laste opp.

Når du er ferdig med å gjøre endringer. Nå kan du teste applikasjonen, og du skal ikke ha mulighet til å laste opp bilder.

### Tillegg rolle til bruker

For å nå kunne gi brukeren din rollen Uploader.

1. Fra hovedmenyen til Azure AD, trykk til "Enterprise Application".
2. Finn applikasjonen du lagde (bruk søkefeltet)
3. Gå til "Users and groups"
4. Trykk på "+ Add user"
5. Legg til deg selv, og velg gruppen "Uploader"
6. Trykk save.

Når du nå logger inn, så skal du igjen ha mulighet til å laste inn bilder.
