param (
    $fileName
)

$chromeProfile = "$PSScriptRoot\Output\ChromeProfile\User Data"

. "$PSScriptRoot\GooglePhrasebookExport\GooglePhraseBookExport.ps1" $chromeProfile $fileName
