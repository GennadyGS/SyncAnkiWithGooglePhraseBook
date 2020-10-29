param (
    $fileName
)

$chromeProfile = "$PSScriptRoot\Output\ChromeProfile"
$chromeProfileUserData = "$chromeProfile\User Data"
if (!(Test-Path $chromeProfileUserData)) {
    $sourceChromeProfilePath = "$env:LocalAppData\Google\Chrome\User Data\Default"
    $targetChromeProfilePath = "$chromeProfileUserData\Default"
	. taskkill /im "chrome.exe" /f /t
    Write-Output "Copying chrome profile from "$sourceChromeProfilePath" to "$targetChromeProfilePath"..."
	Copy-Item $sourceChromeProfilePath $targetChromeProfilePath  -Recurse -ErrorAction Stop
}
. "$PSScriptRoot\GooglePhrasebookExport\GooglePhraseBookExport.ps1" $chromeProfile $fileName
