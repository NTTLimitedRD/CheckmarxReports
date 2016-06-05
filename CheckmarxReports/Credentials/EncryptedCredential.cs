using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;

namespace CheckmarxReports.Credentials
{
    /// <summary>
    /// Used to serialize credentials.
    /// </summary>
    public class EncryptedCredential
    {
        /// <summary>
        /// The algorithm (used for cryptographic agility).
        /// </summary>
        [JilDirective(Name = "algorithm")]
        public Algorithm Algorithm { get; set; }

        /// <summary>
        /// The user name.
        /// </summary>
        [JilDirective(Name = "userName")]
        public string UserName { get; set; }

        /// <summary>
        /// The password.
        /// </summary>
        [JilDirective(Name = "password")]
        public string Password { get; set; }

        /// <summary>
        /// The user name initialization vector (IV).
        /// </summary>
        [JilDirective(Name = "userNameIV")]
        public string UserNameIv { get; set; }

        /// <summary>
        /// The password initialization vector (IV).
        /// </summary>
        [JilDirective(Name = "passwordIV")]
        public string PasswordIv { get; set; }

        /// <summary>
        /// Throw an <see cref="ArgumentException"/> if the data is invalid.
        /// </summary>
        public void AssertValid()
        {
            if (string.IsNullOrWhiteSpace(UserName))
            {
                throw new ArgumentException("UserName cannot be null, empty or whitespace", nameof(UserName));
            }
            if (string.IsNullOrWhiteSpace(UserNameIv))
            {
                throw new ArgumentException("UserNameIV cannot be null, empty or whitespace", nameof(UserNameIv));
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                throw new ArgumentException("Password cannot be null, empty or whitespace", nameof(Password));
            }
            if (string.IsNullOrWhiteSpace(PasswordIv))
            {
                throw new ArgumentException("PasswordIv cannot be null, empty or whitespace", nameof(PasswordIv));
            }
        }
    }
}
