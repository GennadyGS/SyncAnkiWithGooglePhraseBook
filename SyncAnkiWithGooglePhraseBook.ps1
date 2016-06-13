Push-Location (Split-Path $MyInvocation.MyCommand.Path -Parent)

$profileName="GennadyGS"
$collectionFilePath="$([Environment]::GetFolderPath("mydocuments"))\Anki\$profileName\collection.anki2"
$deckName="My words"
$userName="GennadyGS@gmail.com"
$password="9NJZA7Ud"

Invoke-Expression ".\ImportGooglePhraseBook.ps1 $args"

$phraseBook = ((Get-Content .\PhraseBook.json) | Out-String).Replace(',,', ',"",')
$phraseBookJson = $phraseBook | ConvertFrom-Json -Verbose

Foreach($item in $phraseBookJson[2]) {
    $cardFront = $item[3]
    $cardBack = $item[4]
    . .\AddToAnki.ps1 $collectionFilePath $deckName $cardFront $cardBack
    Write-Output "$cardFront`:$cardBack"
}

. .\SyncAnki.ps1 $collectionFilePath $userName $password

Pop-Location

Write-Host "Press any key to continue ..."
$x = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")