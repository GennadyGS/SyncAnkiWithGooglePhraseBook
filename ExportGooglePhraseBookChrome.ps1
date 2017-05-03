param (
    $fileName
)

$chromeProfile = "$PSScriptRoot\Output\ChromeProfile\User Data"
if (!(Test-Path $chromeProfile)) {
    $sourceChromeProfilePath = "$env:LocalAppData\Google\Chrome\User Data\Default"
    $targetChromeProfilePath = "$chromeProfile\Default"
	. taskkill /im "chrome.exe" /f /t
    Write-Output "Copying chrome profile from "$sourceChromeProfilePath" to "$targetChromeProfilePath"..."
	Copy-Item $sourceChromeProfilePath $targetChromeProfilePath  -Recurse -ErrorAction Stop
}
. "$PSScriptRoot\GooglePhrasebookExport\GooglePhraseBookExport.ps1" $chromeProfile $fileName
