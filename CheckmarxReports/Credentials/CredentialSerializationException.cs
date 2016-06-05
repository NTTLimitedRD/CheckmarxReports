using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckmarxReports.Credentials
{
    /// <summary>
    /// Thrown when a credential serialziatoin or deserialization fails.
    /// </summary>
    public class CredentialSerializationException: CredentialException
    {
        /// <summary>
        /// Create a new <see cref="CredentialSerializationException"/>.
        /// </summary>
        public CredentialSerializationException()
        {
            // Do nothing
        }

        /// <summary>
        /// Create a new <see cref="CredentialSerializationException"/>.
        /// </summary>
        /// <param name="message">
        /// A human-readable error message.
        /// </param>
        public CredentialSerializationException(string message)
            : base(message)
        {
            // Do nothing
        }

        /// <summary>
        /// Create a new <see cref="CredentialSerializationException"/>.
        /// </summary>
        /// <param name="message">
        /// A human-readable error message.
        /// </param>
        /// <param name="inner">
        /// The inner or causing <see cref="Exception"/>.
        /// </param>
        public CredentialSerializationException(string message, Exception inner)
            : base(message, inner)
        {
            // Do nothing
        }
    }
}
