using System;
using System.Collections.Generic;
using System.Text;

namespace AuthentikUserImporter.Models
{
    class ImportResult
    {
        public User User { get; private set; } = null!;
        public bool Succeeded { get; private set; }
        public string? ErrorMessage { get; private set; }

        public static ImportResult Success(User user) => new ImportResult { User = user, Succeeded = true };
        public static ImportResult Failure(User user, string errorMessage) => new ImportResult { User = user, Succeeded = false, ErrorMessage = errorMessage };
    }
}
