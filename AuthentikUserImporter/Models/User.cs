using System;
using System.Collections.Generic;
using System.Text;

namespace AuthentikUserImporter.Models
{
    public class User : CsvUser
    {
        public string Username { get; set; } = "";
        public int UsernameNumber { get; set; } = 0;
        public string Uuid { get; set; }

        public User(CsvUser csvUser)
        {
            FirstName = csvUser.FirstName;
            LastName = csvUser.LastName;
            Email = csvUser.Email;
            PhoneNumber = csvUser.PhoneNumber;
            Path = csvUser.Path;
            SuggestUsername();
        }

        public void SuggestUsername()
        {
            UsernameNumber += 1;
            if (string.IsNullOrWhiteSpace(FirstName))
                return;

            Username = FirstName.Substring(0, 2).ToLower() + LastName.Substring(0, 2).ToLower() + UsernameNumber.ToString("00");
        }
    }
}
