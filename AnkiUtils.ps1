Function LoadAnkiConfiguration() {
    . .\AnkiConfig.ps1
    if (Test-Path ".\AnkiConfig.private.ps1") {
        . .\AnkiConfig.private.ps1
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

Function AddNoteToAnki {
    param (
        $deckName,
        $front,
        $back,
        $model
    )

    EnsureAnkiIsRunning
    "Adding note '$front' - '$back' with model '$model' to deck '$deckName'"
    $body =  @{
        action = "addNote";
        version = 6;
        params = @{
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
    } | ConvertTo-Json -Depth 3
    $bodyContent = [System.Text.Encoding]::UTF8.GetBytes($body)
    Invoke-RestMethod -Uri $ankiConnectUrl -Method 'Post' -Body $bodyContent -ContentType 'application/json' `
    | CheckAnkiConnectResult
}

Function SyncAnkiCollection {
    EnsureAnkiIsRunning
    "Sync anki collection"
    $body =  @{
        action = "sync";
        version = 6;
    } | ConvertTo-Json -Depth 3
    Invoke-RestMethod -Uri $ankiConnectUrl -Method 'Post' -Body $body -ContentType 'application/json' `
    | CheckAnkiConnectResult
}
