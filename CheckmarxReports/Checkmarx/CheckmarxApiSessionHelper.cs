using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using CheckmarxReports.CxSDKWebService;

namespace CheckmarxReports.Checkmarx
{
    /// <summary>
    /// Helper methods that wrap multiple <see cref="ICheckmarxApiSession"/> calls.
    /// </summary>
    public static class CheckmarxApiSessionHelper
    {
        /// <summary>
        /// Generate a Checkmarx scan report for the most recent scan for the given project.
        /// </summary>
        /// <param name="checkmarxApiSession">
        /// The <see cref="ICheckmarxApiSession"/> to generate the report with. This cannot be null.
        /// </param>
        /// <param name="project">
        /// The project to get the last scan for. This cannot be null.
        /// </param>
        /// <returns>
        /// An string containing a CSV version of the report.
        /// </returns>
        /// <exception cref="CheckmarxErrorException">
        /// Either the report generation failed or Checkmarx returned invalid XML for the scan report.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// no argument can be null.
        /// </exception>
        public static string GenerateLastScanCsvReport(ICheckmarxApiSession checkmarxApiSession, ProjectScannedDisplayData project)
        {
            return Encoding.UTF8.GetString(GenerateLastScanReport(checkmarxApiSession, project, CxWSReportType.CSV));
        }

        /// <summary>
        /// Generate a Checkmarx scan report for the most recent scan for the given project.
        /// </summary>
        /// <param name="checkmarxApiSession">
        /// The <see cref="ICheckmarxApiSession"/> to generate the report with. This cannot be null.
        /// </param>
        /// <param name="project">
        /// The project to get the last scan for. This cannot be null.
        /// </param>
        /// <returns>
        /// An <see cref="XDocument"/> containing the loaded scan report.
        /// </returns>
        /// <exception cref="CheckmarxErrorException">
        /// Either the report generation failed or Checkmarx returned invalid XML for the scan report.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// no argument can be null.
        /// </exception>
        public static XDocument GenerateLastScanXmlReport(ICheckmarxApiSession checkmarxApiSession, ProjectScannedDisplayData project)
        {
            XDocument xDocument;

            byte[] report = GenerateLastScanReport(checkmarxApiSession, project, CxWSReportType.XML);
            using (MemoryStream memoryStream = new MemoryStream(report))
            {
                try
                {
                    xDocument = XDocument.Load(memoryStream, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
                }
                catch (XmlException ex)
                {
                    throw new CheckmarxErrorException(
                        $"Checkmarx returned invalid XML for report on scan {project.LastScanID} on project {project.ProjectName}", ex);
                }
            }

            return xDocument;
        }

        /// <summary>
        /// Generate a Checkmarx scan report for the most recent scan for the given project.
        /// </summary>
        /// <param name="checkmarxApiSession">
        /// The <see cref="ICheckmarxApiSession"/> to generate the report with. This cannot be null.
        /// </param>
        /// <param name="project">
        /// The project to get the last scan for. This cannot be null.
        /// </param>
        /// <param name="reportType">
        /// The result format for the report.
        /// </param>
        /// <returns>
        /// An <see cref="XDocument"/> containing the loaded scan report.
        /// </returns>
        /// <exception cref="CheckmarxErrorException">
        /// Either the report generation failed or Checkmarx returned invalid XML for the scan report.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// no argument can be null.
        /// </exception>
        private static byte[] GenerateLastScanReport( ICheckmarxApiSession checkmarxApiSession,
            ProjectScannedDisplayData project, CxWSReportType reportType)
        {
            if (checkmarxApiSession == null)
            {
                throw new ArgumentNullException(nameof(checkmarxApiSession));
            }
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (!Enum.IsDefined(typeof (CxWSReportType), reportType))
            {
                throw new ArgumentOutOfRangeException(nameof(reportType));
            }

            long reportId;

            reportId = checkmarxApiSession.CreateScanReport(project.LastScanID, reportType);
            for (;;)
            {
                CxWSReportStatusResponse reportStatusResponse = checkmarxApiSession.GetScanReportStatus(reportId);
                if (reportStatusResponse.IsFailed)
                {
                    throw new CheckmarxErrorException(
                        $"Generating report ID {reportId} on scan {project.LastScanID} on project {project.ProjectName} failed");
                }
                else if (reportStatusResponse.IsReady)
                {
                    break;
                }

                // TODO: Consider a better mechanism
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));

                // TODO: Consider a timeout
            }

            return checkmarxApiSession.GetScanReport(reportId);
        }
    }
}
