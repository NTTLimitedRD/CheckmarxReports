using System;
using System.Collections.Generic;
using System.Linq;
using CheckmarxReports.Checkmarx;
using CheckmarxReports.CxSDKWebService;

namespace CheckmarxReports.Reports
{
    /// <summary>
    /// Return the raw CSV from scan results. This is useful for debugging.
    /// </summary>
    public class RawScanCsvReportRunner: IReportRunner<string>
    {
        /// <summary>
        /// Max simultaneous report runs.
        /// </summary>
        public const int MaxParallelization = 3;

        /// <summary>
        /// Run the report.
        /// </summary>
        /// <param name="checkmarxApiSession">
        /// A <see cref="ICheckmarxApiSession"/> used to run the report. This cannot be null.
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
        public IList<string> Run(ICheckmarxApiSession checkmarxApiSession)
        {
            if (checkmarxApiSession == null)
            {
                throw new ArgumentNullException(nameof(checkmarxApiSession));
            }

            return checkmarxApiSession.GetProjectScans()
                    .AsParallel()
                    .WithDegreeOfParallelism(MaxParallelization)
                    .Select(
                        project =>
                            CheckmarxApiSessionHelper.GenerateLastScanCsvReport(checkmarxApiSession, project).ToString())
                    .ToList();
        }
    }
}
