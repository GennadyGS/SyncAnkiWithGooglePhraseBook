param (
    [Parameter(Mandatory=$true)] $spreadSheetId,
    [switch] ${what-if}
)

$outputPath = "Output"
$phraseBookFileName = "$PSScriptRoot/$outputPath/GooglePhrasebook.json"

$appPathRoot = "$PSScriptRoot/src/src"
$exportGooglePhraseBookFromSpreadSheetAppPath =
    "$appPathRoot/ExportGooglePhraseBookFromSpreadSheet"
$shareGoogleDriveFileAppPath = "$appPathRoot/ShareGoogleDriveFile"
$updateAnkiAppPath = "$appPathRoot/UpdateAnki"

$credentialFilePath = "$exportGooglePhraseBookFromSpreadSheetAppPath/credential.json"
$credential = Get-Content $credentialFilePath -Raw | ConvertFrom-Json

Push-Location $shareGoogleDriveFileAppPath
dotnet run -- -i $spreadSheetId -u $credential.client_email
Pop-Location

Push-Location $exportGooglePhraseBookFromSpreadSheetAppPath
dotnet run -- -i $spreadSheetId -o $phraseBookFileName
Pop-Location

dotnet run --project $updateAnkiAppPath -- -i $phraseBookFileName $(${what-if}?"--what-if" : "")
