using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace ExportGooglePhraseBookFromSpreadSheet;

public static class Program
{
    private const string CredentialFileName = "credential.json";
    private const string PhrasebookRange = "A:D";

    public static void Main(string[] args)
    {
        var spreadsheetId = args[0];
        var outputFileName = args[1];
        ExportPhrasebookToFile(spreadsheetId, outputFileName);
    }

    private static void ExportPhrasebookToFile(string spreadsheetId, string outputFileName)
    {
        var service = CreateSheetsService();

        var request = service.Spreadsheets.Values.Get(spreadsheetId, PhrasebookRange);
        var response = request.Execute();
        IList<IList<object>> values = response.Values;

        if (values is { Count: > 0 })
        {
            foreach (var row in values)
            {
                Console.WriteLine(string.Join(", ", row));
            }
        }
        else
        {
            Console.WriteLine("No data found.");
        }
    }

    private static SheetsService CreateSheetsService()
    {
        var initializer = new BaseClientService.Initializer
        {
            HttpClientInitializer = CreateCredential(),
        };
        return new SheetsService(initializer);
    }

    private static GoogleCredential CreateCredential()
    {
        var scope = SheetsService.Scope.SpreadsheetsReadonly;
        using var stream = new FileStream(CredentialFileName, FileMode.Open, FileAccess.Read);
        return GoogleCredential.FromStream(stream).CreateScoped(scope);
    }
}