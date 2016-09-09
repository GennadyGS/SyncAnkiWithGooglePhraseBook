Function LoadConfig() {
    if (Test-Path .\GoogleConfig.private.ps1) {
        return .\GoogleConfig.private.ps1
    }
    return .\GoogleConfig.ps1
}

$googleConfig = LoadConfig

$googleAccountsUrl="https://accounts.google.com"
$loginUrl = "$googleAccountsUrl/ServiceLogin"
$authUrl = "$googleAccountsUrl/ServiceLoginAuth"
$googleTranslateUrl = "https://translate.google.com"
$googlePhraseBookUrlTemplate = "https://translate.google.com/translate_a/sg?client=t&cm=g&hl=en&xt="

$galxPattern = 'GALX.*?value=\"(.*?)\"\>'
$keyPattern = "USAGE\='(.+?)'"

$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession

$loginResponse = Invoke-WebRequest $loginUrl -WebSession $session
if (!($loginResponse -match $galxPattern)) { throw "Invalid login response." }
$galx = $matches[1]

$authParams = @{
    Email = $googleConfig.userName; 
    Passwd = $googleConfig.password;
    GALX = $galx;
    PersistentCookie = "yes";
    bgresponse = "js_disabled";
    continue = $googleTranslateUrl
}

$authResponse = Invoke-WebRequest $authUrl -Method POST -Body $authParams -WebSession $session
if (!($authResponse -match $keyPattern)) { 
    New-Item -ItemType Directory -Force -Path Output
    $authResponse | Out-File Output\AuthResponse.txt -Force
    $authResponse.Content | Out-File Output\AuthResponse.html -Force
    throw "Invalid auth response." 
}
$key=$matches[1]

$googlePhraseBookUrl = "$googlePhraseBookUrlTemplate$key"
$phrasebookResponse = Invoke-WebRequest $googlePhraseBookUrl -Method POST -WebSession $session

$contentStr = $phrasebookResponse.Content
$contentStr | Out-File .\GooglePhrasebook.json -Encoding utf8
$contentStr.Replace(',,', ',"",') | ConvertFrom-Json
