#Requires -Version 7.0
#Requires -Modules @{ ModuleName = 'PSScriptAnalyzer'; RequiredVersion = '1.24.0' }

$ErrorActionPreference = 'Stop'
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$diagnostics = @(Invoke-ScriptAnalyzer -Path $PSScriptRoot -Recurse `
    -Settings (Join-Path $repositoryRoot 'PSScriptAnalyzerSettings.psd1'))
if ($diagnostics.Count -gt 0) {
    $diagnostics | Format-Table -AutoSize | Out-Host
    throw "PowerShell analysis failed with $($diagnostics.Count) diagnostic(s)."
}

Write-Host 'PowerShell analysis passed.'
