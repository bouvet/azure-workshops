# Bonusleksjoner

## Bonus 1: Azure Key Vault

Denne leksjonen tar for seg Azure Key Vault. Azure Key Vault er et verktøy for sikker lagring og aksessering av secrets. Dette kan være f.eks. API-nøkler, passord eller sertifikater, ting man gjerne ikke vil ha liggende rundt i configfiler (eller kode.)

Les mer her: https://docs.microsoft.com/en-in/azure/key-vault/key-vault-whatis

### Opprett key vault

Frem til nå har blob storage-secrets vært lagret i appsettings.json. Vi skal opprette et key vault og kode om applikasjonen til å benytte dette.

1. Last ned <a href="https://docs.microsoft.com/en-us/cli/azure/install-azure-cli">Azure CLI</a> (hvis du ikke har gjort dette steget tidligere)
2. Åpne en terminal (cmd, powershell, terminal e.l. for ditt OS)
3. Kjør kommandoen az login (hvis du ikke har Azure CLI vil dette feile)
4. Etter å ha logget inn så vil det bli listet alle subscriptions du har tilgang til og en av disse vil ha isDefault: true
5. Hvis du må bytte subscription så gjøres det med kommandoen >az account set --subscription "{subscription name or id}"
6. For å lage en key vault så kjøres kommandoen >az keyvault create --resource-group "{resource group name}" --location norwayeast --name "{keyvaultname}"
7. Gå til <a href="https://portal.azure.com/">Azure Portalen</a>, og finn din nylige opprettede Azure Key Vault.
8. Gå til Settings -> Secrets og klikk Generate/Import
9. Lag en nøkkel for Storage Account Name opprettet i forrige leksjon. Den skal ha følgende verdier:

    | Name | Value (secret) |
    |---------------------------------|-----------------------------------------------------|
    | `AzureStorageConfig--AccountName` | `Verdien som står i AccountName fra appsettings.json` |
    
10.  Lag en nøkkel for Storage Account Key opprettet i forrige leksjon. Den skal ha følgende verdier:
    
| Name | Value (secret) |
|-------------------------------- | ----------------------------------------------------|
| `AzureStorageConfig--AccountKey` | `Verdien som står i AccountKey fra appsettings.json` |


### Bruk key vault

Vi ønsker nå at applikasjonen skal bruke verdiene satt i Key Vault fremfor den gamle config-filen. Heldigvis er det god støtte i .net core for å ta i bruk dette.

Start med å gå til appsettings.json og slett verdien i AccountName og AccountKey, slik at AzureStorageConfig ser slik ut: 
```
  "AzureStorageConfig": {
    "AccountName": "",
    "AccountKey": "",
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
8. Trykk på Add
9. Husk å trykke Save for å lagre endringene til Key Vaultet

Nå skal applikasjonen din ha tilgang til å lese secrets fra denne Key Vaulten.
