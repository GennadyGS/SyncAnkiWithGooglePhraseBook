Function LoadAnkiConfiguration() {
    . .\AnkiConfig.ps1
    if (Test-Path ".\AnkiConfig.private.ps1") {
        . .\AnkiConfig.private.ps1
    }
}

Function AddCardsToDeck() {
    param ($phraseBookJson)

    Foreach($item in $phraseBookJson[2]) {
        $cardFront = $item[3]
        $cardBack = $item[4]
        Write-Output "Adding card $cardFront`:$cardBack"
        . .\AddToAnki.ps1 $ankiConfig.collectionFilePath $ankiConfig.deckName $cardFront $cardBack
    }
}

Push-Location (Split-Path $MyInvocation.MyCommand.Path -Parent)

$phraseBookJson = (& .\ExportGooglePhraseBook.ps1)

LoadAnkiConfiguration

AddCardsToDeck $phraseBookJson

. .\SyncAnki.ps1 $ankiConfig.collectionFilePath $ankiConfig.webCredentials.userName $ankiConfig.webCredentials.password

Pop-Location
