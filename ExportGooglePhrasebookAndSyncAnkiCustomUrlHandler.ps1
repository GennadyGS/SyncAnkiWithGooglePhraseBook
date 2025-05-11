param (
    [Parameter(Mandatory=$true)] $url
)

Add-Type -AssemblyName System.Web

$queryString = [System.Web.HttpUtility]::ParseQueryString($url.Split('?')[1])
$spreadSheetId = $queryString["spreadSheetId"]
& $PSScriptRoot/ExportGooglePhrasebookAndSyncAnki.ps1 $spreadSheetId
