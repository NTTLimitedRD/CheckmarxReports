using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckmarxReports
{
    /// <summary>
    /// Thrown when the Checkmarx web services return an error.
    /// </summary>
    public class CheckmarxErrorException: CheckmarxException
    {
        /// <summary>
        /// Create a new <see cref="CheckmarxErrorException"/>.
        /// </summary>
        public CheckmarxErrorException()
        {
            // Do nothing
        }

        /// <summary>
        /// Create a new <see cref="CheckmarxErrorException"/>.
        /// </summary>
        /// <param name="message">
        /// A human-readable error message.
        /// </param>
        public CheckmarxErrorException(string message)
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
        public CheckmarxErrorException(string message, Exception inner)
            : base(message, inner)
        {
            // Do nothing
        }
    }
}
