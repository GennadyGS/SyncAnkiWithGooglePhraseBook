function Write-ScriptError {
    param (
        [Parameter(Mandatory=$true)]
        [System.Management.Automation.ErrorRecord] $ErrorRecord,
        [Parameter(Mandatory=$true)]
        [string] $LogDirectory,
        [bool] $Interactive = $true
    )

    $message = "ERROR: $($ErrorRecord.Exception.Message)"
    $details = "$message$([Environment]::NewLine)$($ErrorRecord | Out-String)$($ErrorRecord.ScriptStackTrace)"
    [Console]::Error.WriteLine($details)

    try {
        New-Item -Path $LogDirectory -ItemType Directory -Force -ErrorAction Stop | Out-Null
        $errorLogPath = Join-Path $LogDirectory 'ScriptErrors.log'
        Add-Content -LiteralPath $errorLogPath `
            -Value "[$(Get-Date -Format o)] $details" -Encoding utf8 -ErrorAction Stop
        Write-Host "Error logged to '$errorLogPath'."
    } catch {
        [Console]::Error.WriteLine("ERROR: Could not write the script error log: $($_.Exception.Message)")
    }

    if ($Interactive -and [Environment]::UserInteractive -and -not [Console]::IsInputRedirected) {
        try {
            Read-Host "Press enter to exit..." | Out-Null
        } catch {
            [Console]::Error.WriteLine("ERROR: Could not wait for console input: $($_.Exception.Message)")
        }
    }
}
