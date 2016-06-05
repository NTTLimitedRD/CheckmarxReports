using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using CheckmarxReports.CxSDKWebService;

namespace CheckmarxReports.Checkmarx
{
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
        /// An <see cref="XDocument"/> containing the loaded scan report.
        /// </returns>
        /// <exception cref="CheckmarxErrorException">
        /// Either the report generation failed or Checkmarx returned invalid XML for the scan report.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// no argument can be null.
        /// </exception>
        public static XDocument GenerateLastScanReport(ICheckmarxApiSession checkmarxApiSession, ProjectScannedDisplayData project)
        {
            if (checkmarxApiSession == null)
            {
                throw new ArgumentNullException(nameof(checkmarxApiSession));
            }
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            long reportId;
            XDocument xDocument;
            byte[] report;

            reportId = checkmarxApiSession.CreateScanReport(project.LastScanID, CxWSReportType.XML);
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

            report = checkmarxApiSession.GetScanReport(reportId);
            using (MemoryStream memoryStream = new MemoryStream(report))
            {
                try
                {
                    xDocument = XDocument.Load(memoryStream, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
                }
                catch (XmlException ex)
                {
                    throw new CheckmarxErrorException(
                        $"Checkmarx returned invalid XML for report ID {reportId} on scan {project.LastScanID} on project {project.ProjectName}", ex);
                }
            }

            return xDocument;
        }
    }
}
