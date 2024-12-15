param (
    $spreadSheetId
)

$outputPath = "Output"
$phraseBookFileName = "$PSScriptRoot/$outputPath/GooglePhrasebook.json"

. $PSScriptRoot\ExportGooglePhraseBookFromSpreadSheet.ps1 $spreadSheetId $phraseBookFileName
# . $PSScriptRoot\SyncAnkiWithGooglePhraseBook.ps1 $phraseBookFileName
