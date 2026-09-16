using AuthentikUserImporter.Helper;

namespace AuthentikUserImporter;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("╔══════════════════════════════════════════════╗");
        Console.WriteLine("║       Authentik CSV User Import Tool         ║");
        Console.WriteLine("╚══════════════════════════════════════════════╝");
        Console.WriteLine();
        /*
        if (args.Length == 0)
        {
            Console.WriteLine("No arguments provided. Please provide the required arguments.");
            Console.WriteLine("Usage: AuthentikUserImporter <csvFilePath> <authentikBaseUrl> <authentikToken>");
            return;
        }
        if (args.Length < 3)
        {
            Console.WriteLine("Usage: AuthentikUserImporter <csvFilePath> <authentikBaseUrl> <authentikToken>");
            return;
        }*/
        /*
        var csvFilePath = args[0];
        var authentikBaseUrl = args[1];
        var authentikToken = args[2];
        */
        var csvFilePath = @"C:\Users\JosuaL\Desktop\benutzer_beispiel.csv";
        var authentikBaseUrl = "https://auth.mbg-hsw.de";
        var authentikToken = "hItI3H4SBTbabbhQ4fbgt3RqEQYQ6aX5q7bkvOo3AZnv8IlqeNOKMgUfJPoC";
        try
        {
            var csvUsers = CsvHelper.ReadCsv(csvFilePath);
            var httpClient = HttpHelper.BuildHttpClient(authentikBaseUrl, authentikToken);
            foreach (var csvUser in csvUsers)
            {
                var user = new Models.User(csvUser);
                Console.WriteLine($"Importing user: {user.FirstName} {user.LastName}");
                while(await AuthentikHelper.UsernameExists(httpClient, user.Username))
                {
                    Console.WriteLine($"Username {user.Username} already exists. Suggesting a new username...");
                    user.SuggestUsername();
                }
                user.Uuid = await AuthentikHelper.CreateUser(httpClient, user);
                Console.WriteLine($"User {user.FirstName} {user.LastName} created with UUID: {user.Uuid} and Username {user.Username}");
                AuthentikHelper.AddUserToGroup(httpClient, user.Uuid, "8ee1215d-f602-4ad3-a17b-15a78518c0d1").Wait();
                AuthentikHelper.TriggerRecoveryEmail(httpClient, user.Uuid, "mbg-initial-password-set").Wait();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}