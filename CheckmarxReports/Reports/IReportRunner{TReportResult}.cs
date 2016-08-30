using System;
using System.Collections.Generic;
using CheckmarxReports.Checkmarx;
using CheckmarxReports.CommandLineOptions;

namespace CheckmarxReports.Reports
{
    /// <summary>
    /// A Checkmarx report runner.
    /// </summary>
    /// <typeparam name="TReportResult">
    /// The type of entries returned from the report.
    /// </typeparam>
    public interface IReportRunner<TReportResult>
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
        /// <paramref name="checkmarxApiSession"/> and <paramref name="options"/> cannot be null.
        /// </exception>
        /// <exception cref="CheckmarxErrorException">
        /// Checkmarx returned an unexpected result or error.
        /// </exception>
        /// <exception cref="CheckmarxCommunicationException">
        /// Communication with the Checkmarx server failed.
        /// </exception>
        IList<TReportResult> Run(ICheckmarxApiSession checkmarxApiSession, CheckmarxReportOptions options);
    }
}
