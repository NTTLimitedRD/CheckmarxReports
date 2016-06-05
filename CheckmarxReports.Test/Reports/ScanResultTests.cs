using System;
using CheckmarxReports.Reports;
using NUnit.Framework;

namespace CheckmarxReports.Test.Reports
{
    [TestFixture]
    public class ScanResultTests
    {
        [Test]
        public void Ctor()
        {
            ScanResult scanResult;
            const string projectName = "project";
            const string ruleName = "rule";
            const Severity severity = Severity.Medium;
            const string filePath = "file";
            const uint line = 1234;
            Uri deepLink = new Uri("https://checkmarx/path/to/result");
            const Status status = Status.Recurrent;
            const bool falsePositive = true;

            scanResult = new ScanResult(projectName, ruleName, severity, filePath, line, deepLink, status, falsePositive);
            Assert.That(scanResult, Has.Property("ProjectName").EqualTo(projectName));
            Assert.That(scanResult, Has.Property("RuleName").EqualTo(ruleName));
            Assert.That(scanResult, Has.Property("Severity").EqualTo(severity));
            Assert.That(scanResult, Has.Property("FilePath").EqualTo(filePath));
            Assert.That(scanResult, Has.Property("Line").EqualTo(line));
            Assert.That(scanResult, Has.Property("DeepLink").EqualTo(deepLink));
            Assert.That(scanResult, Has.Property("Status").EqualTo(status));
            Assert.That(scanResult, Has.Property("FalsePositive").EqualTo(falsePositive));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Ctor_InvalidProjectName(string projectName)
        {
            Assert.That(() => new ScanResult(projectName, "rule", Severity.High, "filePath", 5678, new Uri("http://a/b"), Status.Recurrent, true),
                Throws.ArgumentException.And.Property("ParamName").EqualTo("projectName"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Ctor_InvalidRuleName(string ruleName)
        {
            Assert.That(() => new ScanResult("projectName", ruleName, Severity.High, "filePath", 5678, new Uri("http://a/b"), Status.Recurrent, true),
                Throws.ArgumentException.And.Property("ParamName").EqualTo("ruleName"));
        }

        [Test]
        public void Ctor_NullDeepLink()
        {
            Assert.That(() => new ScanResult("projectName", "ruleName", Severity.High, "filePath", 5678, null, Status.Recurrent, true),
                Throws.ArgumentNullException.And.Property("ParamName").EqualTo("deepLink"));
        }
    }
}
