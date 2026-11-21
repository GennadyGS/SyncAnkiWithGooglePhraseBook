function Test-AnkiConnect {
    param(
        [string] $Uri = 'http://127.0.0.1:8765',
        [int] $TimeoutSeconds = 2
    )

    $handler = [System.Net.Http.HttpClientHandler]::new()
    $handler.UseProxy = $false
    $client = [System.Net.Http.HttpClient]::new($handler)
    $client.Timeout = [TimeSpan]::FromSeconds($TimeoutSeconds)
    $content = [System.Net.Http.StringContent]::new(
        '{"action":"version","version":6}',
        [System.Text.Encoding]::UTF8,
        'application/json')
    try {
        $response = $client.PostAsync($Uri, $content).GetAwaiter().GetResult()
        try {
            $response.EnsureSuccessStatusCode() | Out-Null
            $body = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
            $result = $body | ConvertFrom-Json -ErrorAction Stop
            if ($null -eq $result -or
                $null -eq $result.PSObject.Properties['error'] -or
                $null -eq $result.PSObject.Properties['result'] -or
                $null -ne $result.error -or
                ($result.result -isnot [int] -and $result.result -isnot [long]) -or
                $result.result -lt 6) {
                throw "Invalid or unsupported AnkiConnect version response: $body"
            }
        } finally {
            $response.Dispose()
        }
    } finally {
        $content.Dispose()
        $client.Dispose()
    }
}

function Start-AnkiWhenNeeded {
    param(
        [string] $AnkiPath,
        [int] $StartupTimeoutSeconds = 30,
        [int] $PollIntervalSeconds = 1
    )

    try {
        Test-AnkiConnect
        Write-Host "Reusing the responsive Anki instance."
        return
    } catch {
        $lastError = $_.Exception.Message
    }

    Write-Host "Starting Anki..."
    $timer = [System.Diagnostics.Stopwatch]::StartNew()
    Start-Process -FilePath $AnkiPath
    while ($timer.Elapsed.TotalSeconds -lt $StartupTimeoutSeconds) {
        try {
            Test-AnkiConnect
            Write-Host "Anki is ready for synchronization."
            return
        } catch {
            $lastError = $_.Exception.Message
        }

        $remaining = $StartupTimeoutSeconds - $timer.Elapsed.TotalSeconds
        if ($remaining -gt 0) {
            Start-Sleep -Milliseconds ([int]([Math]::Min($PollIntervalSeconds, $remaining) * 1000))
        }
    }

    throw "AnkiConnect did not become ready within $StartupTimeoutSeconds seconds. Last probe error: $lastError"
}
