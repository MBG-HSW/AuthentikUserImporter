namespace AuthentikUserImporter.Models
{
    public class CsvUser
    {
        public string FirstName { get; init; } = "";
        public string LastName { get; init; } = "";
        public string Email { get; init; } = "";
        public string PhoneNumber { get; init; } = "";
        public bool Sms { get; init; } = false;
        public string Path { get; init; } = "users";
        public bool EmailPasswordResetLink { get; init; } = true;
    }
}
