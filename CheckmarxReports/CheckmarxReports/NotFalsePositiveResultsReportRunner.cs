using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using CheckmarxReports.CxSDKWebService;

namespace CheckmarxReports
{
    /// <summary>
    /// Get a list of non false positives from the server.
    /// </summary>
    public class NotFalsePositiveResultsReportRunner: IReportRunner<ScanResult>
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
        public IList<ScanResult> Run(ICheckmarxApiSession checkmarxApiSession)
        {
            if (checkmarxApiSession == null)
            {
                throw new ArgumentNullException(nameof(checkmarxApiSession));
            }

            return checkmarxApiSession.GetProjectScans()
                    .AsParallel()
                    .WithDegreeOfParallelism(3)
                    .SelectMany(
                        project =>
                            GenerateLastScanReport(checkmarxApiSession, project)
                                .XPathSelectElements("//Result[@FalsePositive=\"False\"]")
                                .Select(xmlNode => XmlNodeToScanResult(xmlNode, project.ProjectName)))
                    .ToList();
        }

        /// <summary>
        /// Generate a Checkmarx scan report for the most recent scan for the given project.
        /// </summary>
        /// <param name="checkmarxApiSession">
        /// The <see cref="ICheckmarxApiSession"/> to generate the report with.
        /// </param>
        /// <param name="project">
        /// The project to get the last scan for
        /// </param>
        /// <returns>
        /// An <see cref="XDocument"/> containing the loaded scan report.
        /// </returns>
        /// <exception cref="CheckmarxErrorException">
        /// Either the report generation failed or Checkmarx returned invalid XML for the scan report.
        /// </exception>
        private XDocument GenerateLastScanReport(ICheckmarxApiSession checkmarxApiSession, ProjectScannedDisplayData project)
        {
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

        /// <summary>
        /// Convert the Result <see cref="XmlNode"/> to a <see cref="ScanResult"/>.
        /// </summary>
        /// <param name="xmlNode">
        /// The <see cref="XElement"/> associated with a result node.
        /// </param>
        /// <param name="projectName">
        /// The project name.
        /// </param>
        /// <returns>
        /// A populated <see cref="ScanResult"/>.
        /// </returns>
        /// <exception cref="CheckmarxErrorException">
        /// The XML returned by Checkmarx contains errors.
        /// </exception>
        private ScanResult XmlNodeToScanResult(XElement xmlNode, string projectName)
        {
            string ruleName;
            string severity;
            string fileName;
            uint line;
            Uri deepLink;
            string status;
            bool falsePositive;

            ruleName = xmlNode?.Parent?.Attribute(XName.Get("name"))?.Value ?? "(none)";
            severity = xmlNode?.Parent?.Attribute(XName.Get("Severity"))?.Value ?? "(none)";
            fileName = xmlNode?.Attribute(XName.Get("FileName"))?.Value ?? "(none)";
            if (!uint.TryParse(xmlNode?.Attribute(XName.Get("Line"))?.Value ?? "0", out line))
            {
                throw new CheckmarxErrorException($"Line XML attribute for result in project {projectName} omitted or is not an integer");
            }
            if (!Uri.TryCreate(xmlNode?.Attribute(XName.Get("DeepLink"))?.Value ?? "", UriKind.Absolute, out deepLink))
            {
                throw new CheckmarxErrorException($"DeepLink XML attribute for result in project {projectName} omitted or is not a valid URL");
            }
            status = xmlNode?.Attribute(XName.Get("Status"))?.Value ?? "(none)";
            if (!bool.TryParse(xmlNode?.Attribute(XName.Get("FalsePositive"))?.Value ?? "", out falsePositive))
            {
                throw new CheckmarxErrorException($"FalsePositive XML attribute for result in project {projectName} omitted or is not a valid boolean");
            }

            try
            {
                return new ScanResult(projectName, ruleName, severity, fileName, line, deepLink, status, falsePositive);
            }
            catch (ArgumentException ex)
            {
                throw new CheckmarxErrorException($"XML for result in project {projectName} is invalid", ex);
            }
        }
    }
}
