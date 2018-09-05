# Azure Storage 

Azure Storage . Alle 


## Opprette storage account og container

Forenklet sagt så kan alt du kan gjøre i portalen også gjøres i Powershell (+ mye mer).

For å opprette en storage account og en blob-container.

1. Start et Powershell kommandolinjevindu
2. Logg inn med kommandoen : Login-AzureRmAccount
3. Gå til: https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-powershell 
4. Opprett en storage account i samme ressursgruppe som du opprettet i leksjon 1.
5. Opprett en blob-container
6. Test containeren ved å laste opp en fil ved enten å bruke Storage Explorer, Visual Studio eller Powershell

## Opprette SAS-token

Et SAS-token (Shared Access Signature) en som følger Key-Valet patternet. Det er et tidsbegrenset token hvor eieren av ressursen gir en annen bruker en 
Merk at:
- Dersom du endrer Access Keys på 


Du ønsker kun å gi tilgang for web-applikasjonen din til den en blob-containeren i storage accounten, og derfor oppretter vi et SAS-token
