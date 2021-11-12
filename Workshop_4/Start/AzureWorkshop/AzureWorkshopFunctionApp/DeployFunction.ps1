function Deploy_FunctionApp{

    write-host $Env:Fa
    write-host $Env:Rg

    dotnet publish -c Release 
    Compress-Archive -Path .\bin\Release\netcoreapp3.1\publish\* -DestinationPath .\code.zip -Force

    az functionapp deployment source config-zip -g $Env:Rg -n $Env:Fa --src code.zip
}

Deploy_FunctionApp