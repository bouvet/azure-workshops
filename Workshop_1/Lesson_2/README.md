# Azure Storage Account

Denne leksjonen tar for seg Azure Storage. Azure Storage er en lagringstjeneste hvor man kan lagre data i Azure på en forholdsvis rimelig måte. En Storage Account kan inneholde forkjellige typer Storage:

* Blobs - For å lagre filer i Containere. 
* Tables - NoSQL, No-Schema database
* Queues - Persistente køer.
* Files - Filshare

I denne leksjonen skal vi jobbe med blobs for å lagre bildene til app'en i, mens vi oppretter en Table for å lagre metadata om bildene.

Det er 
* Portalen: Ved å velge Data Explorer 
* Visual Studio: I Cloud Explorer kan man
* https://azure.microsoft.com/en-us/features/storage-explorer/

Du velger selv hvilken

## Opprette storage account og container



* Portalen. Manuelt opprette ressurser med pek- og klikk. 
* Powershell: . Kan også kjøres i Azure Shell i nettleseren.
* Azure CLI. Kryss-platform
* ARM-templates


Forenklet sagt så kan alt du kan gjøre i portalen også gjøres i Powershell (+ mye mer).

For å opprette en storage account og en blob-container ved å bruke Powershell.

1. Start et Powershell kommandolinjevindu
2. (Hvis du ikke har Install-Module )
3. Autentiser deg mot din Azure-konto med kommandoen: Login-AzureRmAccount
3. Gå til for å lære hvordan : https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-powershell 
4. Opprett en storage account i samme ressursgruppe som du opprettet i leksjon 1.
5. Opprett en blob-container i storage account.
6. Test containeren ved å laste opp en fil ved enten å bruke Storage Explorer, Visual Studio eller Powershell

## Opprette SAS-token

Et SAS-token (Shared Access Signature) en som følger Key-Valet patternet. Det er et tidsbegrenset token hvor eieren av ressursen gir en annen bruker en tidsbegrenset tilgang til ressursen.



Du ønsker kun å gi tilgang for web-applikasjonen din til den en blob-containeren i storage accounten, og derfor oppretter du et to SAS-token.

1. Bruk 


5. Gjenta 

Merk at:
* Det er ikke mulig å "tilbakekalle" et token.
* Dersom du endrer Access Keys på en Storage Account, så vil access-token bli ugyldig.


## Implementer klasse StorageHelper

I solution under Lesson_2 ligger det en klasse som heter StorageHelper hvor det meste av funksjonalitet mangler. Jobben din er å skrive kode som bruker API-ene til Azure Storage for å laste opp bilder til blob storage, og deretter hente ut url til de samme bildene.

Implementer metodene:
* UploadFileToStorage (Stream fileStream, string fileName, AzureStorageConfig storageConfig)
    * Lag et StorageCredentials-objekt ved å bruke konfigurasjonen i parameteret storageConfig.
    * Bruk storage credentials til å lage en instans av CloudStorageAccount.
    * Lag en CloudBlobClient vha. storageAccount.CreateCloudBlobClient().
    * Bruk klienten for å hente referanse til kontaineren hvor bildene skal ligge(CloudBlobContainer). Navnet på container ligger i storageConfig.
    * Hent referanse til block blob fra kontaineren vha. container.GetBlockBlobReference(fileName).
    * Last opp filen: await blockBlob.UploadFromStreamAsync(fileStream);
    * Returner 'true'.

