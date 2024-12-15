param (
    $spreadSheetId,
    $outputFileName
)

dotnet run --project $PSScriptRoot/src/ExportGooglePhraseBookFromSpreadSheet `
    -- $spreadSheetId $outputFileName

