$Env:Rg = "rcbufynxszparovbwghm-rg"
$Env:Fa = "rcbgaojvluxzcrqbfmyw-fa"

function Deploy_FunctionApp{

    write-host $Env:Fa
    write-host $Env:Rg

    dotnet publish -c Release --interactive
    $Path = Resolve-Path -Path .\bin\Release\net8.0\publish -Relative

    Compress-Archive -Path $Path\* -DestinationPath .\code.zip -Force

    az functionapp deployment source config-zip -g $Env:Rg -n $Env:Fa --src code.zip
}

Deploy_FunctionApp