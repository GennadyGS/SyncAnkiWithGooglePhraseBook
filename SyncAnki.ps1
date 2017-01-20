Push-Location (Split-Path $MyInvocation.MyCommand.Path -Parent)

$env:PYTHONPATH=".\anki"
. .\SetPythonEnvVars.ps1

& $PythonExe addToAnki\sync_anki.py $args

if ($lastExitCode -gt 1) {
    throw "Synchronization of anki collection failed with exit code $lastExitCode"
}
exit $lastExitCode

Pop-Location
