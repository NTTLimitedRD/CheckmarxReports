using System;

namespace CheckmarxReports.Reports
{
    /// <summary>
    /// The result of the scan.
    /// </summary>
    public class ScanResult : IEquatable<ScanResult>
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
        /// The severity. usually "High", "Medium" or "Low".
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
        /// The status, usually "New" or "Recurrent".
        /// </param>
        /// <param name="falsePositive">
        /// True if a Checkmarx user has marked this as "Not Vulnerable", false otherwise.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="projectName"/>, <paramref name="ruleName"/> and <paramref name="filePath"/> cannot be null, empty or whitespace.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="deepLink"/> cannot be null.
        /// </exception>
        public ScanResult(string projectName, string ruleName, Severity severity, string filePath, uint line, Uri deepLink, Status status, bool falsePositive)
        {
            if (string.IsNullOrWhiteSpace(projectName))
            {
                throw new ArgumentException("Argument is null or whitespace", nameof(projectName));
            }
            if (string.IsNullOrWhiteSpace(ruleName))
            {
                throw new ArgumentException("Argument is null or whitespace", nameof(ruleName));
            }
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Argument is null or whitespace", nameof(filePath));
            }
            if (deepLink == null)
            {
                throw new ArgumentNullException(nameof(deepLink));
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
        public string ProjectName { get; private set; }

        /// <summary>
        /// The Checkmarx rule name.
        /// </summary>
        public string RuleName { get; private set; }

        /// <summary>
        /// The severity. usually "High", "Medium" or "Low".
        /// </summary>
        public Severity Severity { get; private set; }

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
        public Status Status { get; private set; }

        /// <summary>
        /// True if a Checkmarx user has marked this as "Not Vulnerable", false otherwise.
        /// </summary>
        public bool FalsePositive { get; private set; }

        /// <summary>
        /// Equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ScanResult other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(ProjectName, other.ProjectName) && string.Equals(RuleName, other.RuleName) && Severity == other.Severity && string.Equals(FilePath, other.FilePath) && Line == other.Line && Equals(DeepLink, other.DeepLink) && Status == other.Status && FalsePositive == other.FalsePositive;
        }

        /// <summary>
        /// Equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ScanResult) obj);
        }

        /// <summary>
        /// Get hash code.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (ProjectName != null ? ProjectName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (RuleName != null ? RuleName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (int) Severity;
                hashCode = (hashCode*397) ^ (FilePath != null ? FilePath.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (int) Line;
                hashCode = (hashCode*397) ^ (DeepLink != null ? DeepLink.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (int) Status;
                hashCode = (hashCode*397) ^ FalsePositive.GetHashCode();
                return hashCode;
            }
        }
    }
}
