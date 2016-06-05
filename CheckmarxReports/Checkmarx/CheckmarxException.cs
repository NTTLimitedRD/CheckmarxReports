using System;

namespace CheckmarxReports.Checkmarx
{
    /// <summary>
    /// Base class for Checkmarx exceptions.
    /// </summary>
    public abstract class CheckmarxException : Exception
    {
        /// <summary>
        /// Create a new <see cref="CheckmarxException"/>.
        /// </summary>
        protected CheckmarxException()
        {
            // Do nothing
        }

        /// <summary>
        /// Create a new <see cref="CheckmarxException"/>.
        /// </summary>
        /// <param name="message">
        /// A human-readable error message.
        /// </param>
        protected CheckmarxException(string message)
            : base(message)
        {
            // Do nothing
        }

        /// <summary>
        /// Create a new <see cref="CheckmarxException"/>.
        /// </summary>
        /// <param name="message">
        /// A human-readable error message.
        /// </param>
        /// <param name="inner">
        /// The inner or causing <see cref="Exception"/>.
        /// </param>
        protected CheckmarxException(string message, Exception inner)
            : base(message, inner)
        {
            // Do nothing
        }
    }
}
