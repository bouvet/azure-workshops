$appname=$args[0]
$rg=$args[1]

function Deploy_FunctionApp{
    param([String]$fa, [String]$rg)

    Write-Host "Deploy av FunctionApp"
    write-host $fa
    write-host $rg

    cd ./AzureWorkshop/AzureWorkshopFunctionApp
    dotnet publish -c Release 
    Compress-Archive -Path ./bin/Release/net8.0/publish/* -DestinationPath ./code.zip -Force

    az functionapp deployment source config-zip -g $rg -n $fa --src code.zip
    
    cd ../..
    Write-Host "Deploy av FunctionApp: completed"
}

az login 
Deploy_FunctionApp -fa $appname -rg $rg

