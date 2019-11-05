# Leksjon 2: Azure AD (HE)

## Azure AD



I denne leksjonen skal vi lage innlogging for applikasjonen. 

Denne applikasjonen vil kun tillate 

### App registration

1. Logg inn i Azure-portalen (https://portal.azure.com).
2. Velg "Azure Active Directory" i menyen til venstre.
3. Velg "App registrations", så trykk på "+ New registration"
4. Gi applikasjonen din et navn, og merk dette navnet slik at du vet at denne er for lokal debugging (eksempel: Image-localhost)
5. Du kan velge hvem Om det er kun brukere som er registrert i din egen Azure AD, hvilken som helst organisasjon i Azure AD eller 
6. Velg så "Web" under "Redirect URI", og skriv inn adressen brukeren skal bli sendt videre "http://localhost:2389/

Gjenta prosessen over en gang til, men gi applikasjonen et nytt navn, og gi i 6) til "https://<dinapplikasjon>.azurewebsites.net". Dette vil være URL'en i Azure som du blir 

- Identity, SSO (pres)
- Generell (pres)
- RBAC (pres)

Satt opp innlogging på applikasjonen.

### Klargjør
For å kunne bruke innloggingsfunksjonaliteten, så trenger du to verdier. Det ene er 
. Det andre er tenantid som 

Legg til Microsoft.AspNetCore.Authentication.AzureAD.UI nuget pakke (Viktig: velg versjon 2.1.1, siden vi bruker .NET Core 2.1.








## Sikre bilder

Skru public til private.
Se om RBAC og innlogget identity kan brukes.
Sikre at ingen eksterne får tilgang til bildene.
Bildene lagres per bruker (GUID)
