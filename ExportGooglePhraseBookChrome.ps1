param (
    $fileName
)

$chromeProfile = "$PSScriptRoot\Output\ChromeProfile\User Data"

. "$PSScriptRoot\GooglePhraseBookExport\GooglePhraseBookExport.exe" $chromeProfile $fileName
