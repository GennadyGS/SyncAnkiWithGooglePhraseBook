Push-Location (Split-Path $MyInvocation.MyCommand.Path -Parent)

$env:PYTHONIOENCODING="utf8"
. .\SetPythonEnvVars.ps1

& $PythonExe  addToAnki\add_to_anki-2.0.12.py $args
if (!($?)) { exit 1 }

Pop-Location
