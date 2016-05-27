using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using CheckmarxReports.CxSDKWebService;
using CheckmarxReports.CxWsResolver;
using CxClientType = CheckmarxReports.CxWsResolver.CxClientType;

namespace CheckmarxReports
{
    /// <summary>
    /// Run the report from the Checkmarx server.
    /// </summary>
    public sealed class ReportFactory
    {
        /// <summary>
        /// Construct a <see cref="ReportFactory"/>.
        /// </summary>
        /// <param name="hostName">
        /// The host name for the Checkmarx server. This cannot be null, empty or whitespace.
        /// </param>
        /// <param name="userName">
        /// The user name to log into Checkmarx with. This cannot be null, empty or whitespace.
        /// </param>
        /// <param name="password">
        /// The password to log into Checkmarx with. This cannot be null, empty or whitespace.
        /// </param>
        public ReportFactory(string hostName, string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(hostName))
            {
                throw new ArgumentNullException(nameof(hostName));
            }
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            HostName = hostName;
            UserName = userName;
            Password = password;
        }

        /// <summary>
        /// The name of the Checkmarx server.
        /// </summary>
        public string HostName
        {
            get; set;
        }

        /// <summary>
        /// The user name to login with.
        /// </summary>
        public string UserName
        {
            get; set;
        }

        /// <summary>
        /// The password to login with.
        /// </summary>
        public string Password
        {
            get; set;
        }

        public void Run()
        {
            Uri sdkUrl;
            string sessionId = null;
            ProjectScannedDisplayData[] projects;

            try
            {
                sdkUrl = CheckmarxApi.GetSdkUrl(HostName);
                using (CxSDKWebServiceSoapClient client = CheckmarxApi.GetSoapClient(sdkUrl))
                {
                    try
                    {
                        sessionId = CheckmarxApi.Login(client, UserName, Password);
                        projects = CheckmarxApi.GetProjectScans(client, sessionId);
                        ProjectScannedDisplayData project = projects.First();
                        // foreach (ProjectScannedDisplayData project in projects)
                        {
                            // TODO: Include in output
                            // project.ProjectName;

                            // Generate an XML scan report
                            long reportId = CheckmarxApi.CreateScanReport(client, sessionId, project.LastScanID, CxWSReportType.XML);

                            // Extract whether there are any issues that are "New" or "Unconfirmed"
                            for (;;)
                            {
                                CxWSReportStatusResponse reportStatusResponse = CheckmarxApi.GetScanReportStatus(
                                    client, sessionId, reportId);
                                if (reportStatusResponse.IsFailed)
                                {
                                    throw new CheckmarxErrorException("Generating report ID {reportID} on scan {project.LastScanId} on project {project.ProjectName} failed");
                                }
                                else if (reportStatusResponse.IsReady)
                                {
                                    break;
                                }
                            }

                            byte[] report = CheckmarxApi.GetScanReport(client, sessionId, reportId);

                        }
                    }
                    finally
                    {
                        if (sessionId != null)
                        {
                            CheckmarxApi.Logout(client, sessionId);
                        }
                    }
                }
            }
            catch (CommunicationException ex)
            {
                throw new CheckmarxCommunicationException(ex.Message, ex);
            }
        }
    }
}
