using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        [Required]
        [JilDirective(Name = "algorithm")]
        public Algorithm Algorithm { get; set; }

        /// <summary>
        /// The user name.
        /// </summary>
        [Required]
        [MinLength(1)]
        [JilDirective(Name = "userName")]
        public string UserName { get; set; }

        /// <summary>
        /// The password.
        /// </summary>
        [Required]
        [MinLength(1)]
        [JilDirective(Name = "password")]
        public string Password { get; set; }

        /// <summary>
        /// The user name initialization vector (IV).
        /// </summary>
        [Required]
        [MinLength(1)]
        [JilDirective(Name = "userNameIV")]
        public string UserNameIv { get; set; }

        /// <summary>
        /// The password initialization vector (IV).
        /// </summary>
        [Required]
        [MinLength(1)]
        [JilDirective(Name = "passwordIV")]
        public string PasswordIv { get; set; }

        /// <summary>
        /// Throw an <see cref="ValidationException"/> if the data is invalid.
        /// </summary>
        public void AssertValid()
        {
            Validator.ValidateObject(this, new ValidationContext(this));
        }
    }
}
