using CommandLine;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using ShareGoogleDriveFile.Models;

namespace ShareGoogleDriveFile;

internal static class Program
{
    private const int ErrorExitCode = 2;
    private const string SecretFileName = "client_secret.json";
    private static readonly string[] Scopes = [DriveService.Scope.Drive];

    public static async Task<int> Main(string[] args)
    {
        try
        {
            return await RunCommandLineParserAsync(args);
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync(e.ToString());
            return ErrorExitCode;
        }
    }

    private static async Task<int> RunCommandLineParserAsync(string[] args) =>
        (await Parser.Default.ParseArguments<CommandLineOptions>(args)
            .WithParsedAsync(async opt =>
            {
                await ShareFileAsync(opt.FileId, opt.UserEmail);
            }))
        .WithNotParsed(errors =>
        {
            foreach (var error in errors)
            {
                Console.WriteLine($"Command line parser error: {error}");
            }
        })
        .MapResult(_ => 0, _ => 1);

    private static async Task<Permission> ShareFileAsync(string fileId, string userEmail)
    {
        try
        {
            var service = await AuthenticateUserAsync();
            var result = await ShareFileAsync(service, fileId, userEmail);
            Console.WriteLine(
                $"Successfully shared file (ID: {fileId}) with user {userEmail}. " +
                $"Permission ID: {result.Id}");
            return result;
        }
        catch (Exception e)
        {
            throw new AggregateException(
                $"Error sharing file (ID: {fileId}) with user {userEmail}", e);
        }
    }

    private static async Task<DriveService> AuthenticateUserAsync()
    {
        await using var stream = new FileStream(SecretFileName, FileMode.Open, FileAccess.Read);
        var secretsStream = await GoogleClientSecrets.FromStreamAsync(stream);
        var credential = await GoogleWebAuthorizationBroker
            .AuthorizeAsync(secretsStream.Secrets, Scopes, "user", CancellationToken.None);

        var assemblyFullName = typeof(Program).Assembly.GetName().Name;
        return new DriveService(
            new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = assemblyFullName,
            });
    }

    private static async Task<Permission> ShareFileAsync(
        DriveService service, string fileId, string userEmail)
    {
        var permission = new Permission
        {
            Type = "user",
            Role = "reader",
            EmailAddress = userEmail,
        };

        var request = service.Permissions.Create(permission, fileId);
        request.Fields = "id";
        return await request.ExecuteAsync();
    }
}
