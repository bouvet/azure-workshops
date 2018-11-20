# Bonusleksjoner

## Bonus 1: Azure Key Vault

Denne leksjonen tar for seg Azure Key Vault. Azure Key Vault er et verktøy for sikker lagring og aksessering av secrets. Dette kan være f.eks. API-nøkler, passord eller sertifikater, ting man gjerne ikke vil ha liggende rundt i configfiler (eller kode.)

Les mer her: https://docs.microsoft.com/en-in/azure/key-vault/key-vault-whatis

### Opprett key vault

Frem til nå har blob storage-secrets vært lagret i appsettings.json. Vi skal opprette et key vault og kode om applikasjonen til å benytte dette.

1. I Visual Studio, høyreklikk på Connected Services i for prosjektet og velg Add connected service.
2. Velg secure Secrets with Azure Key Vault
3. Velg edit og pass på at settingene stemmer, klikk så add.
4. Klikk Manage secrets stored in this Key Vault. Dette tar deg til azureportalen.
5. Klikk Generate/Import
6. Lag en nøkkel med navn AzureStorageAccountName og value som den står i AccountName fra appsettings.json
7. Lag en nøkkel med navn AzureStorageAccountKey og value som den står i AccountKey fra appsettings.json

### Bruk key vault

Vi ønsker nå at applikasjonen skal bruke verdiene satt i Key Vault fremfor den gamle config-filen. Heldigvis er det god støtte i .net core for å ta i bruk dette.

Start med å gå til appsettings.json og slett seksjonen AzureStorageConfig. Denne skal vi ikke bruke lenger.

Vi må så legge til noen nuget-pakker (dette kan du gjøre i Nuget Console):

* Install-Package Microsoft.Azure.KeyVault
* Install-Package Microsoft.Azure.Services.AppAuthentication
* Install-Package Microsoft.Extensions.Configuration.AzureKeyVault

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

Gå til Startup.cs og erstatt configurasjonen av `AzureStorageConfig` med:
```
services.AddSingleton(new AzureStorageConfig
{
    AccountKey = Configuration["AzureStorageAccountKey"],
    AccountName = Configuration["AzureStorageAccountName"],
    ImageContainer = "imagecontainer"
});
```

Når dette er gjort vil `Configuration` inneholde entries som hentes fra Azure Key Vault.

ImageContainer kunne hentet fra appsettings på samme måte som før, siden dette ikke er sensitivt, men vi har her valt å legge den her.

Gå til StorageService og erstatt `IOptions<AzureStorageConfig> config` i ctor med `AzureStorageConfig storageConfig`.

Til slutt gjenstår det bare å kjøre applikasjonen og se at alt fortsatt fungerer. Trykk F5 og se at innstillingene fra Key Vault blir brukt.


## Publisering til Azure og Managed Service Identity 

Publiser så applikasjonen din på nytt til Azure. Nå vil du mest sannsynlig se at applikasjonen din feiler.

Dersom du ønsker å finne ut hvorfor dette skjer kan du:

1. Gå til Web App'en din i Azureportalen, og velg "Console" under "Development Tools".
2. Gå så til katalogen d:\home\LogFiles og inspiser filen eventlog.xml (bruk f.eks. kommandoen tail for å se slutten på filen)

Det du finner ut er mest sannsynlig at Web App'en din ikke har tilgang til å lese ut secrets fra Key Vault. For å løse dette ønsker
vi å bruke Managed Service Identity (MSI), som gjør at man kan opprette en identitet (Service Principal/"bruker") for Web App'en din som man igjen kan gi de tilganger man vil til andre tjenester i Azure (her i Key Vault).

1. Gå til Web Appen din i Azureportalen.
2. Velg så "Managed Service Identity" under Settings, og trykk på "On" og trykk Save.
3. Gå så til Key Vault'en din.
4. Velg "Access policies" under settings.
5. Trykk "Add new".
6. På "Select principal", søker du opp navnet på Web App'en din.
7. Velg så "Get" og "List" under "Secret Permissions"
8. Trykk på OK.

Nå skal applikasjonen din ha tilgang til å lese secrets fra denne Key Vaulten.