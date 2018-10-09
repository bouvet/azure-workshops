# Leksjon 3: Azure Key Vault

Denne leksjonen tar for seg Azure Key Vault. Azure Key Vault er et verktøy for sikker lagring og aksessering av secrets. Dette kan være f.eks. API-nøkler, passord eller sertifikater, ting man gjerne ikke vil ha liggende rundt i configfiler (eller kode.)

Les mer her: https://docs.microsoft.com/en-in/azure/key-vault/key-vault-whatis

## Opprett key vault

Frem til nå har blob storage-secrets vært lagret i appsettings.json. Vi skal opprette et key vault og kode om applikasjonen til å benytte dette.

1. I Visual Studio, høyreklikk på Connected Services i for prosjektet og velg Add connected service.
2. Velg secure Secrets with Azure Key Vault
3. Velg edit og pass på at settingene stemmer, klikk så add.
4. Klikk Manage secrets stored in this Key Vault. Dette tar deg til azureportalen.
5. Klikk Generate/Import
6. Lag en nøkkel med navn AzureStorageAccountName og value som den står i AccountName fra appsettings.json
7. Lag en nøkkel med navn AzureStorageAccountKey og value som den står i AccountKey fra appsettings.json


## Bruk key vault

Vi ønsker nå at applikasjonen skal bruke verdiene satt i Key Vault fremfor den gamle config-filen.

1. Gå til Startup.cs og fjern configurasjonen av AzureStorageConfig.
2. Legg inn `services.AddTransient<AzureStorageConfig>()`;
3. Gå til AzureStorageConfig.cs og legg til følgende ctor:

```
public AzureStorageConfig(Microsoft.Extensions.Configuration.IConfiguration azureKeyVaultConfig)
{
    AccountName = azureKeyVaultConfig["AzureStorageAccountName"];
    AccountKey = azureKeyVaultConfig["AzureStorageAccountKey"];
    ImageContainer = "imagecontainer";
}
```

ImageContainer kunne med computer science blitt hentet fra appsettings på samme måte som før, siden dette ikke er sensitivt.

4. Gå til ImagesController og erstatt IOptions<AzureStorageConfig> config i ctor med AzureStorageConfig storageConfig.
5. Kjør og se at alt fortsatt fungerer.