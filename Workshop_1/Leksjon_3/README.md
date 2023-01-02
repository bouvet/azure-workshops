# Leksjon 3: Azure Storage Account

Denne leksjonen tar for seg Azure Storage. Azure Storage er en lagringstjeneste hvor man kan lagre data i Azure på en forholdsvis rimelig måte. En Storage Account kan inneholde forkjellige typer Storage:

- Blobs - For å lagre filer i Containere.
- Tables - NoSQL, No-Schema database.
- Queues - Persistente køer.
- Files - Filer som oppfører seg om et filshare.

I denne leksjonen skal vi jobbe med blobs for å lagre bildene til app'en i.

Det er flere måter å manipulere data på i Azure Storage:

- Portalen: Ved å velge Data Explorer.
- Visual Studio: I Cloud Explorer i Visual Studio.
- Azure Storage Explorer: Azure Storage Explorer er en klient fra Microsoft for å administrere og bruke Storage Accounts: <a href="https://azure.microsoft.com/en-us/features/storage-explorer/">https://azure.microsoft.com/en-us/features/storage-explorer/</a>

Du velger selv hvilken metode du ønsker å prøve ut.

## Opprette storage account og container for blobs

Det er flere metoder for å opprette ressurser i Azure. De mest vanlige er her:

- Portalen. Manuelt opprette ressurser med pek- og klikk.
- Azure Powershell: Det finnes egne Powershell-moduler (AzureRM f.eks.) for å administrere Azure.
- Azure CLI. Kryss-plattform shell som blant annet finnes både på Windows, Mac og Linux.
- ARM-templates - deklarativ beskrivelse av Azure-komponenter og infrastruktur i JSON-format.
- Bicep - deklarativ beskrivelse slik som ARM-templates, men i et kodespråk som er komponerbart.

I denne leksjonen skal du bruke Powershell eller Azure CLI for å opprette en Storage Account og en Blob-Container. Hovedregelen er at alt du kan gjøre i portalen kan du også gjøre via Powershell/Azure CLI og mye mer.

Azure Shell støtter to forskjellige moduser: Bash eller PowerShell. Bash er tradisjonelt kommandolinjespråket for Unix-varianter, som MacOS og Linux, mens PowerShell er vanligst i Windows. Azure Powershell er vanligst å bruke i Windows-miljøer, mens Azure CLI er kryssplatform og kan brukes både på Linux, Mac og Windows. Begge er tilgjengelige i Azure Shell.  Men for å gjøre forvirringen komplett kan du bruke Azure PowerShell fra Bash og Azure CLI i PowerShell, så dette handler om valgfrihet og preferanser. Hvis du ikke har noen preferanser foreslår vi å bruke Bash og Azure CLI i oppskriften under.

For å opprette en storage account og en blob-container skal vi nå bruke Azure Shell i portalen:

1. Gå til <a href="https://portal.azure.com">portalen</a> og logg inn. Bytte av directory gjøres ved å trykke på profil (øverst i høyre hjørnet) og på "switch directory" (hvis det er nødvendig).
1. Start et Azure Shell kommando ved å klikke på ">\_" på toppen i portalen.
1. Velg om du vil bruke Bash eller PowerShell. 
1. Trykk ja når du får spørsmål om å opprette en storage account. Dette er en storage account som bruker Azure Shellet og som lever bak kulissene for at man skal kunne lagre filer og lignende, mens man er i shellet. Det er ikke mulig å bruke denne storage accounten i applikasjonen.
1. Gå til denne siden for å lære hvordan du oppretter en egen Storage Account, som du ønsker å bruke i applikasjonen din. 
   * Powershell:
   https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-powershell
   * Azure CLI:
   https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-cli
   Vær oppmerksom på at storace accounts kun kan ha små bokstaver og tall i navnet, ingen spesialtegn og max 24 tegn.
1. Opprett en container i Blob Servicen som Storage Accounten tilbyr. Denne skal ha Public Access Level satt til:  
   > Private (no anonymous access)

   
1. Test containeren ved å laste opp en fil ved enten å bruke <a href="https://azure.microsoft.com/en-us/features/storage-explorer/">Storage Explorer</a> eller i <a href="https://portal.azure.com">Azure Portalen</a>.


## Implementer servicen StorageService

I solution ligger en klasse som heter StorageService hvor det meste av funksjonalitet mangler. Jobben din er å skrive kode som bruker API-ene til Azure Storage for å laste opp bilder til blob storage, og deretter hente ut url-ene til de samme bildene.

1. Legg til NuGet-pakker for Azure Storage: Azure.Storage.Blobs 

2. Hent ut ConnectionString fra Access keys i menyen til Storage Account. Hent også ut navnet på blob-container du opprettet tidligere. Legg de inn i appsettings.json filen i webapplikasjonen.

3. Implementer metoden for å laste opp fil:

   **UploadFileToStorage (Stream fileStream, string fileName)**

   Se dokumentasjon: 
   * https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blob-upload

   Følgende klasser og metoder fra Azure.Storage.Blobs ble brukt i løsningsforslaget.

   | Klasse              | Metoder               |
   | ------------------- | --------------------- |
   | BlobServiceClient   | GetBlobContainerClient|
   | BlobContainerClient | CreateIfNotExists     |
   |                     | GetBlobClient         |
   | BlobClient          | UploadAsync           |


   Dersom du står fast eller ønsker å se et ferdig eksempel så kan du se [her](https://github.com/bouvet/azure-workshops/blob/master/Workshop_1/Komplett/AzureWorkshop/AzureWorkshopApp/Services/StorageService.cs).

4. Implementer metoden for å hente ut URL-er til blob-ene.

   **GetImageUrls ()**

   Se dokumentasjon: 
   * https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blobs-list
   * https://learn.microsoft.com/en-us/azure/storage/blobs/sas-service-create


   Følgende klasser og metoder fra Azure.Storage.Blobs ble brukt i løsningsforslaget.

   | Klasse                | Metoder                                                                                                                                                                                                                                                                                                         |
   | --------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
   | BlobServiceClient     | GetBlobContainerClient                                                                                                                                                                                                                                                                                          |
   | BlobContainerClient   | GetBlobsAsync/GetBlobClient                                                                                                                                                                                                                                                                                                   |
   | BlobSasBuilder        | SetPermissions                                                                                                                                                                                                                                                                                                  |
   | BlobClient            | GenerateSasUri                                                                                                                                                                                                                                                                                                  |
   | BlobItem              | Name
  
  Lag for eksempel en foreach. Start med å kalle GetBlobsAsync og enumerer resultat-segmentet som returneres. Lag så en BlobClient ved å bruke navnet i BlobItem. 
  Sett så opp BlobSasBuilder med riktige permissions. Det som må settes er:
   * BlobContainerName
   * BlobName
   * Resource (Settes til "b" for blob)
   * ExpiresOn
   * Permissions

   Dersom du står fast eller ønsker å se et ferdig eksempel så kan du se [her](https://github.com/bouvet/azure-workshops/blob/master/Workshop_1/Komplett/AzureWorkshop/AzureWorkshopApp/Services/StorageService.cs).
