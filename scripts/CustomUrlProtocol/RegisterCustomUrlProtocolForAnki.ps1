#Requires -Version 7.0

param (
    [switch] ${what-if}
)

if (${what-if}) {
    Write-Host "Would register the 'exportGooglePhrasebookToAnki' URL protocol. No registry changes will be applied."
    return
}

$repositoryRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)

& (Join-Path $repositoryRoot 'external\ExportGooglePhraseBook\scripts\CustomUrlProtocol\RegisterCustomUrlProtocol.ps1') `
    -name "exportGooglePhrasebookToAnki" `
    -description "Export Google Phrasebook to Anki" `
    -scriptPath (Join-Path $PSScriptRoot '..\ExportGooglePhrasebookAndSyncAnkiCustomUrlHandler.ps1')
