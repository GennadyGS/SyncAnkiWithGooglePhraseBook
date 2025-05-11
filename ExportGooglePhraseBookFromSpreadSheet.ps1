param (
    [Parameter(Mandatory=$true)] $spreadSheetId,
    [Parameter(Mandatory=$true)] $outputFileName
)

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
dotnet run -- -i $spreadSheetId -o $outputFileName
Pop-Location

dotnet run --project $updateAnkiAppPath -- -i $outputFileName --what-if
