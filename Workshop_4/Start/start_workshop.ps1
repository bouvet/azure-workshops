$rg = "";

function Get-RamdomCharacters
{
    param([int]$length)
    @(97..122) | Get-Random -Count $length | % {[char]$_}
}

function CreateVariable{
    param([String]$name, [int]$maxLength)

    $nameLength = $maxLength-$name.length
    $temp = $name + (Get-RamdomCharacters -length $nameLength)
    @($temp.Replace(' ',''))
}

function GetParameters{
    $path = "./AzureWorkshopInfrastruktur/AzureWorkshopInfrastruktur/azuredeploy.parameters.json"
    $deploy = (Get-Content -Path $path | Out-String | convertfrom-json)
    @($deploy)
}

function SetDeployParameters{
    param([String]$name, [int]$maxLength)
    $deploy = GetParameters

    $wa = (CreateVariable -name $name -maxLength $maxLength) + "-wa"
    $deploy.parameters.webSiteName.value = $wa;

    $sa = (CreateVariable -name $name -maxLength $maxLength) + "sa"
    $deploy.parameters.storageAccountName.value = $sa;
    #$parameters.Item("storageAccountName").Item("value") = $wa;

    $ai = (CreateVariable -name $name -maxLength $maxLength) + "-ai"
    $deploy.parameters.appInsightsName.value = $ai;
    #$parameters.Item("appInsightsName").Item("value") = $wa;

    $fa = (CreateVariable -name $name -maxLength $maxLength) + "-fa"
    $deploy.parameters.functionAppName.value = $fa;
    #$parameters.Item("functionAppName").Item("value") = $wa;

    $sb = (CreateVariable -name $name -maxLength $maxLength) + "sb"
    $deploy.parameters.serviceBusName.value = $sb;
    #$parameters.Item("serviceBusName").Item("value") = $wa;

    $path = "./AzureWorkshopInfrastruktur/AzureWorkshopInfrastruktur/azuredeploy.parameters.dev.json"
    Set-Content -Path $path ($deploy | convertto-json -depth 10)
    
    $obj = @{
        wa=$wa
        fa=$fa
        }
    #$obj.wa = $wa
    #$obj.fa = $fa
    @($obj)
}

function Run_AzureInfra
{
    param([String]$rg)

    az login

    $basePath = "./AzureWorkshopInfrastruktur/AzureWorkshopInfrastruktur/"
    $azuredeploy = $basePath + "azuredeploy.json"
    $parameters = $basePath + "azuredeploy.parameters.dev.json"

    az account set --subscription "Azureskolen"
    
    $output = az group create --location westeurope --name $rg
    if(!$output){
        Write-Error "Kunne ikke lage resourcegroup. Prøv på nytt"
        return 
    }
    else{
        Write-Host $output 
    }

    $deployment = CreateVariable -name "deployment-" -maxLength 15
    $output = az deployment group create --name $deployment --resource-group $rg --template-file $azuredeploy --parameters $parameters
    if(!$output){
        Write-Error "Kunne ikke deploye infrastruktur. Se $deployment"
        return 
    }
    else{
        Write-Host $output
    }
}

function Deploy_WebApp{
    param([String]$wa, [String]$rg)

    write-host $wa
    write-host $rg

    cd ./AzureWorkshop/AzureWorkshopApp
    dotnet publish -c Release 
    Compress-Archive -Path .\bin\Release\netcoreapp3.1\publish\* -DestinationPath .\code.zip -Force

    az functionapp deployment source config-zip -g $rg -n $wa --src code.zip

    cd ../..
}

function Deploy_FunctionApp{
    param([String]$fa, [String]$rg)

    write-host $fa
    write-host $rg

    cd ./AzureWorkshop/AzureWorkshopFunctionApp
    dotnet publish -c Release 
    Compress-Archive -Path .\bin\Release\netcoreapp3.1\publish\* -DestinationPath .\code.zip -Force

    az functionapp deployment source config-zip -g $rg -n $fa --src code.zip
    
    cd ../..
}

function Start_Workshop{
    #$name = Read-host "Vennligst oppgi ditt navn"
    $name = "Petter Abrahamsen"
    $name = $name.Replace(' ','').ToLower()
    $maxLength = 20

    if ($name.length -gt 20){
        $name = $name.Substring(0,18)
    }
    if ($name.length -gt 15){
        $maxLength = 22
    }

    #write-host $name

    $rg = (CreateVariable -name $name -maxLength $maxLength) +"-rg"
    Write-Host "Se etter resourcegroup i portal: $rg"
    Write-Host 'Referanser til resourcegroup videre kan bruke environmentvariabelen: $Env:Rg'

    $obj = SetDeployParameters -name $name -maxLength $maxLength

    $Env:Rg = $rg
    $Env:Wa = $obj.wa
    $Env:Fa = $obj.fa

    Run_AzureInfra -rg $rg
    Deploy_WebApp -wa $obj.wa -rg $rg
    Deploy_FunctionApp -fa $obj.fa -rg $rg

    Write-Host "Se etter resourcegroup i portal: $rg"
    Write-Host 'Referanser til resourcegroup videre kan bruke environmentvariabelen: $Env:Rg'
}


Start_Workshop