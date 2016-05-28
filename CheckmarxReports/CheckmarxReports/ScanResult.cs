using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckmarxReports
{
    /// <summary>
    /// The result of the scan.
    /// </summary>
    public class ScanResult
    {
        /// <summary>
        /// Create a <see cref="ScanResult"/>.
        /// </summary>
        /// <param name="ruleName">
        /// The Checkmarx rule name.
        /// </param>
        /// <param name="severity">
        /// The severity. usually "High", "Medium" or "Low".
        /// </param>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <param name="line">
        /// The line number.
        /// </param>
        /// <param name="deepLink">
        /// The URL to the Checkmarx result.
        /// </param>
        /// <param name="status">
        /// The status, usually "New" or "Recurrent".
        /// </param>
        public ScanResult(string ruleName, string severity, string filePath, string line, string deepLink, string status)
        {
            if (string.IsNullOrWhiteSpace(ruleName))
            {
                throw new ArgumentException("Argument is null or whitespace", nameof(ruleName));
            }
            if (string.IsNullOrWhiteSpace(severity))
            {
                throw new ArgumentException("Argument is null or whitespace", nameof(severity));
            }
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Argument is null or whitespace", nameof(filePath));
            }
            if (string.IsNullOrWhiteSpace(line))
            {
                throw new ArgumentException("Argument is null or whitespace", nameof(line));
            }
            if (string.IsNullOrWhiteSpace(deepLink))
            {
                throw new ArgumentException("Argument is null or whitespace", nameof(deepLink));
            }
            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentException("Argument is null or whitespace", nameof(status));
            }

            RuleName = ruleName;
            Severity = severity;
            FilePath = filePath;
            Line = line;
            DeepLink = deepLink;
            Status = status;
        }

        /// <summary>
        /// The Checkmarx rule name.
        /// </summary>
        public string RuleName { get; private set; }

        /// <summary>
        /// The severity. usually "High", "Medium" or "Low".
        /// </summary>
        public string Severity { get; private set; }

        /// <summary>
        /// The file path.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// The line number.
        /// </summary>
        public string Line { get; private set; }

        /// <summary>
        /// The URL to the Checkmarx result.
        /// </summary>
        public string DeepLink { get; private set; }

        /// <summary>
        /// The status, usually "New" or "Recurrent".
        /// </summary>
        public string Status { get; private set; }
    }
}
