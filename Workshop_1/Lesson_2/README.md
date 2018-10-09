# Leksjon 2: Azure Storage Account

Denne leksjonen tar for seg Azure Storage. Azure Storage er en lagringstjeneste hvor man kan lagre data i Azure på en forholdsvis rimelig måte. En Storage Account kan inneholde forkjellige typer Storage:

* Blobs - For å lagre filer i Containere. 
* Tables - NoSQL, No-Schema database.
* Queues - Persistente køer.
* Files - Filer som oppfører seg om et filshare.

I denne leksjonen skal vi jobbe med blobs for å lagre bildene til app'en i.

Det er flere måter 
* Portalen: Ved å velge Data Explorer.
* Visual Studio: I Cloud Explorer i Visual Studio kan man gjøre.
* Azure Storage Explorer: Azure Storage Explorer er en klient fra Microsoft for å administrere og bruke Storage Accounts: https://azure.microsoft.com/en-us/features/storage-explorer/

Du velger selv hvilken metode du ønsker å prøve ut.

## Opprette storage account og container for blobs

Det er flere metoder for å opprette ressurser i Azure. De mest vanlige er her:

* Portalen. Manuelt opprette ressurser med pek- og klikk. 
* Powershell: Det finnes egne Powershell-moduler (AzureRM f.eks.) for å administrere Azure. Powershell også kjøres i Azure Shell i nettleseren.
* Azure CLI. Kryss-plattform shell som blant annet finnes både på Windows, Mac og Linux.
* ARM-templates - deklarativ beskrivelse av Azure-komponenter og infrastruktur i JSON-format. 

I denne leksjonen skal du bruke Powershell for å opprette en Storage Account og en Blob-Container.
Hovedregelen er at alt du kan gjøre i portalen kan du også gjøre via Powershell, og det er også mye.

For å opprette en storage account og en blob-container ved å bruke Powershell. 

1. Start et Powershell kommandolinjevindu, eller bruk Azure Shell.
2. (Hvis du bruker ikke har gjort dette før, Install-Module AzureRM)
3. Autentiser deg mot din Azure-konto med kommandoen: Login-AzureRmAccount
4. Gå til denne siden for å lære hvordan det utføres: https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-powershell 
5. Opprett en storage account i samme ressursgruppe som du opprettet i leksjon 1.
6. Opprett en blob-container i storage account.
7. Test containeren ved å laste opp en fil ved enten å bruke Storage Explorer, Visual Studio eller Powershell.

## Implementer klasse StorageHelper

I solution ligger en klasse som heter StorageHelper hvor det meste av funksjonalitet mangler. Jobben din er å skrive kode som bruker API-ene til Azure Storage for å laste opp bilder til blob storage, og deretter hente ut url-ene til de samme bildene.

1. Legg til NuGet-pakke for Azure Storage: WindowsAzure.Storage.

2. Hent ut AccountName og AccountKey for storage kontoen du opprettet tidligere og legg disse i appsettings.json.

3. Implementer metoden for å laste opp fil:

   __UploadFileToStorage (Stream fileStream, string fileName, AzureStorageConfig storageConfig)__
   
   Følgende klasser og metoder fra Microsoft.WindowsAzure.Storage ble brukt i løsningsforslaget.

   | Klasse              | Metoder               |
   |---------------------|-----------------------|
   | StorageCredentials  |                       |
   | CloudStorageAccount | CreateCloudBlobClient |
   | CloudBlobClient     | GetContainerReference |
   | CloudBlobContainer  | GetBlockBlobReference |
   | CloudBlockBlob      | UploadFromStreamAsync |
    
4. Implementer metoden for å hente ut URL-er til blob-ene.
   
   __GetImageUrls (AzureStorageConfig storageConfig)__
   
   Følgende klasser og metoder fra Microsoft.WindowsAzure.Storage ble brukt i løsningsforslaget.
   
   | Klasse              | Metoder               |
   |---------------------|-----------------------|
   | StorageCredentials  |                       |
   | CloudStorageAccount | CreateCloudBlobClient |
   | CloudBlobClient     | GetContainerReference |
   | CloudBlobContainer  | GetBlockBlobReference, ListBlobsSegmentedAsync |
   | BlobContinuationToken | Lag for eksempel en do-while. Start med å kalle ListBlobsSegmentedAsync og enumerer resultat-segmentet som returneres. Fortsett å gjøre dette så lenge continuation token i resultat-segmentet ikke er null. Når continuation tokenet er null, så har det siste segmentet blitt returnert og loopen kan brytes. |
   

