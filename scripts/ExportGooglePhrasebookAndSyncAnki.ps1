#Requires -Version 7.0

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

$ankiConnectPort = 8765
$maxStartAnkiRetryCount = 3

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

function RetryBlock {
    param(
        [Parameter(Mandatory)] [ScriptBlock] $ScriptBlock,
        [int] $RetryCount = 0,
        [int] $DelaySeconds = 3
    )
    $attemptCount = [Math]::Max($RetryCount + 1, 1)
    for ($i = 1; $i -le $attemptCount; $i++) {
        try {
            & $ScriptBlock
            return
        } catch {
            if ($i -ge $attemptCount) {
                throw $_
            } else {
                Write-Warning (
                    "Attempt $i failed: $($_.Exception.Message). " +
                    "Retrying in $DelaySeconds second(s)...")
                Start-Sleep -Seconds $DelaySeconds
            }
        }
    }
}

RetryBlock `
    -ScriptBlock {
        Write-Host "Starting Anki..."
        cmd /c start "" "${Env:LocalAppData}\Programs\Anki\anki.exe"
        Start-Sleep -Seconds 5
        if (!(Get-NetTCPConnection -LocalPort $ankiConnectPort -State Listen -ErrorAction SilentlyContinue)) {
            Write-Host "Anki port $ankiConnectPort not responding; stopping Anki..."
            taskkill.exe /im anki.exe /t /f > $null 2>&1
            throw "Anki port $ankiConnectPort not responding"
        }
    } `
    -RetryCount $maxStartAnkiRetryCount

Write-Host "Anki started successfully and is ready for synchronization." -ForegroundColor Green

Invoke-ExternalCommand dotnet run "--project" (Join-Path $appPathRoot 'UpdateAnki') `
    "--" `
    "-i" $phraseBookFilePath `
    "-l" $logPath

& (Join-Path $repositoryRoot 'external\ExportGooglePhraseBook\scripts\DeleteSpreadSheet.ps1') -spreadSheetId $spreadSheetId
