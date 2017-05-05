using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckmarxReports.Checkmarx;
using CheckmarxReports.CommandLineOptions;
using CheckmarxReports.CxSDKWebService;

namespace CheckmarxReports.Reports
{
    /// <summary>
    /// Base class for report runners.
    /// </summary>
    public static class ReportRunnerHelper
    {
        /// <summary>
        /// Max simultaneous report runs.
        /// </summary>
        /// <remarks>
        /// May move this into command line arguments in the future.
        /// </remarks>
        public const int MaxParallelization = 3;

        /// <summary>
        /// Determine the project predicate to exclude projects.
        /// </summary>
        /// <param name="options">
        /// The command line options. This cannot be null.
        /// </param>
        /// <returns>
        /// A predicate determining which projects to include in the report.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="options"/> cannot be null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Cannot have both include and exclude projects in <paramref name="options"/>.
        /// </exception>
        public static Func<ProjectScannedDisplayData, bool> GetProjectPredicate(CheckmarxReportOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (options.ExcludeProjects != null && options.ExcludeProjects.Any()
                && options.Projects != null && options.Projects.Any())
            {
                throw new ArgumentException("Cannot have both exclude and include projects");
            }

            Func<ProjectScannedDisplayData, bool> result;

            if (options.ExcludeProjects != null
                && options.ExcludeProjects.Any())
            {
                result = project => !options.ExcludeProjects.Contains(project.ProjectName);
            }
            else if (options.Projects != null
                && options.Projects.Any())
            {
                result = project => options.Projects.Contains(project.ProjectName);
            }
            else
            {
                result = project => true;
            }

            return result;
        }
    }
}
