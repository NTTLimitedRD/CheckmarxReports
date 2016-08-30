using System;
using System.Collections.Generic;
using System.IO;
using CheckmarxReports.CommandLineOptions;

namespace CheckmarxReports.Reports
{
    /// <summary>
    /// Concatenate the strings.
    /// </summary>
    public class TextStringFormatter: IReportResultFormatter<string>
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
        /// <param name="options">
        /// Command line options. This cannot be null.
        /// </param>
        /// <param name="username">
        /// The user the report was run by. This cannot be null, empty or whitespace.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// No argument can be null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="username"/> cannot be null, empty or whitespace.
        /// </exception>
        public void Format(IList<string> reportResults, TextWriter output, CheckmarxReportOptions options, string username)
        {
            if (reportResults == null)
            {
                throw new ArgumentNullException(nameof(reportResults));
            }
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Cannot be null, empty or whitespace", nameof(username));
            }

            foreach (string result in reportResults)
            {
                output.WriteLine(result);
            }
        }
    }
}
