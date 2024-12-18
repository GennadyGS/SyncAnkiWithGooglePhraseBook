using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace ShareSpreadSheet;

internal static class Program
{
    // Scopes for Google Drive API
    private const string ApplicationName = "Drive API C# Quickstart";
    private static readonly string[] Scopes = [DriveService.Scope.Drive];

    static async Task Main(string[] args)
    {
        // Authenticate and create a Drive service
        var service = await AuthenticateUserAsync();

        // Service account email
        const string serviceAccountEmail =
            "googlesheetscalendar@iconic-iridium-374712.iam.gserviceaccount.com";
        const string fileId = "1wmx2Nr8-IwRlHe4OanMxDxt_Jf1XMd7S7YbCdTV8LyY";

        await ShareFileWithServiceAccount(service, fileId, serviceAccountEmail);
    }

    static async Task<DriveService> AuthenticateUserAsync()
    {
        // Path to credentials.json (OAuth 2.0 credentials file)
        await using var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read);
        const string credPath = "token.json";
        var secretsStream = await GoogleClientSecrets.FromStreamAsync(stream);
        var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            secretsStream.Secrets,
            Scopes,
            "user",
            CancellationToken.None,
            new FileDataStore(credPath, true));

        // Create Drive API service
        return new DriveService(new BaseClientService.Initializer()
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
            // Create a new permission
            var permission = new Google.Apis.Drive.v3.Data.Permission
            {
                Type = "user",  // Sharing with a specific user
                Role = "reader", // Access level (reader, writer, etc.)
                EmailAddress = serviceAccountEmail
            };

            // Call the permissions.create method
            var request = service.Permissions.Create(permission, fileId);
            request.Fields = "id"; // Only return the ID of the permission
            var result = await request.ExecuteAsync();

            Console.WriteLine($"Successfully shared file (ID: {fileId}) with {serviceAccountEmail}. Permission ID: {result.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sharing file (ID: {fileId}): {ex.Message}");
        }
    }
}