Push-Location (Split-Path $MyInvocation.MyCommand.Path -Parent)

$env:PYTHONPATH=".\anki"
. .\SetPythonEnvVars.ps1

& $PythonExe addToAnki\sync_anki.py $args
if (!($?)) { exit 1 }

Pop-Location
