param (
    $name = "exportGooglePhrasebookToAnki",
    $description = "Export Google Phrasebook to Anki"
)

$url = "pwsh.exe -noexit -WindowStyle Minimized `"$PSScriptRoot/../ExportGooglePhrasebookAndSyncAnkiCustomUrlHandler.ps1`" `"%1`""
$classesPath = "HKCU:\SOFTWARE\Classes"
New-Item -Path "$classesPath" -Name "$name" -Force

New-Item -Path "$classesPath\$name" -Name "shell" -Force
Set-ItemProperty -Path "$classesPath\$name" -Name "(Default)" -Value "$description"
New-ItemProperty -Path "$classesPath\$name" -Name "URL Protocol" -Value "" -PropertyType String -Force

New-Item -Path "$classesPath\$name\shell" -Name "open" -Force
New-Item -Path "$classesPath\$name\shell\open" -Name "command" -Force
Set-ItemProperty -Path "$classesPath\$name\shell\open\command" -Name "(Default)" -Value "$url"
