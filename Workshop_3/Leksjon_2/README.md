# Leksjon 2: Azure AD (HE)

## Azure AD - Autentisering.

I denne leksjonen skal vi lage innlogging for applikasjonen vår, slik at kun brukere som har en bruker i Azure AD (for din tenant) har tilgang
til applikasjonen. Vi skal

Denne applikasjonen vil kun tillate 

### App registrering - Klargjøre for innlogging i applikasjonen i Azure AD

1. Logg inn i Azure-portalen (https://portal.azure.com).
2. Velg "Azure Active Directory" i menyen til venstre.
3. Velg "App registrations", så trykk på "+ New registration"
4. Gi applikasjonen din et navn, og merk dette navnet slik at du vet at denne er for lokal debugging (eksempel: <navn>-dev)
5. Velg "Accounts in this organizational directory only." 
6. Velg så "Web" under "Redirect URI", og skriv inn adressen brukeren skal bli sendt videre "https://localhost:51350/". Dette vil være
   Web-applikasjonen din når du kjører lokal utvikling. Trykk register.
7. Gå så til Authentication, og legg til "https://localhost:51350/signin-oidc" som et ekstra redirect URL. Trykk "Save"
8. Under "Logout URL", skriv inn "https://localhost:51350/signout-oidc"
9. Ta vare på "Application (client) ID" og "Directory (tenant) ID" som står på oversiktssiden "Overview". Du trenger denne senere.

Gjenta prosessen over en gang til, men gi applikasjonen et nytt navn med navnet produksjon, og endre url i  6) til "https://<dinapplikasjon>.azurewebsites.net". Dette vil være URL'en i Azure som du blir redirected

- Identity, SSO (pres)
- Generell (pres)
- RBAC (pres)

Satt opp innlogging på applikasjonen.


https://docs.microsoft.com/en-us/azure/active-directory/develop/identity-platform-integration-checklist


OAuth 2.0 og OpenID Connect

https://login.microsoftonline.com

### Klargjør

Kode for authentisering. Legg
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",

    // Azure AD Audience among:
    // - the tenant Id as a GUID obtained from the azure portal to sign-in users in your organization
    // - "organizations" to sign-in users in any work or school accounts
    // - "common" to sign-in users with any work and school account or Microsoft personal account
    // - "consumers" to sign-in users with Microsoft personal account only
    "TenantId": "bd4f7f1a-5006-4dd1-b922-b440e1561dd6",

    // Client Id (Application ID) obtained from the Azure portal
    "ClientId": "bdb44b85-3815-497b-bcdc-c7305dad6b55",
    "CallbackPath": "/signin-oidc",
    "SignedOutCallbackPath ": "/signout-oidc"
  }
}

For å fi


Legg inn kode for autentisering.
Konfigurer applikasjon.
Test at innlogging fungerer.
Lage redirect som hetner kun for en bruker? 

## Autorisasjon

Autorisasjon 

Nå skal vi sette opp applikasjonen slik at kun noen brukere for laste opp bilder, mens
alle som er innlogget får se bildene. Brukere som ikke er logget inn skal ikke ha mulighet til å se bildene.

Vi ønsker å implementere rollebasert autorisasjon i applikasjonen, slik at kun en rolle (Uploader)
skal ha mulighet til å 
Bildeapplikasjonen vil være en 


Først må du legge til rollen du ønsker Azure AD skal returnere dersom brukeren som autentiserer 
seg med.
Legge til rolle
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



1. Skjul v

2. Selv om du skjuler opplastings-funksjonaliteten i brukergrensesnittet, er det viktig at man også krever 



Microsoft Identity Platform 2.0 https://docs.microsoft.com/en-us/azure/active-directory/develop/
består av:
OAuth 2.0 and OpenID Connect standard-compliant authentication service
- Jobb eller skole-kontoer.
- Personlige-kontoer


Legg til Microsoft.AspNetCore.Authentication.AzureAD.UI nuget pakke (Viktig: velg versjon 2.1.1, siden vi bruker .NET Core 2.1.


## Sikre bilder (bør flyttes til Leksjon 1)

Nå når du har laget innlogging i applikasjonen, så er det viktig at poeng at . Til nå har bildene kun ligget åpen container i Storage Accounts, og 



Legge inn slik at SAS-token.
Et SAS-token (Shared Access Signature) er en begrenset rettighet til.

Merk; dersom 



Skru public til private.
Se om RBAC og innlogget identity kan brukes.
Sikre at ingen eksterne får tilgang til bildene.
Bildene lagres per bruker (GUID)
