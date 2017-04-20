$outputPath = "Output"
$phraseBookFileName = "$PSScriptRoot\$outputPath\GooglePhrasebook.json"

#. $PSScriptRoot\ExportGooglePhraseBook.ps1 $phraseBookFileName
. $PSScriptRoot\ExportGooglePhraseBookChrome.ps1 $phraseBookFileName
. $PSScriptRoot\SyncAnkiWithGooglePhraseBook.ps1 $phraseBookFileName
