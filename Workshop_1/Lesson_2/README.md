# Azure Storage Account

Denne leksjonen tar for seg Azure Storage. Azure Storage er en lagringstjeneste hvor man kan lagre data i Azure på en forholdsvis rimelig måte. En Storage Account kan inneholde fork

* Blobs - For å lagre filer i Containere. 
* Tables - (Database)-tabeller som lagres i.
* Queues - Persistente køer.
* Files - Filshare

I denne leksjonen skal vi jobbe med blobs for å lagre bildene til app'en i, mens vi oppretter en Table for å lagre metadata om bildene.

https://azure.microsoft.com/en-us/features/storage-explorer/


## Opprette storage account og container

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
* Dersom du endrer Access Keys på en Storage Account, så vil access-token


## Implementer klasse for

I solution din er det en mock-implementasjon av 

1. Implementer