using System;
using System.Collections.Generic;
using System.Text;
using CheckmarxReports.Checkmarx;
using CheckmarxReports.CxSDKWebService;
using CheckmarxReports.Reports;
using Moq;
using NUnit.Framework;

namespace CheckmarxReports.Test.Reports
{
	[TestFixture]
    public class NotFalsePositiveResultsReportRunnerTests
    {
		[Test]
	    public void Run_NullICheckmarxApiSession()
		{
            Assert.That(() => new NotFalsePositiveResultsReportRunner().Run(null),
				Throws.ArgumentNullException.And.Property("ParamName").EqualTo("checkmarxApiSession"));
        }

        [Test]
	    public void Run_NoScans()
        {
            Mock<ICheckmarxApiSession> checkmarxApiSessionMock;
            NotFalsePositiveResultsReportRunner notFalsePositiveResultsReportRunner;

            checkmarxApiSessionMock = new Mock<ICheckmarxApiSession>(MockBehavior.Strict);
            checkmarxApiSessionMock
                .Setup(checkmarxApiSession => checkmarxApiSession.GetProjectScans())
                .Returns(() => new ProjectScannedDisplayData[0]);

            notFalsePositiveResultsReportRunner = new NotFalsePositiveResultsReportRunner();
            Assert.That(
                notFalsePositiveResultsReportRunner.Run(checkmarxApiSessionMock.Object),
                Is.Empty);

            checkmarxApiSessionMock.VerifyAll();
        }

        [Test]
        public void Run_OneRule_OneResult()
        {
            Mock<ICheckmarxApiSession> checkmarxApiSessionMock;
            NotFalsePositiveResultsReportRunner notFalsePositiveResultsReportRunner;
            const string projectName = "Project Name";
            const string ruleName = "Rule Name";
            const Severity severity = Severity.High;
            const string fileName = "File Name";
            const uint line = 42;
            const string deepLink = "http://deepLink";
            const Status status = Status.Recurrent;
            const bool falsePositive = false;
            const long scanId = 1;
            const long reportId = 1;
            string reportXml = $@"
<Project>
    <Rule name='{ruleName}' Severity='{severity}'>
        <Result FileName='{fileName}' Line='{line}' DeepLink='{deepLink}' Status='{status}' FalsePositive='{falsePositive}' />
    </Rule>
</Project>";
            byte[] reportBytes = Encoding.UTF8.GetBytes(reportXml);
            IList<ScanResult> scanResults;

            checkmarxApiSessionMock = new Mock<ICheckmarxApiSession>(MockBehavior.Strict);
            checkmarxApiSessionMock
                .Setup(checkmarxApiSession => checkmarxApiSession.GetProjectScans())
                .Returns(() => new ProjectScannedDisplayData[]
                {
                    new ProjectScannedDisplayData
                    {
                        ProjectName = projectName,
                        LastScanID = scanId
                    }
                });
            checkmarxApiSessionMock
                .Setup(checkmarxApiSession => checkmarxApiSession.CreateScanReport(scanId, CxWSReportType.XML))
                .Returns(() => reportId);
            checkmarxApiSessionMock
                .Setup(checkmarxApiSession => checkmarxApiSession.GetScanReportStatus(reportId))
                .Returns(() => new CxWSReportStatusResponse
                {
                    IsFailed = false,
                    IsReady = true,
                });
            checkmarxApiSessionMock
                .Setup(checkmarxApiSession => checkmarxApiSession.GetScanReport(reportId))
                .Returns(() => reportBytes);

            notFalsePositiveResultsReportRunner = new NotFalsePositiveResultsReportRunner();
            scanResults = notFalsePositiveResultsReportRunner.Run(checkmarxApiSessionMock.Object);
            Assert.That(scanResults,
                Is.EquivalentTo(new []
                {
                    new ScanResult(
                        projectName,
                        ruleName,
                        severity,
                        fileName,
                        line,
                        new Uri(deepLink),
                        status,
                        falsePositive
                    )
                }));

            checkmarxApiSessionMock.VerifyAll();
        }

