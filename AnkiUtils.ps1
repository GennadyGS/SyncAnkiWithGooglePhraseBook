Function LoadAnkiConfiguration() {
    . $PSScriptRoot\AnkiConfig.ps1
    if (Test-Path ".\AnkiConfig.private.ps1") {
        . $PSScriptRoot\AnkiConfig.private.ps1
    }
}

Function EnsureAnkiIsRunning {
    if (!(Get-Process anki -ErrorAction SilentlyContinue)) {
        Start-Process $ankiExecutablePath -WindowStyle Minimized
        Start-Sleep -Seconds 5
    }
}

Function CheckAnkiConnectResult {
    [CmdletBinding()]
    param (
        [parameter(ValueFromPipeline)]
        $result
    )
    if ($result.error) {
        Write-Error $result.error
    }
}

Function InvokeAnkiCommand([string] $action, [PSObject] $params) {
    $command = @{
        action = $action;
        version = 6;
    }
    if ($params) {
        $command["params"] = $params;
    }
    $commandJson = $command | ConvertTo-Json -Depth 100
    $bodyContent = [System.Text.Encoding]::UTF8.GetBytes($commandJson)
    Invoke-RestMethod -Uri $ankiConnectUrl -Method POST -Body $bodyContent
    | CheckAnkiConnectResult
}

Function AddNoteToAnki {
    param (
        $deckName,
        $model,
        $front,
        $back
    )

    EnsureAnkiIsRunning
    "Adding note '$front' - '$back' with model '$model' to deck '$deckName'"
    $params = @{
        note = @{
            deckName = $deckName;
            modelName = $model;
            fields = @{
                Front = $front;
                Back = $back;
            };
            options = @{
                allowDuplicate = $false;
                duplicateScope = "deck";
            };
        }
    }
    InvokeAnkiCommand "addNote" $params
}

Function SyncAnkiCollection {
    EnsureAnkiIsRunning
    "Sync anki collection"
    InvokeAnkiCommand "sync"
}
