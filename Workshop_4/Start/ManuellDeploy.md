# Manuell deploy av infrastruktur og kode

### Deploy infrastruktur til Azure
1. Åpne Powershell/Terminal
1. `az login` Logg på bouvet kontoen din
1. Velg Azureskolen subscription az account set --subscription "Azureskolen"
   * Hvis du ikke har Azureskolen subscription tilgjengelig, be om tilgang eller bruk et annet subscription du har tilgjengelig
1. Lag så en Resource Group ved å kjøre kommandoen az group create --location westeurope --name YourResourceGroup
   * Bytt ut YourResourceGroup med et navn på din ressursgruppe
1. Naviger til Start/AzureWorkshopInfrastruktur/AzureWorkshopInfrastruktur mappen
   * Her finner du azuredeploy.json og azuredeploy.parameters.json
1. Åpne azuredeploy.parameters.json i en tekst editor
1. Legg inn verdier for parameterne som ikke er satt (husk å lagre og at Azure ofte krever unike ressursnavn)
1. Kjør så kommandoen az deployment group create --name ExampleDeployment --resource-group YourResourceGroup --template-file .\azuredeploy.json --parameters '@azuredeploy.parameters.json'
   * Bytt ut ExampleDeployment og YourResourceGroup med et navn på deploymenten (f.eks. infrastrukturDeployment) og resource groupen du lagde ovenfor
1. Hvis alt gikk gjennom så skal du få en json output med ressursene som ble laget. Dersom det ikke er tilfellet legg til --debug flagget i kommandoen. Om du da finleser output fra kommandoen, så finner du trolig ut at et av ressursnavnene ikke er unikt
1. Gå gjerne til portalen og sjekk at ressursene ligger i ressurs gruppen din


#### Deploy av AzureWorkshopApp

1. Åpne en terminal/powershell
1. Naviger til `Start/AzureWorkshop/AzureWorkshopApp` mappen
1. Kjør kommandoen `dotnet publish -c Release`
   - Dette lager en release av koden
1. Zip filene i mappen `./bin/Release/net8.0/publish/` 
   - Powershell `Compress-Archive -Path ./bin/Release/net8.0/publish/* -DestinationPath ./code.zip` (legg til -Force for å overskrive)
   - Terminal (Linux/Mac) `zip -r code.zip ./bin/Release/net8.0/publish/*`
1. Deploy ved å kjøre `az functionapp deployment source config-zip -g {YourResourceGroup} -n {YourAppServiceName} --src code.zip` 
1. Hvis du får tilbake `Deployment endpoint responded with status code 202` så er applikasjonen lastet opp og klar til å testes

#### Deploy av FunctionApp

1. Åpne en terminal/powershell
1. Naviger til `Start/AzureWorkshop/AzureWorkshopFunctionApp` mappen
1. Kjør kommandoen `dotnet publish -c Release`
   - Dette lager en release av koden
1. Zip filene i mappen `./bin/Release/net8.0/publish/` 
   - Powershell `Compress-Archive -Path ./bin/Release/net8.0/publish/* -DestinationPath ./code.zip` (legg til -Force for å overskrive)
   - Terminal (Linux/Mac) `zip -r code.zip ./bin/Release/net8.0/publish/*`
1. Deploy ved å kjøre `az functionapp deployment source config-zip -g {YourResourceGroup} -n {YourAppServiceName} --src code.zip` 
1. Hvis du får tilbake `Deployment endpoint responded with status code 202` så er applikasjonen lastet opp og klar til å testes