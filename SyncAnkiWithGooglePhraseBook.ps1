$lastHandledTimeFilePath = ".\LastHandledTime.txt"

Function LoadAnkiConfiguration() {
    if (Test-Path ".\AnkiConfig.private.ps1") {
        return .\AnkiConfig.private.ps1
    }
    return .\AnkiConfig.ps1
}

Function LoadLastHandledTime() {
    if (!(Test-Path $lastHandledTimeFilePath)) {
        return 0
    }
    return [long](Get-Content $lastHandledTimeFilePath)
}

Function SaveLastHandledTime() {
    param ($lastHandledTime)
    $lastHandledTime | Out-File $lastHandledTimeFilePath
}

Function AddCardsToDeck() {
    param (
        $phraseBookJson
    )

    $maxHandledTime = $lastHandledTime;
    Foreach($item in $phraseBookJson[2]) {
        $cardFront = $item[3]
        $cardBack = $item[4]
        $cardTime = [long]$item[5]
        if ($cardTime -gt $lastHandledTime) {
            Write-Output "Adding card $cardFront`:$cardBack"
            .\AddToAnki.ps1 $ankiConfig.collectionFilePath $ankiConfig.deckName $cardFront $cardBack
            if ($cardTime -gt $maxHandledTime) {
                $maxHandledTime = $cardTime
            }
        }
    }
    $global:lastHandledTime = $maxHandledTime
}

Push-Location (Split-Path $MyInvocation.MyCommand.Path -Parent)

$phraseBookJson = .\ExportGooglePhraseBook.ps1

$lastHandledTime = LoadLastHandledTime

$ankiConfig = LoadAnkiConfiguration

AddCardsToDeck $phraseBookJson $ankiConfig

. .\SyncAnki.ps1 $ankiConfig.collectionFilePath $ankiConfig.webCredentials.userName $ankiConfig.webCredentials.password

SaveLastHandledTime $lastHandledTime

Pop-Location
