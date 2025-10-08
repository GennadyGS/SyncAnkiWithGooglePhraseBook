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

$outputPath = "$PSScriptRoot/Output"
$logPath = "$PSScriptRoot/logs"
$phraseBookFilePath = "$outputPath/GooglePhrasebook.json"
$appPathRoot = "$PSScriptRoot/src/src"

& $PSScriptRoot/ExportGooglePhraseBook/ExportGooglePhrasebook.ps1 `
    -spreadSheetId $spreadSheetId `
    -outputFilePath $phraseBookFilePath

cmd /c start "" "${Env:LocalAppData}\Programs\Anki\anki.exe"

Invoke-ExternalCommand dotnet run "--project" "$appPathRoot/UpdateAnki" `
    "--" `
    "-i" $phraseBookFilePath $(${what-if} ? "--what-if" : "") `
    "-l" $logPath

& $PSScriptRoot/ExportGooglePhraseBook/DeleteSpreadSheet.ps1 -spreadSheetId $spreadSheetId
