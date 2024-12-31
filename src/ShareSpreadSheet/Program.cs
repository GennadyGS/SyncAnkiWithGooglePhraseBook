using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace ShareSpreadsheet;

internal static class Program
{
    private const string ApplicationName = "Drive API C# Quickstart";
    private const string SecretFileName = "client_secret.json";
    private static readonly string[] Scopes = [DriveService.Scope.Drive];

    static async Task Main(string[] args)
    {
        var service = await AuthenticateUserAsync();

        const string serviceAccountEmail =
            "googlesheetscalendar@iconic-iridium-374712.iam.gserviceaccount.com";
        const string fileId = "1wmx2Nr8-IwRlHe4OanMxDxt_Jf1XMd7S7YbCdTV8LyY";

        await ShareFileWithServiceAccount(service, fileId, serviceAccountEmail);
    }

    static async Task<DriveService> AuthenticateUserAsync()
    {
        await using var stream = new FileStream(SecretFileName, FileMode.Open, FileAccess.Read);
        const string credPath = "token.json";
        var secretsStream = await GoogleClientSecrets.FromStreamAsync(stream);
        var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            secretsStream.Secrets,
            Scopes,
            "user",
            CancellationToken.None,
            new FileDataStore(credPath, true));

        return new DriveService(
            new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
    }

    static async Task ShareFileWithServiceAccount(
        DriveService service, string fileId, string serviceAccountEmail)
    {
        try
        {
            var permission = new Google.Apis.Drive.v3.Data.Permission
            {
                Type = "user",
                Role = "reader",
                EmailAddress = serviceAccountEmail,
            };

            var request = service.Permissions.Create(permission, fileId);
            request.Fields = "id";
            var result = await request.ExecuteAsync();

            Console.WriteLine(
                $"Successfully shared file (ID: {fileId}) with {serviceAccountEmail}. Permission ID: {result.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sharing file (ID: {fileId}): {ex.Message}");
        }
    }
}