Push-Location (Split-Path $MyInvocation.MyCommand.Path -Parent)

.\SyncAnkiWithGooglePhraseBook.ps1

& ${env:ProgramFiles(x86)}\Anki\anki.exe

Pop-Location