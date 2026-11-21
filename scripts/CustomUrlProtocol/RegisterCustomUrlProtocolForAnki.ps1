$repositoryRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)

& (Join-Path $repositoryRoot 'external\ExportGooglePhraseBook\scripts\CustomUrlProtocol\RegisterCustomUrlProtocol.ps1') `
    -name "exportGooglePhrasebookToAnki" `
    -description "Export Google Phrasebook to Anki" `
    -scriptPath (Join-Path $PSScriptRoot '..\ExportGooglePhrasebookAndSyncAnkiCustomUrlHandler.ps1')
