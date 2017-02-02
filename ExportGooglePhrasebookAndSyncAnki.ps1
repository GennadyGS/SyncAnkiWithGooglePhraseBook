$outputPath = "Output"
$phraseBookFileName = "$outputPath\GooglePhrasebook.json"
$scriptPath = $(Get-Location).Path

#. $scriptPath\ExportGooglePhraseBook.ps1 $phraseBookFileName
. $scriptPath\SyncAnkiWithGooglePhraseBook.ps1 $phraseBookFileName