        [Test]
        public void Run_OneRule_OneFP_OneTP()
        {
            Mock<ICheckmarxApiSession> checkmarxApiSessionMock;
            NotFalsePositiveResultsReportRunner notFalsePositiveResultsReportRunner;
            const string projectName = "Project Name";
            const string ruleName = "Rule Name";
            const Severity severity = Severity.High;
            const string fileName1 = "File Name";
            const uint line1 = 42;
            const string deepLink1 = "http://deepLink";
            const Status status1 = Status.Recurrent;
            const bool falsePositive1 = false;
            const string fileName2 = "File Name 2";
            const uint line2 = 54;
            const string deepLink2 = "http://deepLink2";
            const Status status2 = Status.Recurrent;
            const bool falsePositive2 = true;
            const long scanId = 1;
            const long reportId = 1;
            string reportXml = 
$@"<Project>
    <Rule name='{ruleName}' Severity='{severity}'>
        <Result FileName='{fileName1}' Line='{line1}' DeepLink='{deepLink1}' Status='{status1}' FalsePositive='{falsePositive1}' />
        <Result FileName='{fileName2}' Line='{line2}' DeepLink='{deepLink2}' Status='{status2}' FalsePositive='{falsePositive2}' />
    </Rule>
</Project>";
            byte[] reportBytes = Encoding.UTF8.GetBytes(reportXml);
            IList<ScanResult> scanResults;

            checkmarxApiSessionMock = new Mock<ICheckmarxApiSession>(MockBehavior.Strict);
            checkmarxApiSessionMock
                .Setup(checkmarxApiSession => checkmarxApiSession.GetProjectScans())
                .Returns(() => new ProjectScannedDisplayData[]
                {
                    new ProjectScannedDisplayData
                    {
                        ProjectName = projectName,
                        LastScanID = scanId
                    }
                });
            checkmarxApiSessionMock
                .Setup(checkmarxApiSession => checkmarxApiSession.CreateScanReport(scanId, CxWSReportType.XML))
                .Returns(() => reportId);
            checkmarxApiSessionMock
                .Setup(checkmarxApiSession => checkmarxApiSession.GetScanReportStatus(reportId))
                .Returns(() => new CxWSReportStatusResponse
                {
                    IsFailed = false,
                    IsReady = true,
                });
            checkmarxApiSessionMock
                .Setup(checkmarxApiSession => checkmarxApiSession.GetScanReport(reportId))
                .Returns(() => reportBytes);

            notFalsePositiveResultsReportRunner = new NotFalsePositiveResultsReportRunner();
            scanResults = notFalsePositiveResultsReportRunner.Run(checkmarxApiSessionMock.Object);
            Assert.That(scanResults,
                Is.EquivalentTo(new[]
                {
                    new ScanResult(
                        projectName,
                        ruleName,
                        severity,
                        fileName1,
                        line1,
                        new Uri(deepLink1),
                        status1,
                        falsePositive1
                    )
                }));

            checkmarxApiSessionMock.VerifyAll();
        }

        [Test]
        public void Run_OneRule_TwoTP()
        {
            Mock<ICheckmarxApiSession> checkmarxApiSessionMock;
            NotFalsePositiveResultsReportRunner notFalsePositiveResultsReportRunner;
            const string projectName = "Project Name";
            const string ruleName = "Rule Name";
            const Severity severity = Severity.High;
            const string fileName1 = "File Name";
            const uint line1 = 42;
            const string deepLink1 = "http://deepLink";
            const Status status1 = Status.Recurrent;
            const bool falsePositive1 = false;
            const string fileName2 = "File Name 2";
            const uint line2 = 54;
            const string deepLink2 = "http://deepLink2";
            const Status status2 = Status.Recurrent;
            const bool falsePositive2 = false;
            const long scanId = 1;
            const long reportId = 1;
            string reportXml =
$@"<Project>
    <Rule name='{ruleName}' Severity='{severity}'>
        <Result FileName='{fileName1}' Line='{line1}' DeepLink='{deepLink1}' Status='{status1}' FalsePositive='{falsePositive1}' />
        <Result FileName='{fileName2}' Line='{line2}' DeepLink='{deepLink2}' Status='{status2}' FalsePositive='{falsePositive2}' />
    </Rule>
</Project>";
            byte[] reportBytes = Encoding.UTF8.GetBytes(reportXml);
            IList<ScanResult> scanResults;

            checkmarxApiSessionMock = new Mock<ICheckmarxApiSession>(MockBehavior.Strict);
            checkmarxApiSessionMock
                .Setup(checkmarxApiSession => checkmarxApiSession.GetProjectScans())
                .Returns(() => new ProjectScannedDisplayData[]
                {
                    new ProjectScannedDisplayData
                    {
                        ProjectName = projectName,
                        LastScanID = scanId
                    }
                });
            checkmarxApiSessionMock
                .Setup(checkmarxApiSession => checkmarxApiSession.CreateScanReport(scanId, CxWSReportType.XML))
                .Returns(() => reportId);
            checkmarxApiSessionMock
                .Setup(checkmarxApiSession => checkmarxApiSession.GetScanReportStatus(reportId))
                .Returns(() => new CxWSReportStatusResponse
                {
                    IsFailed = false,
                    IsReady = true,
                });
            checkmarxApiSessionMock
                .Setup(checkmarxApiSession => checkmarxApiSession.GetScanReport(reportId))
                .Returns(() => reportBytes);

            notFalsePositiveResultsReportRunner = new NotFalsePositiveResultsReportRunner();
            scanResults = notFalsePositiveResultsReportRunner.Run(checkmarxApiSessionMock.Object);
            Assert.That(scanResults,
                Is.EquivalentTo(new[]
                {
                    new ScanResult(
                        projectName,
                        ruleName,
                        severity,
                        fileName1,
                        line1,
                        new Uri(deepLink1),
                        status1,
                        falsePositive1
                    ),
                    new ScanResult(
                        projectName,
                        ruleName,
                        severity,
                        fileName2,
                        line2,
                        new Uri(deepLink2),
                        status2,
                        falsePositive2
                    )
                }));

            checkmarxApiSessionMock.VerifyAll();
        }

    }
}
