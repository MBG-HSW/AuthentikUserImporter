using System;
using System.Collections.Generic;
using System.Text;

namespace AuthentikUserImporter.Models
{
    public class User : CsvUser
    {
        public string Username { get; set; } = "";
        public int UsernameNumber { get; set; } = 0;
        public string Uuid { get; set; } = "";

        public User(CsvUser csvUser)
        {
            FirstName = csvUser.FirstName;
            LastName = csvUser.LastName;
            Email = csvUser.Email;
            PhoneNumber = csvUser.PhoneNumber;
            Path = csvUser.Path;
            Sms = csvUser.Sms;
            SuggestUsername();
        }

        public void SuggestUsername()
        {
            UsernameNumber += 1;
            if (string.IsNullOrWhiteSpace(FirstName))
                return;

            Username = User.ReplaceUmlaute(FirstName).Substring(0, 2).ToLower() + User.ReplaceUmlaute(LastName).Substring(0, 2).ToLower() + UsernameNumber.ToString("00");
        }
        public static string ReplaceUmlaute(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            return input
                .Replace("ä", "ae")
                .Replace("ö", "oe")
                .Replace("ü", "ue")
                .Replace("Ä", "Ae")
                .Replace("Ö", "Oe")
                .Replace("Ü", "Ue")
                .Replace("ß", "ss");
        }

    }
}
