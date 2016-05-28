using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CheckmarxReports.Test
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

        // TODO: Mock out ICheckmarxApiSession and test a report run
    }
}
