using System;
using System.Collections.Generic;
using System.Text;

namespace AuthentikUserImporter.Models
{
    public class Configuration
    {
        public string CsvFilePath { get; set; } = "";
        public string AuthentikBaseUrl { get; set; } = "https://auth.mbg-hsw.de";
        public string AuthentikToken { get; set; } = "";
        public string AuthentikGroupId { get; set; } = "";
        public bool EmailPasswordResetLink { get; set; } = true;
        public string EmailStageName { get; set; } = "mbg-initial-password-set";
    }
}
