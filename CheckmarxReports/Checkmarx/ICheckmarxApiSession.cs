using System;
using CheckmarxReports.CxSDKWebService;

namespace CheckmarxReports.Checkmarx
{
    /// <summary>
    /// A Checkmarx API session.
    /// </summary>
    public interface ICheckmarxApiSession: IDisposable
    {
        /// <summary>
        /// Start creating a scan report.
        /// </summary>
        /// <param name="scanId">
        /// The scan ID to generate a report for.
        /// </param>
        /// <param name="reportType">
        /// The report type.
        /// </param>
        /// <returns>
        /// The report ID.
        /// </returns>
        /// <exception cref="CheckmarxErrorException">
        /// The Checkmarx API returned an unexpected error.
        /// </exception>
        /// <exception cref="CheckmarxCommunicationException">
        /// An error occurred communicating with the Checkmarx server.
        /// </exception>
        long CreateScanReport(long scanId, CxWSReportType reportType);

        /// <summary>
        /// Get the projects.
        /// </summary>
        /// <returns>
        /// The <see cref="ProjectDisplayData"/> containing project information.
        /// </returns>
        /// <exception cref="CheckmarxErrorException">
        /// The Checkmarx API returned an unexpected error.
        /// </exception>
        /// <exception cref="CheckmarxCommunicationException">
        /// An error occurred communicating with the Checkmarx server.
        /// </exception>
        ProjectDisplayData[] GetProjects();

        /// <summary>
        /// Get the scans.
        /// </summary>
        /// <exception cref="CheckmarxErrorException">
        /// The Checkmarx API returned an unexpected error.
        /// </exception>
        /// <exception cref="CheckmarxCommunicationException">
        /// An error occurred communicating with the Checkmarx server.
        /// </exception>
        ProjectScannedDisplayData[] GetProjectScans();

        /// <summary>
        /// Get the report contents.
        /// </summary>
        /// <param name="reportId">
        /// The report ID returned from <see cref="CheckmarxApiSession.CreateScanReport"/>.
        /// </param>
        /// <exception cref="CheckmarxErrorException">
        /// The Checkmarx API returned an unexpected error.
        /// </exception>
        /// <exception cref="CheckmarxCommunicationException">
        /// An error occurred communicating with the Checkmarx server.
        /// </exception>
        byte[] GetScanReport(long reportId);

        /// <summary>
        /// See if a report has been generated yet or has failed.
        /// </summary>
        /// <param name="reportId">
        /// The report ID returned from <see cref="CheckmarxApiSession.CreateScanReport"/>.
        /// </param>
        /// <exception cref="CheckmarxErrorException">
        /// The Checkmarx API returned an unexpected error.
        /// </exception>
        /// <exception cref="CheckmarxCommunicationException">
        /// An error occurred communicating with the Checkmarx server.
        /// </exception>
        CxWSReportStatusResponse GetScanReportStatus(long reportId);
    }
}