# Leksjon 2: Azure AD (HE)

I denne leksjonen skal vi lage innlogging for applikasjonen vår, slik at kun brukere som har en bruker i Azure AD (for din tenant) har tilgang til applikasjonen. Vi skal også legge på 
rollebasert autentisering som styres i Azure AD, slik at kun brukere som har en spesiell rolle
(Uploader) har lov til å laste opp bilder. Andre som logger seg inn har mulighet til å se bilder, men ikke laste opp.

## Azure AD - Autentisering.

Vi begynner med å legge på autentisering på applikasjonen.

### App registrering - Klargjøre for innlogging i applikasjonen i Azure AD

Først må vi klargjøre for applikasjonen vår i Azure AD ved å lage en App Registrering for applikasjonen vår.

1. Logg inn i Azure-portalen (https://portal.azure.com).
2. Velg "Azure Active Directory" i menyen til venstre.
3. Velg "App registrations", så trykk på "+ New registration"
4. Gi applikasjonen din et navn, og merk dette navnet slik at du vet at denne er for lokal debugging (eksempel: <navn>-dev)
5. Velg "Accounts in this organizational directory only.". Dette betyr at kun brukere som er registrert i din AD har mulighet til å logge inn her.
6. Velg så "Web" under "Redirect URI", og skriv inn adressen brukeren skal bli sendt videre "https://localhost:51350/". Dette vil være
   Web-applikasjonen din når du kjører lokal utvikling. Trykk register.
7. Gå så til Authentication, og legg til "https://localhost:51350/signin-oidc" som et ekstra redirect URL. Trykk "Save"
8. Under "Logout URL", skriv inn "https://localhost:51350/signout-oidc"
9. Ta vare på "Application (client) ID" og "Directory (tenant) ID" som står på oversiktssiden "Overview". Du trenger denne senere.

Gjenta prosessen over en gang til, men gi applikasjonen et nytt navn med navnet test, og endre url i  6) til "https://<webappname>.azurewebsites.net". Dette vil være URL'en i Azure som du blir redirected til etter at du har autentisert deg i Azure AD. `webappname` vil her være navnet på applikasjonen din fra Leksjon 1.

### Konfigurasjon: 
For å kunne 

1. Legg til Microsoft.AspNetCore.Authentication.AzureAD.UI nuget-pakke (Viktig: velg versjon 2.1.1, siden vi bruker .NET Core 2.1)
2. Editer filen appsettings.json og legg inn konfigurasjon for AzureID. Fyll inn verdiene for TenantID og ClientID som du fikk tak i forrige.
```
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",

    // Azure AD Audience among:
    // - the tenant Id as a GUID obtained from the azure portal to sign-in users in your organization
    // - "organizations" to sign-in users in any work or school accounts
    // - "common" to sign-in users with any work and school account or Microsoft personal account
    // - "consumers" to sign-in users with Microsoft personal account only
    "TenantId": "<TenantID>",

    // Client Id (Application ID) obtained from the Azure portal
    "ClientId": "<ClientID>",
    "CallbackPath": "/signin-oidc",
    "SignedOutCallbackPath ": "/signout-oidc"
  }
}
```
3. Editer så filen Startup.cs, og legg inn koden for å laste autentisering middleware (kode er her lagt inn som kommentar.)


### Klargjør

Kode for authentisering. Legg

For å fi


Legg inn kode for autentisering.
Konfigurer applikasjon.
Test at innlogging fungerer.
Lage redirect som hetner kun for en bruker? 

## Autorisasjon

Autorisasjon er hva en autentisert bruker har lov til å gjøre. Nå skal vi sette opp applikasjonen slik at kun noen brukere for 
lov til å  laste opp bilder, mensalle som er innlogget får se bildene. 

Vi ønsker å implementere rollebasert autorisasjon i applikasjonen, slik at kun en rolle (Uploader)
skal ha mulighet til å 

Først må du legge til rollen du ønsker Azure AD skal returnere dersom brukeren som autentiserer 
seg med.
Legge til rolle i manifestet for applikasjonen.

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

Dette vil lage rollen "Uploader" og returnere dette i tokenet når man autentiserer seg mot denne applikasjonen.

Så skal du oppdatere applikasjonen til kun å tillatte brukere som har rollen "Uploader"


### Skjul upload-funksjonalitet for brukere som ikke har tilgang. 

I denne leksjonen skal du 

2. Oppdater filen Views/Home
2. Selv om du skjuler opplastings-funksjonaliteten i brukergrensesnittet, er det viktig at man også krever 



### Gi en bruker rollen Uploader

Gå så til App Registrationen


### Fjern 

Selv om man har fjernet upload-funksjonaliteten i viewet, så må vi også blokkere selve 







Microsoft Identity Platform 2.0 https://docs.microsoft.com/en-us/azure/active-directory/develop/
består av:
OAuth 2.0 and OpenID Connect standard-compliant authentication service
- Jobb eller skole-kontoer.
- Personlige-kontoer

## Sikre bilder (bør flyttes til Leksjon 1)

Nå når du har laget innlogging i applikasjonen, så er det viktig at poeng at . Til nå har bildene kun ligget åpen container i Storage Accounts, og 



Legge inn slik at SAS-token.
Et SAS-token (Shared Access Signature) er en begrenset rettighet til.

Merk; dersom 



Skru public til private.
Se om RBAC og innlogget identity kan brukes.
Sikre at ingen eksterne får tilgang til bildene.
Bildene lagres per bruker (GUID)




- Identity, SSO (pres)
- Generell (pres)
- RBAC (pres)

Satt opp innlogging på applikasjonen.


https://docs.microsoft.com/en-us/azure/active-directory/develop/identity-platform-integration-checklist


OAuth 2.0 og OpenID Connect

https://login.microsoftonline.com
