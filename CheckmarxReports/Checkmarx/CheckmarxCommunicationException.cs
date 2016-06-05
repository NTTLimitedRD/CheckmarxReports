using System;

namespace CheckmarxReports.Checkmarx
{
    /// <summary>
    /// Thrown when an error occurs communicating with the Checkmarx server.
    /// </summary>
    public class CheckmarxCommunicationException: CheckmarxException
    {
        /// <summary>
        /// Create a new <see cref="CheckmarxErrorException"/>.
        /// </summary>
        public CheckmarxCommunicationException()
        {
            // Do nothing
        }

        /// <summary>
        /// Create a new <see cref="CheckmarxErrorException"/>.
        /// </summary>
        /// <param name="message">
        /// A human-readable error message.
        /// </param>
        public CheckmarxCommunicationException(string message)
            : base(message)
        {
            // Do nothing
        }

        /// <summary>
        /// Create a new <see cref="CheckmarxErrorException"/>.
        /// </summary>
        /// <param name="message">
        /// A human-readable error message.
        /// </param>
        /// <param name="inner">
        /// The inner or causing <see cref="Exception"/>.
        /// </param>
        public CheckmarxCommunicationException(string message, Exception inner)
            : base(message, inner)
        {
            // Do nothing
        }
    }
}
