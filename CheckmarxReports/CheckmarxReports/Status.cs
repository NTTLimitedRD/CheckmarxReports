using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckmarxReports
{
    /// <summary>
    /// Whether the result has been seen previously.
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// This scan was the first time the result was found.
        /// </summary>
        New,
        /// <summary>
        /// This result has been seen previously.
        /// </summary>
        Recurrent
    }
}
