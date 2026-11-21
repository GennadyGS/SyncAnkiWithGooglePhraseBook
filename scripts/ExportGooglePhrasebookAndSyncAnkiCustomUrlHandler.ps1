#Requires -Version 7.0

param (
    [Parameter(Mandatory=$true)]
    [AllowEmptyString()]
    [AllowNull()]
    [string] $url,
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

$parsedUrl = $null
if (-not [Uri]::TryCreate($url, [UriKind]::Absolute, [ref]$parsedUrl)) {
    throw "Invalid URL: an absolute URL is required."
}

if ($parsedUrl.Scheme -ne 'exportGooglePhrasebookToAnki') {
    throw "Invalid URL scheme: expected 'exportGooglePhrasebookToAnki'."
}

$queryString = [System.Web.HttpUtility]::ParseQueryString($parsedUrl.Query)
$spreadSheetIds = $queryString.GetValues('spreadSheetId')
if ($null -eq $spreadSheetIds -or $spreadSheetIds.Count -ne 1 -or
    [string]::IsNullOrWhiteSpace($spreadSheetIds[0])) {
    throw "Invalid URL: exactly one non-empty 'spreadSheetId' query parameter is required."
}

if (!${what-if}) {
    Remove-Item Env:PLATFORM -ErrorAction SilentlyContinue
}

$LASTEXITCODE = 0
& (Join-Path $PSScriptRoot 'ExportGooglePhrasebookAndSyncAnki.ps1') `
    -spreadSheetId $spreadSheetIds[0] -what-if:${what-if} -interactive:$interactive
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}
