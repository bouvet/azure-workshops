# Bonusleksjoner

## Bonus 1: Azure Key Vault

Denne leksjonen tar for seg Azure Key Vault. Azure Key Vault er et verktøy for sikker lagring og aksessering av secrets. Dette kan være f.eks. API-nøkler, passord eller sertifikater, ting man gjerne ikke vil ha liggende rundt i configfiler (eller kode.)

Les mer her: https://docs.microsoft.com/en-in/azure/key-vault/key-vault-whatis

### Opprett key vault

Frem til nå har blob storage-secrets vært lagret i appsettings.json. Vi skal opprette et key vault og kode om applikasjonen til å benytte dette.

1. Gå til Azure-portalen om du ikke allerede er der
2. Åpne Azure Shell (som du brukte til å opprette storage account i forrige leksjon)
3. For å lage en key vault så kjøres kommandoen `az keyvault create --resource-group "{resource group name}" --location norwayeast --name "{keyvaultname}"` For å slippe å opprette en ny ressursgruppe, er det fint om du gjenbruker samme ressursgruppe som i tidligere leksjoner
4. Finn ditt nylig opprettede KeyVault i portalen
5. Gå til Settings -> Secrets og klikk Generate/Import
6. Lag en nøkkel for ConnectionString opprettet i forrige leksjon. Den skal ha følgende verdier:

    | Name | Value (secret) |
    |---------------------------------|-----------------------------------------------------|
    | `AzureStorageConfig--ConnectionString` | `Verdien som står i ConnectionString fra appsettings.json` |


### Bruk key vault

Vi ønsker nå at applikasjonen skal bruke verdiene satt i Key Vault fremfor den gamle config-filen. Heldigvis er det god støtte i .net core for å ta i bruk dette.

Først må du installere nødvendige NuGet-pakker:
- Azure.Identity
- Azure.Extensions.AspNetCore.Configuration.Secrets

Start med å gå til appsettings.json og slett verdien i AccountName og AccountKey, slik at AzureStorageConfig ser slik ut: 
```
  "AzureStorageConfig": {
    "ConnectionString": "",
    "ImageContainer": "Navnet på din image container" (default er imagecontainer)
  }
```

Gå så til Program.cs og erstatt `CreateWebHostBuilder` med følgende:

```
private const string KeyVaultEndpoint = "https://<navn-på-keyvault>.vault.azure.net/";

public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
    WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((ctx, builder) =>
            {
                builder.AddAzureKeyVault(new Uri(KeyVaultEndpoint), new DefaultAzureCredential());
            })
            .UseStartup<Startup>();
```

Her setter man opp KeyVault, og bruker DefaultAzureCredential til å autentisere mot KeyVault. Dette er en måte å autentisere på som prøver gjennom forskjellige credential-types, om man er interessert står det mer informasjon om dette [finnes det her](https://docs.microsoft.com/en-us/dotnet/api/overview/azure/identity-readme). I vårt tilfelle vil dette fungere i skyen når man senere setter opp managed identity for autentisering mellom appen og KeyVault, og den bør fungere lokalt ved å bruke innlogget bruker i Visual Studio.

Til slutt gjenstår det bare å kjøre applikasjonen og se at alt fortsatt fungerer. Trykk F5 og se at innstillingene fra Key Vault blir brukt.

> Dersom man ved kjøring av applikasjonen får feilmelding `"Azure.Identity.CredentialUnavailableException: 'DefaultAzureCredential failed to retrieve a token from the included credentials."`, kan man i Visual Studio gå til Tools -> Options -> Azure Services Authentication, og re-autentisere derfra.

### Publisering til Azure og Managed Service Identity 

Publiser så applikasjonen din på nytt til Azure. Nå vil du mest sannsynlig se at applikasjonen din feiler.

Dersom du ønsker å finne ut hvorfor dette skjer kan du:

1. Gå til Web App'en din i Azureportalen, og velg "Console" under "Development Tools".
2. Gå så til katalogen d:\home\LogFiles og inspiser filen eventlog.xml (bruk f.eks. kommandoen tail for å se slutten på filen)

Det du finner ut er mest sannsynlig at Web App'en din ikke har tilgang til å lese ut secrets fra Key Vault. For å løse dette ønsker
vi å bruke [Managed identities](https://docs.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/overview), som gjør at man kan velge å aktivere en managed identity (Service Principal/"bruker") for Web App'en din som man igjen kan gi de tilganger man vil til andre tjenester i Azure (her i Key Vault).

1. Gå til Web Appen din i Azureportalen.
2. Velg så `Identity` under Settings, og trykk på `On` og trykk Save. Kopier samtidig `Object ID` til din utklippstavle.
3. Gå så til Key Vault'en din.
4. Velg `Access policies` under settings.
5. Trykk `Add Access Policies`.
6. På `Select principal`, søker du opp navnet på Web App'en din eller limer inn `Object ID` om du husket å kopiere denne i steg 2.
7. Velg så `Get` og `List` under `Secret Permissions`
8. Trykk på Add
9. Husk å trykke Save for å lagre endringene til Key Vaultet

Nå skal applikasjonen din ha tilgang til å lese secrets fra denne Key Vaulten, og du kan prøve å oppdatere applikasjonen din og se at bildene blir vist.