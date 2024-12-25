param (
    $fileName
)

. $PSScriptRoot\AnkiUtils.ps1

$outputPath = "Output"
$lastHandledTimeFilePath = "$outputPath\LastHandledTime.txt"
$logFileName = "$outputPath\SyncAnkiWithGooglePhraseBook.log"

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

Function GetDeckName() {
    param (
        $srcLanguage,
        $destLanguage
    )
    $languagesSorted = ($srcLanguage, $destLanguage) | Sort-Object
    $ankiDeckNameTemplate -f $languagesSorted[0], $languagesSorted[1]
}

Function AddCardsToDeck() {
    param (
        $phraseBookJson
    )

    $maxHandledTime = $lastHandledTime;
    $collectionChanged = $false
    Foreach($item in $phraseBookJson[2]) {
        $srcLanguage = $item[1]
        $destLanguage = $item[2]
        $srcPhrase = $item[3]
        $destPhrase = $item[4]
        $time = [long]$item[5]
        $deckName = GetDeckName $srcLanguage $destLanguage
        if (($lastHandledTime -eq 0) -or ($time -gt $lastHandledTime)) {
            $cardModel = $ankiModelNameTemplate -f $srcLanguage, $destLanguage
            AddNoteToAnki $deckName $cardModel $srcPhrase $destPhrase
            if ($?) {
                $global:collectionChanged = $true
            }
            if ($time -gt $maxHandledTime) {
                $maxHandledTime = $time
            }
        }
    }
    $global:lastHandledTime = $maxHandledTime
}

Push-Location (Split-Path $MyInvocation.MyCommand.Path -Parent)

if (!(Test-Path -Path $outputPath))
{
    New-Item $outputPath -type directory | Out-Null
}

try{
    Write-Output "Starting transcription to $logFileName..."
    Start-Transcript -path $logFileName
}
catch {
    Write-Error "Error starting subscription: $_";
}

try {
    try {
        $contentStr = Get-Content $fileName
        $phraseBookJson = $contentStr.Replace(',,', ',"",') | ConvertFrom-Json
        . LoadAnkiConfiguration
        SyncAnkiCollection

        $global:lastHandledTime = LoadLastHandledTime
        $global:collectionChanged = $false

        AddCardsToDeck $phraseBookJson

        if ($global:collectionChanged) {
            SyncAnkiCollection
        }

        SaveLastHandledTime $global:lastHandledTime
    }
    catch {
        Write-Error -ErrorRecord $Error[0]
        throw
    }
    finally {
        try{
            Stop-Transcript
        }
        catch {
            Write-Error "Error stopping subscription: $_";
        }
    }
}
catch {
    $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") | Out-Null
}
Pop-Location
