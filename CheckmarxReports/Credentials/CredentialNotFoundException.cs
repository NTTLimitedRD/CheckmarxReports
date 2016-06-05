using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckmarxReports.Credentials
{
    /// <summary>
    /// No credentials were stored for the specified server.
    /// </summary>
    public class CredentialNotFoundException: Exception
    {
        /// <summary>
        /// Create a new <see cref="CredentialNotFoundException"/> saying credentials
        /// could not be located for <paramref name="server"/>.
        /// </summary>
        /// <param name="server">
        /// The server to 
        /// </param>
        public CredentialNotFoundException(string server)
            : base($"Credentials not found for server '{server ?? "(null)"}'")
        {
            // Do nothing
        }
    }
}
