using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckmarxReports
{
    /// <summary>
    /// Convert the report results to something human readable.
    /// </summary>
    public interface IReportResultFormatter<TReportResult>
    {
        /// <summary>
        /// Format the report results.
        /// </summary>
        /// <param name="reportResults">
        /// The report results. This cannot be null.
        /// </param>
        /// <param name="output">
        /// The <see cref="TextWriter"/> to write the results to. This cannot be null.
        /// </param>
        /// <param name="server">
        /// The Checkmarx server the report was run on. This cannot be null, empty or whitespace.
        /// </param>
        /// <param name="username">
        /// The user the report was run by. This cannot be null, empty or whitespace.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// No argument can be null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="server"/> and <paramref name="username"/> cannot be null, empty or whitespace.
        /// </exception>
        void Format(IList<TReportResult> reportResults, TextWriter output, string server, string username);
    }
}
