using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckmarxReports.Checkmarx;

namespace CheckmarxReports
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
        IList<TReportResult> Run(ICheckmarxApiSession checkmarxApiSession);
    }
}
