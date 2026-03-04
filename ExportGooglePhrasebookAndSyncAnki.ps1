param (
    [Parameter(Mandatory=$true)] $spreadSheetId,
    [switch] ${what-if}
)

$ankiConnectPort = 8765

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

# Start Anki with retry logic (up to 3 attempts)
$maxRetries = 3
$ankiStarted = $false

for ($attempt = 1; $attempt -le $maxRetries; $attempt++) {
    Write-Host "Starting Anki (attempt $attempt/$maxRetries)..."
    cmd /c start "" "${Env:LocalAppData}\Programs\Anki\anki.exe"

    Start-Sleep -Seconds 5

    if (Get-NetTCPConnection -LocalPort $ankiConnectPort -State Listen -ErrorAction SilentlyContinue) {
        Write-Host "Anki is listening on port $ankiConnectPort" -ForegroundColor Green
        $ankiStarted = $true
        break
    }
    else {
        Write-Host `
            "Anki port $ankiConnectPort not responding (attempt $attempt/$maxRetries); stopping Anki..." `
            -ForegroundColor Yellow
        taskkill.exe /im anki.exe /t /f > $null 2>&1
        Start-Sleep -Seconds 1
    }
}

if (-not $ankiStarted) {
    throw "Failed to start Anki and establish port $ankiConnectPort connection after $maxRetries attempts"
}
Write-Host "Anki started successfully and is ready for synchronization." -ForegroundColor Green

Invoke-ExternalCommand dotnet run "--project" "$appPathRoot/UpdateAnki" `
    "--" `
    "-i" $phraseBookFilePath $(${what-if} ? "--what-if" : "") `
    "-l" $logPath

& $PSScriptRoot/ExportGooglePhraseBook/DeleteSpreadSheet.ps1 -spreadSheetId $spreadSheetId
