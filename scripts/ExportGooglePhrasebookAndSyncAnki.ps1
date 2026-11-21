#Requires -Version 7.0

[System.Diagnostics.CodeAnalysis.SuppressMessageAttribute(
    'PSReviewUnusedParameter', 'interactive', Justification = 'Used by the script-level error trap.')]
param (
    [Parameter(Mandatory=$true)]
    [AllowEmptyString()]
    [AllowNull()]
    [string] $spreadSheetId,
    [switch] ${what-if},
    [switch] $interactive = $true
)

$ErrorActionPreference = "Stop"
. (Join-Path $PSScriptRoot 'Common.ps1')
. (Join-Path $PSScriptRoot 'AnkiStartup.ps1')
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$logPath = Join-Path $repositoryRoot 'artifacts\logs'
trap {
    Write-ScriptError -ErrorRecord $_ -LogDirectory $logPath -Interactive $interactive
    exit 1
}

if ([string]::IsNullOrWhiteSpace($spreadSheetId)) {
    throw "Invalid spreadsheet ID: a non-empty value is required."
}

if (${what-if}) {
    Write-Host "Running in what-if mode. No changes will be applied."
    Write-Host "Would export spreadsheet '$spreadSheetId', start Anki, synchronize translations, and delete the spreadsheet."
    return
}

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

$outputPath = Join-Path $repositoryRoot 'artifacts\Output'
$phraseBookFilePath = Join-Path $outputPath 'GooglePhrasebook.json'
$appPathRoot = Join-Path $repositoryRoot 'src'

& (Join-Path $repositoryRoot 'external\ExportGooglePhraseBook\scripts\ExportGooglePhrasebook.ps1') `
    -spreadSheetId $spreadSheetId `
    -outputFilePath $phraseBookFilePath

Start-AnkiWhenNeeded -AnkiPath (Join-Path $Env:LocalAppData 'Programs\Anki\anki.exe')

Invoke-ExternalCommand dotnet run "--project" (Join-Path $appPathRoot 'UpdateAnki') `
    "--" `
    "-i" $phraseBookFilePath `
    "-l" $logPath

& (Join-Path $repositoryRoot 'external\ExportGooglePhraseBook\scripts\DeleteSpreadSheet.ps1') -spreadSheetId $spreadSheetId
