Push-Location (Split-Path $MyInvocation.MyCommand.Path -Parent)

$phraseBook = (Invoke-Expression ".\ImportGooglePhraseBook.ps1 $args").Replace(',,', ',"",')
$phraseBookJson = $phraseBook | ConvertFrom-Json

. .\AnkiConfig.ps1
if (Test-Path ".\AnkiConfig.private.ps1") {
    . .\AnkiConfig.private.ps1
}

Foreach($item in $phraseBookJson[2]) {
    $cardFront = $item[3]
    $cardBack = $item[4]
    . .\AddToAnki.ps1 $ankiConfig.collectionFilePath $ankiConfig.deckName $cardFront $cardBack
    Write-Output "$cardFront`:$cardBack"
}

. .\SyncAnki.ps1 $ankiConfig.collectionFilePath $ankiConfig.webCredentials.userName $ankiConfig.webCredentials.password

Pop-Location