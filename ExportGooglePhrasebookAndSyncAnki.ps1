$outputPath = "Output"
$phraseBookFileName = "$PSScriptRoot\$outputPath\GooglePhrasebook.json"

#. $scriptPath\ExportGooglePhraseBook.ps1 $phraseBookFileName
. $PSScriptRoot\ExportGooglePhraseBookChrome.ps1 $phraseBookFileName
. $PSScriptRoot\SyncAnkiWithGooglePhraseBook.ps1 $phraseBookFileName
