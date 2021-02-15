# Leksjon 3: Azure Storage Account

Denne leksjonen tar for seg Azure Storage. Azure Storage er en lagringstjeneste hvor man kan lagre data i Azure på en forholdsvis rimelig måte. En Storage Account kan inneholde forkjellige typer Storage:

- Blobs - For å lagre filer i Containere.
- Tables - NoSQL, No-Schema database.
- Queues - Persistente køer.
- Files - Filer som oppfører seg om et filshare.

I denne leksjonen skal vi jobbe med blobs for å lagre bildene til app'en i.

Det er flere måter å manipulere data på i Azure Storage:

- Portalen: Ved å velge Data Explorer.
- Visual Studio: I Cloud Explorer i Visual Studio kan man gjøre.
- Azure Storage Explorer: Azure Storage Explorer er en klient fra Microsoft for å administrere og bruke Storage Accounts: https://azure.microsoft.com/en-us/features/storage-explorer/

Du velger selv hvilken metode du ønsker å prøve ut.

## Opprette storage account og container for blobs

Det er flere metoder for å opprette ressurser i Azure. De mest vanlige er her:

- Portalen. Manuelt opprette ressurser med pek- og klikk.
- Powershell: Det finnes egne Powershell-moduler (AzureRM f.eks.) for å administrere Azure. Powershell også kjøres i Azure Shell i nettleseren.
- Azure CLI. Kryss-plattform shell som blant annet finnes både på Windows, Mac og Linux.
- ARM-templates - deklarativ beskrivelse av Azure-komponenter og infrastruktur i JSON-format.

I denne leksjonen skal du bruke Powershell eller Azure CLI for å opprette en Storage Account og en Blob-Container. Hovedregelen er at alt du kan gjøre i portalen kan du også gjøre via Powershell/Azure CLI, og det er også mye mer du kan gjøre via kommandolinje.

Powershell er fint å bruk i Windows miljøer, mens Azure CLI er kryssplatform og kan brukes både på Linux, Mac og Windows. Begge er tilgjengelige i Azure Shell.

For å opprette en storage account og en blob-container:

1. Start et Azure Shell kommando ved å klikke på ">\_" på toppen i portalen.
2. Trykk ja når du får spørsmål om å opprette en storage account. Dette er en storage account som bruker Azure Shellet og som lever bak kulissene for at man skal kunne lagre filer og lignende, mens man er i shellet. Det er ikke mulig å bruke denne storage accounten i applikasjonen.
3. Gå til denne siden for å lære hvordan du oppretter en egen Storage Account, som du ønsker å bruke i applikasjonen din.
   
   * Powershell:
   https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-powershell
    
   * Azure CLI:
   https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-cli

4. Opprett en container i Blob Servicen som Storage Accounten tilbyr. Denne skal ha Public Access Level satt til: 
    ```sh
    Blob (anonymous read access for blobs only).
    ```
   
1. Test containeren ved å laste opp en fil ved enten å bruke <a href="https://azure.microsoft.com/en-us/features/storage-explorer/">Storage Explorer</a> eller i <a href="https://portal.azure.com">Azure Portalen</a>.


## Implementer servicen StorageService

I solution ligger en klasse som heter StorageService hvor det meste av funksjonalitet mangler. Jobben din er å skrive kode som bruker API-ene til Azure Storage for å laste opp bilder til blob storage, og deretter hente ut url-ene til de samme bildene.

1. Legg til NuGet-pakke for Azure Storage: WindowsAzure.Storage.

2. Hent ut StorageAccountName og AccountKey fra Access keys i menyen til Storage Account. Hent også ut navnet på blob-kontaineren du opprettet tidligere. Legg de inn i appsettings.json filen i webapplikasjonen.

3. Implementer metoden for å laste opp fil:

   **UploadFileToStorage (Stream fileStream, string fileName)**

   Følgende klasser og metoder fra Microsoft.WindowsAzure.Storage ble brukt i løsningsforslaget.

   | Klasse              | Metoder               |
   | ------------------- | --------------------- |
   | StorageCredentials  |                       |
   | CloudStorageAccount | CreateCloudBlobClient |
   | CloudBlobClient     | GetContainerReference |
   | CloudBlobContainer  | GetBlockBlobReference |
   | CloudBlockBlob      | UploadFromStreamAsync |

   Dersom du står fast eller ønsker å se et ferdig eksempel så kan du se [her](https://github.com/bouvet/azure-workshops/blob/master/Workshop_1/Komplett/AzureWorkshop/AzureWorkshopApp/Services/StorageService.cs).

4. Implementer metoden for å hente ut URL-er til blob-ene.

   **GetImageUrls ()**

   Følgende klasser og metoder fra Microsoft.WindowsAzure.Storage ble brukt i løsningsforslaget.

   | Klasse                | Metoder                                                                                                                                                                                                                                                                                                         |
   | --------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
   | StorageCredentials    |                                                                                                                                                                                                                                                                                                                 |
   | CloudStorageAccount   | CreateCloudBlobClient                                                                                                                                                                                                                                                                                           |
   | CloudBlobClient       | GetContainerReference                                                                                                                                                                                                                                                                                           |
   | CloudBlobContainer    | GetBlockBlobReference, ListBlobsSegmentedAsync                                                                                                                                                                                                                                                                  |
   | BlobContinuationToken | Lag for eksempel en do-while. Start med å kalle ListBlobsSegmentedAsync og enumerer resultat-segmentet som returneres. Fortsett å gjøre dette så lenge continuation token i resultat-segmentet ikke er null. Når continuation tokenet er null, så har det siste segmentet blitt returnert og loopen kan brytes. |

   Dersom du står fast eller ønsker å se et ferdig eksempel så kan du se [her](https://github.com/bouvet/azure-workshops/blob/master/Workshop_1/Komplett/AzureWorkshop/AzureWorkshopApp/Services/StorageService.cs).
