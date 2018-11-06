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

Vi ønsker nå at applikasjonen skal bruke verdiene satt i Key Vault fremfor den gamle config-filen. Heldigvis er det godt støtte i .net core for å ta i bruk dette.

1. Vi starter med å legge til noen nuget-pakker:

* Install-Package Microsoft.Azure.KeyVault
* Install-Package Microsoft.Azure.Services.AppAuthentication
* Install-Package Microsoft.Extensions.Configuration.AzureKeyVault

Gå så til Program.cs og erstatt CreateWebHostBuilder med følgende:

`private const string KeyVaultEndpoint = "https://azureworkshopapp-0-kv.vault.azure.net/";

public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
    WebHost.CreateDefaultBuilder(args)
           .ConfigureAppConfiguration((ctx, builder) =>
           {
               var azureServiceTokenProvider = new AzureServiceTokenProvider();
               var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
               builder.AddAzureKeyVault(KeyVaultEndpoint, keyVaultClient, new DefaultKeyVaultSecretManager());
           })
           .UseStartup<Startup>();`

Referer til de nye komponentene:

`using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;`

2. Gå til Startup.cs og erstatt configurasjonen av AzureStorageConfig med:
`services.AddSingleton(new AzureStorageConfig
{
    AccountKey = Configuration["AzureStorageAccountKey"],
    AccountName = Configuration["AzureStorageAccountName"],
    ImageContainer = "imagecontainer"
});`

ImageContainer kunne med computer science blitt hentet fra appsettings på samme måte som før, siden dette ikke er sensitivt. Dette er utenfor scope for denne leksjonen.

3. Gå til ImagesController og erstatt `IOptions<AzureStorageConfig> config` i ctor med `AzureStorageConfig storageConfig`.
4. Gå til appsettings.json og slett seksjonen AzureStorageConfig. Denne skal vi ikke bruke lenger.
5. Kjør og se at alt fortsatt fungerer.


## Bonus 2: lagring av tags på bildene dine

Legg til funksjonalitet i Web-applikasjonen din for å legge på egne tags, tekster som beskriver bildene dine.

* Bruk en Table i Azure Storage kontoen din til å lagre tags i.
* Utvid applikasjonen med funksjonalitet for å legge til tags. Eksempel her kan være et tekst-felt under hvert bilde etc.
* Introduksjon av Azure Table Storage med lenker videre til C# API: https://docs.microsoft.com/en-us/azure/storage/tables/table-storage-overview
