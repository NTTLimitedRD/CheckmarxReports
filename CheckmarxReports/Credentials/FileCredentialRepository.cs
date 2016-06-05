using System;

namespace CheckmarxReports.Credentials
{
    /// <summary>
    /// 
    /// </summary>
    public class FileCredentialRepository : ICredentialRepository
    {
        /// <summary>
        /// Save the credentials. Existing credentials, if any, are overwritte.
        /// </summary>
        /// <param name="server">
        /// The server name. This cannot be null, empty or whitespace.
        /// </param>
        /// <param name="userName">
        /// The server name. This cannot be null, empty or whitespace.
        /// </param>
        /// <param name="password">
        /// The password. This cannot be null, empty or whitespace.
        /// </param>
        /// <exception cref="ArgumentException">
        /// No argument can be null, empty or whitespace.
        /// </exception>
        public void Save(string server, string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(server))
            {
                throw new ArgumentException("Argument is null or whitespace", nameof(server));
            }
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("Argument is null or whitespace", nameof(userName));
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Argument is null or whitespace", nameof(password));
            }

            // Get file


            // Save credentials
        }

        /// <summary>
        /// Does the respository contain credentials for <paramref name="server"/>?
        /// </summary>
        /// <param name="server">
        /// The server to check. This cannot be null, empty or whitespace.
        /// </param>
        /// <returns>
        /// <c>True</c> if credentials are stored for that server, <c>false</c> otherwise.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// No argument can be null, empty or whitespace.
        /// </exception>
        public bool Contains(string server)
        {
            if (string.IsNullOrWhiteSpace(server))
            {
                throw new ArgumentException("Argument is null or whitespace", nameof(server));
            }

            return false;
        }

        /// <summary>
        /// Load the stored credentials for the server.
        /// </summary>
        /// <param name="server">
        /// The server name. This cannot be null, empty or whitespace.
        /// </param>
        /// <param name="userName">
        /// Receives the stored user name.
        /// </param>
        /// <param name="password">
        /// Receives the stored password.
        /// </param>
        /// <exception cref="CredentialNotFoundException">
        /// No credentials are stored for that server.
        /// </exception>
        public void Load(string server, out string userName, out string password)
        {
            userName = null;
            password = null;
        }
    }
}
