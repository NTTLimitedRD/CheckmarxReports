using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckmarxReports
{

    /// <summary>
    /// Report output format.
    /// </summary>
    public enum OutputFormat
    {
        /// <summary>
        /// A styled, human-readable HTML report.
        /// </summary>
        Html,
        /// <summary>
        /// A CSV intended for further manipulation or processing.
        /// </summary>
        Csv
    }
}
