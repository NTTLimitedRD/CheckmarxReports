using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.ServiceModel.Security;
using System.Text;
using CheckmarxReports.CommandLineOptions;
using Jil;

namespace CheckmarxReports.Credentials
{
    /// <summary>
    /// Save or load credentials to a configuration file.
    /// </summary>
    /// <remarks>
    /// Credentials are encrypted using the Windows Data Protection API. Each
    /// user name and password has a unique IV generated when the credential is
    /// saved.
    /// </remarks>
    public class FileCredentialRepository : ICredentialRepository
    {
        /// <summary>
        /// Create a new <see cref="FileCredentialRepository"/> using
        /// the default configuration file.
        /// </summary>
        public FileCredentialRepository()
            : this(GetDefaultFilePath())
        {
            // Do nothing    
        }

        /// <summary>
        /// Create a new <see cref="FileCredentialRepository"/>.
        /// </summary>
        /// <param name="filePath">
        /// The file name to use.
        /// </param>
        public FileCredentialRepository(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException(
                    "Argument cannot be null, empty or whitespace", nameof(filePath));
            }

            FilePath = filePath;
            Scope = DataProtectionScope.CurrentUser;
        }

        /// <summary>
        /// The configuration file path.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// The scope for encryption keys.
        /// </summary>
        public DataProtectionScope Scope { get; }

        /// <summary>
        /// Default config file.
        /// </summary>
        /// <returns>
        /// The default file for storing credentials.
        /// </returns>
        public static string GetDefaultFilePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "credentials.json");
        }

        /// <summary>
        /// Save the credentials. Existing credentials, if any, are overwritten.
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

            Dictionary<string, EncryptedCredential> credentials;

            credentials = LoadCredentials(FilePath);
            credentials[server] = Encrypt(userName, password);
            SaveCredentials(FilePath, credentials);
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

            return LoadCredentials(FilePath).ContainsKey(server);
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
        /// <seealso cref="Contains"/>
        public void Load(string server, out string userName, out string password)
        {
            EncryptedCredential credential;

            if (LoadCredentials(FilePath).TryGetValue(server, out credential))
            {
                Decrypt(credential, out userName, out password);
            }
            else
            {
                throw new CredentialNotFoundException(server);
            }
        }

        /// <summary>
        /// Load credentials from <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">
        /// The 
        /// </param>
        /// <returns>
        /// A mapping of server to credential.
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// The file <paramref name="filePath"/> does not exist.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="filePath"/> cannot be null, empty or whitespace.
        /// </exception>
        internal Dictionary<string, EncryptedCredential> LoadCredentials(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Cannot be null, empty or whitespace", nameof(filePath));
            }

            Dictionary<string, EncryptedCredential> result;

            try
            {
                using (StreamReader streamReader = new StreamReader(filePath, Encoding.UTF8))
                {
                    result = JSON.Deserialize<Dictionary<string, EncryptedCredential>>(streamReader);
                }
            }
            catch (FileNotFoundException)
            {
                result = new Dictionary<string, EncryptedCredential>();
            }

            return result;
        }

        /// <summary>
        /// Save credentials to <paramref name="filePath"/>;
        /// </summary>
        /// <param name="filePath">
        /// The path to write files out to. This cannot be null, empty or whitespace.
        /// </param>
        /// <param name="credentials">
        /// The credentials to save.
        /// </param>
        internal void SaveCredentials(string filePath, Dictionary<string, EncryptedCredential> credentials)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Cannot be null, empty or whitespace", nameof(filePath));
            }

            using (StreamWriter streamWriter = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                JSON.Serialize(credentials, streamWriter);
            }
        }

        /// <summary>
        /// Descrypt the user name and password supplied in the given <see cref="EncryptedCredential"/>.
        /// </summary>
        /// <param name="credential">
        /// The credential to decrypt.
        /// </param>
        /// <param name="userName">
        /// Receives the user name.
        /// </param>
        /// <param name="password">
        /// Receives the password.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="credential"/> cannot be null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="credential"/> is not valid.
        /// </exception>
        internal void Decrypt(EncryptedCredential credential, out string userName, out string password)
        {
            if (credential == null)
            {
                throw new ArgumentNullException(nameof(credential));
            }
            credential.AssertValid();

            byte[] userNamePlainText;
            byte[] passwordPlainText;

            userNamePlainText = ProtectedData.Unprotect(
                Convert.FromBase64String(credential.UserName), 
                Convert.FromBase64String(credential.UserNameIv),
                Scope);
            passwordPlainText = ProtectedData.Unprotect(
                Convert.FromBase64String(credential.Password), 
                Convert.FromBase64String(credential.PasswordIv),
                Scope);

            userName = new string(Encoding.UTF8.GetChars(userNamePlainText));
            password = new string(Encoding.UTF8.GetChars(passwordPlainText));
        }

        /// <summary>
        /// Encrypt the supplied <paramref name="userName"/> and <paramref name="password"/> into the given <see cref="EncryptedCredential"/>.
        /// </summary>
        /// <param name="userName">
        /// The user name to encrypt. This cannot be null, empty or whitespace.
        /// </param>
        /// <param name="password">
        /// The password to encrypt. This cannot be null, empty or whitespace.
        /// </param>
        /// <returns>
        /// A <see cref="EncryptedCredential"/> containing the encrypted username and password.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// No argument can be null, empty or whitespace.
        /// </exception>
        internal EncryptedCredential Encrypt(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("Cannot be null, empty or whitespace", nameof(userName));
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Cannot be null, empty or whitespace", nameof(password));
            }

            byte[] userNameIv;
            byte[] passwordIv;

            using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
            {
                userNameIv = new byte[20];
                randomNumberGenerator.GetBytes(userNameIv);
                passwordIv = new byte[20];
                randomNumberGenerator.GetBytes(passwordIv);
            }

            return new EncryptedCredential
            {
                UserNameIv = Convert.ToBase64String(userNameIv),
                PasswordIv = Convert.ToBase64String(passwordIv),
                UserName = Convert.ToBase64String(
                    ProtectedData.Protect(
                        Encoding.UTF8.GetBytes(userName),
                        userNameIv,
                        Scope)),
                Password = Convert.ToBase64String(
                    ProtectedData.Protect(
                        Encoding.UTF8.GetBytes(password),
                        passwordIv,
                        Scope))
            };
        }
    }
}
