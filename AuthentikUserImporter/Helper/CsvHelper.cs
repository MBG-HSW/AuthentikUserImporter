using AuthentikUserImporter.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthentikUserImporter.Helper
{
    public class CsvHelper
    {
        public static List<CsvUser> ReadCsv(string path)
        {
            var users = new List<CsvUser>();
            var lines = File.ReadAllLines(path, Encoding.UTF8);
            if (lines.Length < 2) return users;

            char delimiter = DetectDelimiter(lines[0]);

            var headers = SplitCsv(lines[0], delimiter);
            int Idx(params string[] names)
            {
                foreach (var n in names)
                {
                    int i = Array.FindIndex(headers, h => h.Equals(n, StringComparison.OrdinalIgnoreCase));
                    if (i >= 0) return i;
                }
                return -1;
            }

            int iEmail = Idx("email", "e-mail", "mail");
            int iFirstName = Idx("firstname", "first_name", "vorname");
            int iLastName = Idx("lastname", "last_name", "nachname");
            int iPhoneNumber = Idx("phonenumber", "phone_number", "telefon", "telefonnummer", "phone");
            int iSms = Idx("sms", "sms_enabled", "sms aktiv");
            int iPath = Idx("path", "pfad");

            if (iEmail < 0)
            {
                throw new InvalidDataException(
                    "CSV muss mindestens die Spalte 'email' enthalten.");
            }

            for (int i = 1; i < lines.Length; i++)
            {
                var cols = SplitCsv(lines[i], delimiter);
                if (cols.Length < 1) continue;

                string Get(int idx) => idx >= 0 && idx < cols.Length ? cols[idx].Trim() : "";

                var email = Get(iEmail);
                if (email.Length == 0) continue;

                users.Add(new CsvUser
                {
                    Email = email,
                    FirstName = Get(iFirstName),
                    LastName = Get(iLastName),
                    PhoneNumber = Get(iPhoneNumber),
                    Sms = Get(iSms).Equals("true", StringComparison.OrdinalIgnoreCase) || Get(iSms).Equals("1"),
                    Path = Get(iPath).Length > 0 ? Get(iPath) : "users",
                });
            }

            return users;
        }

        static char DetectDelimiter(string headerLine)
        {
            // Zählt Kandidaten und nimmt den häufigsten
            char[] candidates = { ';', ',', '\t' };
            return candidates
                .OrderByDescending(c => headerLine.Count(ch => ch == c))
                .First();
        }

        static string[] SplitCsv(string line, char delimiter)
        {
            var result = new List<string>();
            bool inQuotes = false;
            var current = new StringBuilder();

            foreach (char c in line)
            {
                if (c == '"') { inQuotes = !inQuotes; continue; }
                if (c == delimiter && !inQuotes) { result.Add(current.ToString()); current.Clear(); continue; }
                current.Append(c);
            }
            result.Add(current.ToString());
            return result.ToArray();
        }

    }
}
