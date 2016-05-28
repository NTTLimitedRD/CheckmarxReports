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
        /// <param name="projectName">
        /// The name of the Checkmarx project this scan occurred on. This cannot be null, empty or whitespace.
        /// </param>
        /// <param name="ruleName">
        /// The Checkmarx rule name. This cannot be null, empty or whitespace.
        /// </param>
        /// <param name="severity">
        /// The severity. usually "High", "Medium" or "Low". This cannot be null, empty or whitespace.
        /// </param>
        /// <param name="filePath">
        /// The file path. This cannot be null, empty or whitespace.
        /// </param>
        /// <param name="line">
        /// The line number.
        /// </param>
        /// <param name="deepLink">
        /// The URL to the Checkmarx result.
        /// </param>
        /// <param name="status">
        /// The status, usually "New" or "Recurrent". This cannot be null, empty or whitespace.
        /// </param>
        /// <param name="falsePositive">
        /// True if a Checkmarx user has marked this as "Not Vulnerable", false otherwise.
        /// </param>
        public ScanResult(string projectName, string ruleName, string severity, string filePath, uint line, Uri deepLink, string status, bool falsePositive)
        {
            if (string.IsNullOrWhiteSpace(projectName))
            {
                throw new ArgumentException("Argument is null or whitespace", nameof(projectName));
            }
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
            if (deepLink == null)
            {
                throw new ArgumentNullException(nameof(deepLink));
            }
            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentException("Argument is null or whitespace", nameof(status));
            }

            ProjectName = projectName;
            RuleName = ruleName;
            Severity = severity;
            FilePath = filePath;
            Line = line;
            DeepLink = deepLink;
            Status = status;
            FalsePositive = falsePositive;
        }

        /// <summary>
        /// The name of the Checkmarx project this scan occurred on.
        /// </summary>
        public string ProjectName { get; set; }

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
        public uint Line { get; private set; }

        /// <summary>
        /// The URL to the Checkmarx result.
        /// </summary>
        public Uri DeepLink { get; private set; }

        /// <summary>
        /// The status, usually "New" or "Recurrent".
        /// </summary>
        public string Status { get; private set; }

        /// <summary>
        /// True if a Checkmarx user has marked this as "Not Vulnerable", false otherwise.
        /// </summary>
        public bool FalsePositive { get; set; }
    }
}
