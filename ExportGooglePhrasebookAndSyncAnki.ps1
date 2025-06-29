param (
    [Parameter(Mandatory=$true)] $spreadSheetId,
    [switch] ${what-if}
)

$ErrorActionPreference = "Stop"
if (${what-if}) {
    Write-Host "Running in what-if mode. No changes will be applied."
}

$outputPath = "Output"
$logPath = "$PSScriptRoot/logs"
$phraseBookFileName = "$PSScriptRoot/$outputPath/GooglePhrasebook.json"

$appPathRoot = "$PSScriptRoot/src/src"
$exportGooglePhraseBookFromSpreadSheetAppPath =
    "$appPathRoot/ExportGooglePhraseBookFromSpreadSheet"

$credentialFilePath = "$exportGooglePhraseBookFromSpreadSheetAppPath/credential.json"
$credential = Get-Content $credentialFilePath -Raw | ConvertFrom-Json

Push-Location "$appPathRoot/GoogleDriveFileControl"
dotnet run -- -a Share -i $spreadSheetId -u $credential.client_email
Pop-Location

dotnet run --project $exportGooglePhraseBookFromSpreadSheetAppPath `
    -- -i $spreadSheetId -o $phraseBookFileName

cmd /c start "" "${Env:LocalAppData}\Programs\Anki\anki.exe"

dotnet run --project "$appPathRoot/UpdateAnki" `
    -- `
    -i $phraseBookFileName $(${what-if} ? "--what-if" : "") `
    -l $logPath

Push-Location "$appPathRoot/GoogleDriveFileControl"
dotnet run -- -a Delete -i $spreadSheetId
Pop-Location

