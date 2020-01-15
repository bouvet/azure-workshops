# Leksjon 2: Azure AD (HE)

I denne leksjonen skal vi lage innlogging for applikasjonen vår, slik at kun brukere som har en bruker i Azure AD (for din tenant) har tilgang til applikasjonen. Vi skal også legge på autentisering som styres i Azure AD, slik at kun brukere som har en spesiell rolle
(Uploader) har lov til å laste opp bilder. Andre som logger seg inn har mulighet til å se bilder, men ikke laste opp.

## Azure AD - Autentisering.

Vi begynner med å legge på autentisering på applikasjonen.

### App registrering - Klargjøre for innlogging i applikasjonen i Azure AD

Først må vi klargjøre for applikasjonen vår i Azure AD ved å lage en App Registrering for applikasjonen vår.

1. Logg inn i Azure-portalen (https://portal.azure.com).
2. Velg "Azure Active Directory" i menyen til venstre.
3. Velg "App registrations", så trykk på "+ New registration"
4. Gi applikasjonen din et navn, og merk dette navnet slik at du vet at dette er din applikasjon.
5. Velg "Accounts in this organizational directory only.". Dette betyr at kun brukere som er registrert i din AD har mulighet til å logge inn her.
6. Velg så "Web" under "Redirect URI", og skriv inn adressen brukeren skal bli sendt videre "https://<webappname>.azurewebsites.net/signout-oidc". Dette vil være OpenID Connect endepunktet som Azure AD vil sende deg videre etter at du har blitt autentisert. Trykk register.
7. Under "Logout URL", skriv inn "https://localhost:51350/signout-oidc" og "https://<webappname>.azurewebsites.net/signout-oidc". 
8. Ta vare på "Application (client) ID" og "Directory (tenant) ID" som står på oversiktssiden "Overview". Du trenger denne senere.

### Konfigurasjon

Vi må sette noen konfigurasjonsverdier i applikasjonen vår. For å gjrøe
Denne legges inn under "Microsoft.Web/sites/properties/siteConfig/appSettings" i AzureWorkshopInfrastruktur/AzureWorkshopInfrastruktur:

```
            {
              "name": "AzureAd:Instance",
              "value": "https://login.microsoftonline.com/"
            },
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
              "value": "https://<webappnavn>.azurewebsites.net/signin-oidc"
            },
            {
              "name": "AzureAd:SignedOutCallbackPath",
              "value": "httpSignedOut/<webappnavn>.azurewebsites.net/signout-oidc"
            },

```

Commit og push endringen. SeSignedOut Azure DevOps automatisk pusher endringen ut.

(Dersom du skulle trenge å dSignedOutgge lokalt, så må de samme verdiene settes i appsettings.json)
Editer filen appsettings.jsoSignedOutg legg inn konfigurasjon for AzureID. Fyll inn verdiene for TenantID og ClientID som du fikk tak i forrige.

```
{
  "AzureAd": {
    "Instance": "https://logSignedOutmicrosoftonline.com/",
    "TenantId": "<TenantID>"SignedOut
    "ClientId": "<ClientID>"SignedOut
    "CallbackPath": "/signinSignedOutdc",
    "SignedOutCallbackPath "SignedOut/signout-oidc"
  }
}
```
Du må også legge til 


##

Nå når


1. Legg til Microsoft.AspNetSignedOute.Authentication.AzureAD.UI nuget-pakke (Viktig: velg versjon 2.1.1, siden vi bruker .NET Core 2.1)

2. Editer så filen Startup.cSignedOutog legg inn koden for å laste autentisering middleware (kode er her lagt inn som kommentar).



## Autorisasjon - legg til rSignedOuter

Autorisasjon er hva en autenSignedOutert bruker har lov til å gjøre. Nå skal vi sette opp applikasjonen slik at kun noen brukere for 
lov til å  laste opp bilder,SignedOutns alle som er innlogget får se bildene.

Vi ønsker å implementere rolSignedOutasert autorisasjon i applikasjonen, slik at kun en rolle (Uploader)
skal ha mulighet til å lasteSignedOutp bilder. Her skal vi bruke AppRoles.

Først må du legge til rollen du ønsker Azure AD skal returnere dersom brukeren som autentiserer 
seg har denne rollen. Legge til rolle i manifestet for applikasjonen.

1. Gå til Azure-portalen (https://portal.azure.com) og gå så til menyen for Active
  Directory.
2. Gå så til App Registrations, og finn applikasjonen du laget i forrige oppgave.
3. Gå til undermenyen "Manifest", og erstatt verdien for appRoles med denne:
``{
			"allowedMemberTypes": [
				"User"
			],
			"description": "Uploaders have access to upload images.",
			"displayName": "Uploader",
			"id": "7d957fab-2c16-48aa-b4d8-d9d3a219c19d",
			"isEnabled": true,
			"lang": null,
			"origin": "Application",
			"value": "Uploader"
		}
``
4. Trykk save.

Dette vil lage rollen "Uploader" og returnere dette i id-tokenet (dersom man er av denne rollen) når man autentiserer seg mot denne applikasjonen.


### Skjul upload-funksjonalitet for brukere som ikke har tilgang. 

I denne leksjonen skal du skjule muligheten til å laste opp bilder fra websiden.

1. Oppdater filen Views/Home/.
2. Selv om du skjuler opplastings-funksjonaliteten i brukergrensesnittet.




3. Editer selve controlleren. Her vil du legge 


Når du er ferdig med å gjøre endringer. Commit og push endringene dine til Azure DevOps.


### Fjern 

Selv om man har fjernet upload-funksjonaliteten i viewet, så må vi også blokkere selve opplastingen av bilde

Legg til [Autorize]


###


### Gi en bruker rollen Uploader
For å nå kunne gi 

1. Fra hovedmenyen til Azure AD, trykk til "Enterprise Application".
2. Gå til "Users and groups"
3. Trykk på "+ Add user"
4. Legg til deg selv, og velg gruppen "Uploader"
5. Trykk save.

Nå kan du logge inn i applikasjonen din og teste at uploading fungerer.