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
            notFalsePositiveResultsReportRunner.Run(checkmarxApiSessionMock.Object);

            checkmarxApiSessionMock.VerifyAll();
        }

        // TODO: Mock out ICheckmarxApiSession and do more complex runs
    }
}
