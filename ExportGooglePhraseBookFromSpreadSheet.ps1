param (
    [Parameter(Mandatory=$true)] $spreadSheetId,
    [Parameter(Mandatory=$true)] $outputFileName
)

$appPathRoot = "$PSScriptRoot/src"
$exportGooglePhraseBookFromSpreadSheetAppPath =
    "$appPathRoot/ExportGooglePhraseBookFromSpreadSheet"
$shareGoogleDriveFileAppPath = "$appPathRoot/ShareGoogleDriveFile"

$credentialFilePath = "$exportGooglePhraseBookFromSpreadSheetAppPath/credential.json"
$credential = Get-Content $credentialFilePath -Raw | ConvertFrom-Json

Push-Location $shareGoogleDriveFileAppPath
dotnet run -- -i $spreadSheetId -u $credential.client_email
Pop-Location

Push-Location $exportGooglePhraseBookFromSpreadSheetAppPath
dotnet run -- -i $spreadSheetId -o $outputFileName
Pop-Location
