using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckmarxReports.CommandLineOptions;
using CheckmarxReports.CxSDKWebService;
using CheckmarxReports.Reports;
using NUnit.Framework;

namespace CheckmarxReports.Test.Reports
{
    [TestFixture]
    public class ReportRunnerHelperTests
    {
        [Test]
        public void Test_GetProjectPredicate_Null()
        {
            Assert.That(
                () => ReportRunnerHelper.GetProjectPredicate(null),
                Throws.ArgumentNullException.And.Property("ParamName").EqualTo("options"));
        }

        [TestCaseSource(nameof(Test_GetProjectPredicate_Source))]
        public void Test_GetProjectPredicate_ExcludeProjects(string excludeProjectNames)
        {
            CheckmarxReportOptions options;
            Func<ProjectScannedDisplayData, bool> projectPredicate;

            options = new NotFalsePositiveReportOptions();
            if (excludeProjectNames != null)
            {
                options.ExcludeProjects = excludeProjectNames.Split(new[] {", "}, StringSplitOptions.RemoveEmptyEntries);
            }

            projectPredicate = ReportRunnerHelper.GetProjectPredicate(options);
            foreach (string testProjectName in new[] {"a", "b", "c"})
            {
                Assert.That(
                    projectPredicate(new ProjectScannedDisplayData { ProjectName = testProjectName }),
                    Is.EqualTo(options.ExcludeProjects == null || !options.ExcludeProjects.Contains(testProjectName)),
                    $"Project '{testProjectName}' incorrect");
            }
        }

        [TestCaseSource(nameof(Test_GetProjectPredicate_Source))]
        public void Test_GetProjectPredicate_Projects(string projectNames)
        {
            CheckmarxReportOptions options;
            Func<ProjectScannedDisplayData, bool> projectPredicate;

            options = new NotFalsePositiveReportOptions();
            if (projectNames != null)
            {
                options.Projects = projectNames.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            }

            projectPredicate = ReportRunnerHelper.GetProjectPredicate(options);
            foreach (string testProjectName in new[] { "a", "b", "c" })
            {
                Assert.That(
                    projectPredicate(new ProjectScannedDisplayData { ProjectName = testProjectName }),
                    Is.EqualTo(options.Projects == null || options.Projects.Contains(testProjectName)),
                    $"Project '{testProjectName}' incorrect");
            }
        }

        [Test]
        public void Test_GetProjectPredicate_ProjectsAndExcludeProjects()
        {
            CheckmarxReportOptions options;

            options = new NotFalsePositiveReportOptions();
            options.ExcludeProjects = new [] {"a"};
            options.Projects = new [] { "a" };

            Assert.That(() => ReportRunnerHelper.GetProjectPredicate(options),
                Throws.ArgumentException);
        }

        private static IEnumerable<TestCaseData> Test_GetProjectPredicate_Source()
        {
            yield return new TestCaseData(null);
            yield return new TestCaseData("a");
            yield return new TestCaseData("A");
            yield return new TestCaseData("a, b");
            yield return new TestCaseData("a, c");
            yield return new TestCaseData("c, a");
            yield return new TestCaseData("a, b, c");
            yield return new TestCaseData("a, b, c, d");
        }
    }
}
