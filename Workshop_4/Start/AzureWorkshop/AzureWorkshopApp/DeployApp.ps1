$Env:Rg = "" # Resource Group
$Env:Fa = "" # Function App

function Deploy_WebApp{

    write-host $Env:Wa
    write-host $Env:Rg

    dotnet publish -c Release 
    Compress-Archive -Path .\bin\Release\net8.0\publish\* -DestinationPath .\code.zip -Force

    az functionapp deployment source config-zip -g $Env:Rg -n $Env:Wa --src code.zip
}

Deploy_WebApp