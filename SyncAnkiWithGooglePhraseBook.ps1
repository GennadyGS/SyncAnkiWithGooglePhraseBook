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
        $srcLanguage = $item[1]
        $destLanguage = $item[2]
        $srcPhrase = $item[3]
        $destPhrase = $item[4]
        $time = [long]$item[5]
        if ($time -gt $lastHandledTime) {
            $cardModel = $ankiConfig.modelNameTemplate -f $srcLanguage, $destLanguage
            Write-Output "Adding card '$srcPhrase`:$destPhrase' to deck '$ankiConfig.deckName' with model '$cardModel'"
            .\AddToAnki.ps1 $ankiConfig.collectionFilePath $ankiConfig.deckName $srcPhrase $destPhrase $cardModel
            if ($time -gt $maxHandledTime) {
                $maxHandledTime = $time
            }
        }
    }
    $global:lastHandledTime = $maxHandledTime
}

Push-Location (Split-Path $MyInvocation.MyCommand.Path -Parent)

$phraseBookJson = .\ExportGooglePhraseBook.ps1

$global:lastHandledTime = LoadLastHandledTime

$ankiConfig = LoadAnkiConfiguration

AddCardsToDeck $phraseBookJson $ankiConfig

. .\SyncAnki.ps1 $ankiConfig.collectionFilePath $ankiConfig.webCredentials.userName $ankiConfig.webCredentials.password

SaveLastHandledTime $global:lastHandledTime

Pop-Location
