using System;
using System.Collections.Generic;
using System.Linq;
using CheckmarxReports.Checkmarx;
using CheckmarxReports.CommandLineOptions;
using CheckmarxReports.CxSDKWebService;

namespace CheckmarxReports.Reports
{
    /// <summary>
    /// Return the raw CSV from scan results. This is useful for debugging.
    /// </summary>
    public class RawScanCsvReportRunner: IReportRunner<string>
    {
        /// <summary>
        /// Run the report.
        /// </summary>
        /// <param name="checkmarxApiSession">
        /// A <see cref="ICheckmarxApiSession"/> used to run the report. This cannot be null.
        /// </param>
        /// <param name="options">
        /// Command line options. This cannot be null.
        /// </param>
        /// <returns>
        /// The report results.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="checkmarxApiSession"/> cannot be null.
        /// </exception>
        /// <exception cref="CheckmarxErrorException">
        /// Checkmarx returned an unexpected result or error.
        /// </exception>
        /// <exception cref="CheckmarxCommunicationException">
        /// Communication with the Checkmarx server failed.
        /// </exception>
        public IList<string> Run(ICheckmarxApiSession checkmarxApiSession, CheckmarxReportOptions options)
        {
            if (checkmarxApiSession == null)
            {
                throw new ArgumentNullException(nameof(checkmarxApiSession));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return checkmarxApiSession.GetProjectScans()
                    .AsParallel()
                    .WithDegreeOfParallelism(ReportRunnerHelper.MaxParallelization)
                    .Where(ReportRunnerHelper.GetProjectPredicate(options))
                    .Select(
                        project =>
                            CheckmarxApiSessionHelper.GenerateLastScanCsvReport(checkmarxApiSession, project).ToString())
                    .ToList();
        }
    }
}
