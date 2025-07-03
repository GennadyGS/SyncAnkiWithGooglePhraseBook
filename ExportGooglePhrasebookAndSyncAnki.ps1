param (
    [Parameter(Mandatory=$true)] $spreadSheetId,
    [switch] ${what-if}
)

function Invoke-ExternalCommand {
    param(
        [Parameter(Mandatory=$true, Position=0)]
        [string]$Command,

        [Parameter(ValueFromRemainingArguments=$true)]
        [string[]]$Arguments
    )

    & $Command @Arguments
    if ($LastExitCode -ne 0) {
        throw "Command '$Command' failed with exit code $LastExitCode"
    }
}

$ErrorActionPreference = "Stop"
trap {
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Press enter to exit..."
    Read-Host
    exit 1
}

if (${what-if}) {
    Write-Host "Running in what-if mode. No changes will be applied."
}

$outputPath = "Output"
$logPath = "$PSScriptRoot/logs"
$phraseBookFileName = "$PSScriptRoot/$outputPath/GooglePhrasebook.json"

$appPathRoot = "$PSScriptRoot/src/src"
$exportGooglePhraseBookFromSpreadSheetAppPath =
    "$appPathRoot/ExportGooglePhraseBookFromSpreadSheet"
$googleDriveFileControlAppPath = "$appPathRoot/GoogleDriveFileControl"

$credentialFilePath = "$exportGooglePhraseBookFromSpreadSheetAppPath/credential.json"
$credential = Get-Content $credentialFilePath -Raw | ConvertFrom-Json

Invoke-ExternalCommand dotnet run "--project" $googleDriveFileControlAppPath `
    "--" "-a" Share "-i" $spreadSheetId "-u" $credential.client_email

Invoke-ExternalCommand dotnet run "--project" $exportGooglePhraseBookFromSpreadSheetAppPath `
    "--" "-i" $spreadSheetId "-o" $phraseBookFileName

cmd /c start "" "${Env:LocalAppData}\Programs\Anki\anki.exe"

Invoke-ExternalCommand dotnet run "--project" "$appPathRoot/UpdateAnki" `
    "--" `
    "-i" $phraseBookFileName $(${what-if} ? "--what-if" : "") `
    "-l" $logPath

Invoke-ExternalCommand dotnet run "--project" $googleDriveFileControlAppPath `
    "--no-build" `
    "--" "-a" Delete "-i" $spreadSheetId
