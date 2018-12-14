$outputPath = "Output"
$phraseBookFileName = "$PSScriptRoot\$outputPath\GooglePhrasebook.json"

. $PSScriptRoot\ExportGooglePhraseBookChrome.ps1 $phraseBookFileName
. $PSScriptRoot\SyncAnkiWithGooglePhraseBook.ps1 $phraseBookFileName
