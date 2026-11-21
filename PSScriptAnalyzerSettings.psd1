@{
    Severity = @('Error', 'Warning')
    ExcludeRules = @(
        # These entry points intentionally provide console output and a custom -what-if contract.
        'PSAvoidUsingWriteHost'
        'PSUseShouldProcessForStateChangingFunctions'
        'PSUseSupportsShouldProcess'
        # Preserve existing script parameters for protocol handlers and unattended callers.
        'PSAvoidDefaultValueSwitchParameter'
    )
}
