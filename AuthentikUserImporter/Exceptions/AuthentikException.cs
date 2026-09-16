using System;
using System.Collections.Generic;
using System.Text;

namespace AuthentikUserImporter.Exceptions
{
    internal class AuthentikException : Exception
    {
        public bool IsConflict { get; }
        public AuthentikException(string message, bool isConflict = false)
            : base(message) => IsConflict = isConflict;
    }
}
