param (
    $fileName
)

$scriptPath = $(Get-Location).Path
$chromeProfile = "$scriptPath\Output\ChromeProfile\User Data"

. "$scriptPath\GooglePhraseBookExport\GooglePhraseBookExport.exe" $chromeProfile $fileName
