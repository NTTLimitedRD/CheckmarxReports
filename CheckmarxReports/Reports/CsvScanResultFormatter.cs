using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CheckmarxReports.CommandLineOptions;

namespace CheckmarxReports.Reports
{
    /// <summary>
    /// Format scan results as a comma-seperavted variable (CSV) file.
    /// </summary>
    public class CsvScanResultFormatter: IReportResultFormatter<ScanResult>
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
        public void Format(IList<ScanResult> reportResults, TextWriter output, CheckmarxReportOptions options, string username)
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

            WriteHeader(output);
            foreach (ScanResult scanResult in reportResults.OrderBy(sr => sr.ProjectName).ThenBy(sr => sr.RuleName))
            {
                WriteRow(output, scanResult);
            }
        }

        private void WriteHeader(TextWriter output)
        {
            output.WriteLine("Project,Rule,Severity,File,Line,DeepLink,Status,FalsePositive");
        }

        private void WriteRow(TextWriter output, ScanResult scanResult)
        {
            string[] result =
            {
                scanResult.ProjectName,
                scanResult.RuleName,
                scanResult.Severity.ToString(),
                scanResult.FilePath,
                scanResult.Line.ToString(),
                scanResult.DeepLink.ToString(),
                scanResult.Status.ToString(),
                scanResult.FalsePositive.ToString()
            };
            output.WriteLine(string.Join(",", result.Select(EscapeForCsv)));
        }

        private string EscapeForCsv(string input)
        {
            return input.Replace(",", "\\,");
        }
    }
}
