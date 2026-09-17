using AuthentikUserImporter.Helper;
using AuthentikUserImporter.Models;

namespace AuthentikUserImporter;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("╔══════════════════════════════════════════════╗");
        Console.WriteLine("║       Authentik CSV User Import Tool         ║");
        Console.WriteLine("╚══════════════════════════════════════════════╝");
        Console.WriteLine();

        Configuration configuration = new Configuration();

        AskForParameters(configuration);

        await ExecuteImport(configuration);
    }

    static void AskForParameters(Configuration config)
    {
        config.CsvFilePath = ReadWithDefault("Enter the path to the CSV file", config.CsvFilePath);
        config.AuthentikBaseUrl = ReadWithDefault("Enter the Authentik base URL", config.AuthentikBaseUrl);
        config.AuthentikToken = ReadWithDefault("Enter the Authentik token", config.AuthentikToken);
        config.AuthentikGroupId = ReadWithDefault("Enter the Authentik group ID", config.AuthentikGroupId);
        config.EmailPasswordResetLink = bool.Parse(ReadWithDefault("Should the password reset link be sent via email? (true/false)", config.EmailPasswordResetLink.ToString()));
        if (config.EmailPasswordResetLink)
            config.EmailStageName = ReadWithDefault("Enter the email stage name", config.EmailStageName);

        if (!ConfirmParameters())
        {
            AskForParameters(config);
        }
    }

    static string ReadWithDefault(string prompt, string currentValue)
    {
        Console.Write($"{prompt} ({currentValue}): ");
        var input = Console.ReadLine();
        return string.IsNullOrWhiteSpace(input) ? currentValue : input;
    }

    static bool ConfirmParameters()
    {
        Console.Write("Do you want to proceed with these parameters? (y/n): ");
        var input = Console.ReadLine();
        return input?.ToLower() == "y";
    }

    static async Task ExecuteImport(Configuration config)
    {
        var results = new List<ImportResult>();

        List<CsvUser> csvUsers;
        try
        {
            csvUsers = CsvHelper.ReadCsv(config.CsvFilePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fatal error reading CSV file: {ex.Message}");
            return;
        }

        var httpClient = HttpHelper.BuildHttpClient(config.AuthentikBaseUrl, config.AuthentikToken);
        Console.WriteLine();
        Console.WriteLine($"Found {csvUsers.Count} user(s) in CSV. Starting import...");
        Console.WriteLine();

        int current = 0;
        foreach (var csvUser in csvUsers)
        {
            current++;
            var user = new User(csvUser);
            Console.WriteLine($"[{current}/{csvUsers.Count}] Importing user: {user.FirstName} {user.LastName}");

            try
            {
                while (await AuthentikHelper.UsernameExists(httpClient, user.Username))
                {
                    Console.WriteLine($"  Username {user.Username} already exists. Suggesting a new username...");
                    user.SuggestUsername();
                }

                user.Uuid = await AuthentikHelper.CreateUser(httpClient, user);
                Console.WriteLine($"  Created with UUID: {user.Uuid}, Username: {user.Username}");

                await AuthentikHelper.AddUserToGroup(httpClient, user.Uuid, config.AuthentikGroupId);

                if (config.EmailPasswordResetLink)
                {
                    await AuthentikHelper.TriggerRecoveryEmail(httpClient, user.Uuid, config.EmailStageName);
                    Console.WriteLine("  Password reset email triggered.");
                }

                results.Add(ImportResult.Success(user));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Error importing {user.FirstName} {user.LastName}: {ex.Message}");
                results.Add(ImportResult.Failure(user, ex.Message));
            }

            Console.WriteLine();
        }

        PrintSummary(results);
    }

    static void PrintSummary(List<ImportResult> results)
    {
        var successCount = results.Count(r => r.Succeeded);
        var failureCount = results.Count - successCount;

        Console.WriteLine("╔══════════════════════════════════════════════╗");
        Console.WriteLine("║                   Summary                    ║");
        Console.WriteLine("╚══════════════════════════════════════════════╝");
        Console.WriteLine($"Total:   {results.Count}");
        Console.WriteLine($"Success: {successCount}");
        Console.WriteLine($"Failed:  {failureCount}");
        Console.WriteLine();

        if (successCount > 0)
        {
            Console.WriteLine("Successfully imported:");
            foreach (var r in results.Where(r => r.Succeeded))
            {
                Console.WriteLine($"  - {r.User.FirstName} {r.User.LastName} (Username: {r.User.Username}, UUID: {r.User.Uuid})");
            }
            Console.WriteLine();
        }

        if (failureCount > 0)
        {
            Console.WriteLine("Failed imports:");
            foreach (var r in results.Where(r => !r.Succeeded))
            {
                Console.WriteLine($"  - {r.User.FirstName} {r.User.LastName}: {r.ErrorMessage}");
            }
            Console.WriteLine();
        }
    }
}