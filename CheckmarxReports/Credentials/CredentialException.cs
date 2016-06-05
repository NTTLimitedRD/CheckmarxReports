using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckmarxReports.Credentials
{
    /// <summary>
    /// Base class for Checkmarx exceptions.
    /// </summary>
    public abstract class CredentialException : Exception
    {
        /// <summary>
        /// Create a new <see cref="CredentialException"/>.
        /// </summary>
        protected CredentialException()
        {
            // Do nothing
        }

        /// <summary>
        /// Create a new <see cref="CredentialException"/>.
        /// </summary>
        /// <param name="message">
        /// A human-readable error message.
        /// </param>
        protected CredentialException(string message)
            : base(message)
        {
            // Do nothing
        }

        /// <summary>
        /// Create a new <see cref="CredentialException"/>.
        /// </summary>
        /// <param name="message">
        /// A human-readable error message.
        /// </param>
        /// <param name="inner">
        /// The inner or causing <see cref="Exception"/>.
        /// </param>
        protected CredentialException(string message, Exception inner)
            : base(message, inner)
        {
            // Do nothing
        }
    }
}
