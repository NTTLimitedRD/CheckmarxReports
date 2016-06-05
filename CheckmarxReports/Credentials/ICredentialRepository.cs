using System;

namespace CheckmarxReports.Credentials
{
    /// <summary>
    /// Credential repository interface.
    /// </summary>
    public interface ICredentialRepository
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
        void Save(string server, string userName, string password);

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
        bool Contains(string server);

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
        void Load(string server, out string userName, out string password);
    }
}