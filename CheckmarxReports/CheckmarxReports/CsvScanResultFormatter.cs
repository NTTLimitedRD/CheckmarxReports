using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckmarxReports
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
        public void Format(IList<ScanResult> reportResults, TextWriter output, string server, string username)
        {
            if (reportResults == null)
            {
                throw new ArgumentNullException(nameof(reportResults));
            }
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }
            if (string.IsNullOrWhiteSpace(server))
            {
                throw new ArgumentException("Cannot be null, empty or whitespace", nameof(server));
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
