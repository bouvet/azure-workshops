# Bonusleksjoner

## Bonus 1: Azure Key Vault

Denne leksjonen tar for seg Azure Key Vault. Azure Key Vault er et verktøy for sikker lagring og aksessering av secrets. Dette kan være f.eks. API-nøkler, passord eller sertifikater, ting man gjerne ikke vil ha liggende rundt i configfiler (eller kode.)

Les mer her: https://docs.microsoft.com/en-in/azure/key-vault/key-vault-whatis

### Opprett key vault

Frem til nå har blob storage-secrets vært lagret i appsettings.json. Vi skal opprette et key vault og kode om applikasjonen til å benytte dette.

1. I Visual Studio, klikk på `Project` øverst til venstre, derretter velg `Add Connected Service`.
2. Klikk på + tegnet ved siden av `Service Dependencies`
3. Velg Azure Key Vault 
4. Opprett Azure Key Vault ved å trykke på + tegnet og skriv inn følgende verdier:

    | Azure Key Vault | Verdi                                                   |
    | --------------- | ------------------------------------------------------- |
    | Resource name   | `Valgfritt`                                             |
    | Subscription    | `Visual Studio Professional || Den du har tilgjengelig` |
    | Resource Group  | `Den du opprettet tidlig`                               |
    | Location        | `Anbefalt Norway East || Valgfritt`                     |
    | SKU             | `Standard`                                              |
    

5. Klikk deg videre, sørg for at alle "Project changes" er huket på derretter klikk på "Finish"
6. Gå til <a href="https://portal.azure.com/">Azure Portalen</a>, og finn din nylige opprettede Azure Key Vault.
7. Gå til Settings -> Secrets og klikk Generate/Import
8. Lag en nøkkel for Storage account name opprettet i forrige leksjon. Den skal ha følgende verdier:

    | Name                              | Value (secret)                                        |
    | --------------------------------- | ----------------------------------------------------- |
    | `AzureStorageConfig--AccountName` | `Verdien som står i AccountName fra appsettings.json` |
    
9.  Lag en nøkkel for Storage Account Key opprettet i forrige leksjon. Den skal ha følgende verdier:

    | Name                             | Value (secret)                                       |
    | -------------------------------- | ---------------------------------------------------- |
    | `AzureStorageConfig--AccountKey` | `Verdien som står i AccountKey fra appsettings.json` |

### Bruk key vault

Vi ønsker nå at applikasjonen skal bruke verdiene satt i Key Vault fremfor den gamle config-filen. Heldigvis er det god støtte i .net core for å ta i bruk dette.

Start med å gå til appsettings.json og slett verdien i AccountName og AccountKey, slik at AzureStorageConfig ser slik ut: 
```
  "AzureStorageConfig": {
    "AccountName": "",
    "AccountKey": "",
    "ImageContainer": "Navnet på din image container"
  }
```

Gå så til Program.cs og erstatt `CreateWebHostBuilder` med følgende:

```
private const string KeyVaultEndpoint = "https://<navn-på-keyvault>.vault.azure.net/";

public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
    WebHost.CreateDefaultBuilder(args)
           .ConfigureAppConfiguration((ctx, builder) =>
           {
               var azureServiceTokenProvider = new AzureServiceTokenProvider();
               var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
               builder.AddAzureKeyVault(KeyVaultEndpoint, keyVaultClient, new DefaultKeyVaultSecretManager());
           })
           .UseStartup<Startup>();
```

Referer til de nye komponentene:

```
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
```

Til slutt gjenstår det bare å kjøre applikasjonen og se at alt fortsatt fungerer. Trykk F5 og se at innstillingene fra Key Vault blir brukt.


## Publisering til Azure og Managed Service Identity 

Publiser så applikasjonen din på nytt til Azure. Nå vil du mest sannsynlig se at applikasjonen din feiler.

Dersom du ønsker å finne ut hvorfor dette skjer kan du:

1. Gå til Web App'en din i Azureportalen, og velg "Console" under "Development Tools".
2. Gå så til katalogen d:\home\LogFiles og inspiser filen eventlog.xml (bruk f.eks. kommandoen tail for å se slutten på filen)

Det du finner ut er mest sannsynlig at Web App'en din ikke har tilgang til å lese ut secrets fra Key Vault. For å løse dette ønsker
vi å bruke Managed Service Identity (MSI), som gjør at man kan opprette en identitet (Service Principal/"bruker") for Web App'en din som man igjen kan gi de tilganger man vil til andre tjenester i Azure (her i Key Vault).

1. Gå til Web Appen din i Azureportalen.
2. Velg så `Identity` under Settings, og trykk på `On` og trykk Save. Kopier samtidig `Object ID` til din utklippstavle.
3. Gå så til Key Vault'en din.
4. Velg `Access policies` under settings.
5. Trykk `Add Access Policies`.
6. På `Select principal`, søker du opp navnet på Web App'en din eller limer inn `Object ID` om du husket å kopiere denne i steg 2.
7. Velg så `Get` og `List` under `Secret Permissions`
8. Trykk på OK.

Nå skal applikasjonen din ha tilgang til å lese secrets fra denne Key Vaulten.
