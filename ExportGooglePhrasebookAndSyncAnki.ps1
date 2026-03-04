param (
    [Parameter(Mandatory=$true)] $spreadSheetId,
    [switch] ${what-if}
)

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

Invoke-ExternalCommand dotnet run "--project" "$appPathRoot/UpdateAnki" `
    "--" `
    "-i" $phraseBookFilePath $(${what-if} ? "--what-if" : "") `
    "-l" $logPath

& $PSScriptRoot/ExportGooglePhraseBook/DeleteSpreadSheet.ps1 -spreadSheetId $spreadSheetId
