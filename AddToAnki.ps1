Push-Location (Split-Path $MyInvocation.MyCommand.Path -Parent)

$env:PYTHONIOENCODING="utf8"
. .\SetPythonEnvVars.ps1

# Write-Output "Executing $PythonExe addToAnki\add_to_anki-2.0.12.py $args..."
& $PythonExe addToAnki\add_to_anki-2.0.12.py $args

if ($lastExitCode -gt 1) {
    throw "Adding card to anki collection failed with exit code $lastExitCode"
}
exit $lastExitCode

Pop-Location
