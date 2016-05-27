using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using CheckmarxReports.CxSDKWebService;
using CheckmarxReports.CxWsResolver;
using CxClientType = CheckmarxReports.CxSDKWebService.CxClientType;

namespace CheckmarxReports
{
    /// <summary>
    /// Wrappers for the individual Checkmarx web methods.
    /// </summary>
    public static class CheckmarxApi
    {
        private const int CheckmarxApiVersion = 1;
        private const int CheckmarxLocaleId = 1033; // US English

        /// <summary>
        /// Start creating a scan report.
        /// </summary>
        /// <param name="client">
        /// The <see cref="CxSDKWebServiceSoapClient"/> to use. This cannot be null.
        /// </param>
        /// <param name="sessionId">
        /// The session ID returned from <see cref="Login"/>. This cannot be null, empty or whitespace.
        /// </param>
        /// <param name="scanId">
        /// The scan ID to generate a report for.
        /// </param>
        /// <param name="reportType">
        /// The report type.
        /// </param>
        /// <returns>
        /// The report ID.
        /// </returns>
        public static long CreateScanReport(CxSDKWebServiceSoapClient client, string sessionId, long scanId, CxWSReportType reportType)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new ArgumentNullException(nameof(sessionId));
            }

            CxWSReportRequest reportRequest;

            reportRequest = new CxWSReportRequest()
            {
                ScanID = scanId,
                Type = reportType
            };

            CxWSCreateReportResponse response = client.CreateScanReport(sessionId, reportRequest);
            if (!response.IsSuccesfull)
            {
                throw new CheckmarxErrorException(response.ErrorMessage);
            }

            return response.ID;
        }

        /// <summary>
        /// Get the projects.
        /// </summary>
        /// <param name="client">
        /// The <see cref="CxSDKWebServiceSoapClient"/> to use. This cannot be null.
        /// </param>
        /// <param name="sessionId">
        /// The session ID returned from <see cref="Login"/>. This cannot be null, empty or whitespace.
        /// </param>
        public static ProjectDisplayData[] GetProjects(CxSDKWebServiceSoapClient client, string sessionId)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new ArgumentNullException(nameof(sessionId));
            }

            CxWSResponseProjectsDisplayData response = client.GetProjectsDisplayData(sessionId);
            if (!response.IsSuccesfull)
            {
                throw new CheckmarxErrorException(response.ErrorMessage);
            }

            return response.projectList;
        }

        /// <summary>
        /// Get the scans.
        /// </summary>
        /// <param name="client">
        /// The <see cref="CxSDKWebServiceSoapClient"/> to use. This cannot be null.
        /// </param>
        /// <param name="sessionId">
        /// The session ID returned from <see cref="Login"/>. This cannot be null, empty or whitespace.
        /// </param>
        public static ProjectScannedDisplayData[] GetProjectScans(CxSDKWebServiceSoapClient client, string sessionId)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new ArgumentNullException(nameof(sessionId));
            }

            CxWSResponseProjectScannedDisplayData response = client.GetProjectScannedDisplayData(sessionId);
            if (!response.IsSuccesfull)
            {
                throw new CheckmarxErrorException(response.ErrorMessage);
            }

            return response.ProjectScannedList;
        }

        /// <summary>
        /// Get the report contents.
        /// </summary>
        /// <param name="client">
        /// The <see cref="CxSDKWebServiceSoapClient"/> to use. This cannot be null.
        /// </param>
        /// <param name="sessionId">
        /// The session ID returned from <see cref="Login"/>. This cannot be null, empty or whitespace.
        /// </param>
        /// <param name="reportId">
        /// The report ID returned from <see cref="CreateScanReport"/>.
        /// </param>
        public static byte[] GetScanReport(CxSDKWebServiceSoapClient client, string sessionId, long reportId)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new ArgumentNullException(nameof(sessionId));
            }

            CxWSResponseScanResults response = client.GetScanReport(sessionId, reportId);
            if (!response.IsSuccesfull)
            {
                throw new CheckmarxErrorException(response.ErrorMessage);
            }

            return response.ScanResults;
        }

        /// <summary>
        /// See if a report has been generated yet or has failed.
        /// </summary>
        /// <param name="client">
        /// The <see cref="CxSDKWebServiceSoapClient"/> to use. This cannot be null.
        /// </param>
        /// <param name="sessionId">
        /// The session ID returned from <see cref="Login"/>. This cannot be null, empty or whitespace.
        /// </param>
        /// <param name="reportId">
        /// The report ID returned from <see cref="CreateScanReport"/>.
        /// </param>
        public static CxWSReportStatusResponse GetScanReportStatus(CxSDKWebServiceSoapClient client, string sessionId, long reportId)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new ArgumentNullException(nameof(sessionId));
            }

            CxWSReportStatusResponse response = client.GetScanReportStatus(sessionId, reportId);
            if (!response.IsSuccesfull)
            {
                throw new CheckmarxErrorException(response.ErrorMessage);
            }

            return response;
        }

        /// <summary>
        /// Get the Checkmarx SDK URL.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="hostName"/> cannot be null, empty or whitespace.
        /// </exception>
        /// <exception cref="CommunicationException">
        /// An error occurred communicating with the Checkmarx server.
        /// </exception>
        /// <exception cref="CheckmarxErrorException">
        /// The Checkmarx API returned an unexpected error.
        /// </exception>
        public static Uri GetSdkUrl(string hostName)
        {
            if (string.IsNullOrWhiteSpace(hostName))
            {
                throw new ArgumentNullException(nameof(hostName));
            }

            BasicHttpBinding binding;
            EndpointAddress endpointAddress;

            binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            endpointAddress = new EndpointAddress(string.Format($"https://{Uri.EscapeUriString(hostName)}/Cxwebinterface/CxWsResolver.asmx"));

            using (CxWSResolverSoapClient client = new CxWSResolverSoapClient(binding, endpointAddress))
            {
                CxWSResponseDiscovery response = client.GetWebServiceUrl(CxWsResolver.CxClientType.SDK, CheckmarxApiVersion);
                if (!response.IsSuccesfull)
                {
                    throw new CheckmarxErrorException(response.ErrorMessage);
                }

                return new Uri(response.ServiceURL, UriKind.Absolute);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sdkUrl"></param>
        /// <returns></returns>
        public static CxSDKWebServiceSoapClient GetSoapClient(Uri sdkUrl)
        {
            BasicHttpBinding binding;
            EndpointAddress endpointAddress;

            binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            endpointAddress = new EndpointAddress(sdkUrl);

            return new CxSDKWebServiceSoapClient(binding, endpointAddress);
        }

        /// <summary>
        /// Login the given user.
        /// </summary>
        /// <param name="client">
        /// The <see cref="CxSDKWebServiceSoapClient"/> to use. This cannot be null.
        /// </param>
        /// <param name="username">
        /// The user name to login with. This cannot be null, empty or whitespace.
        /// </param>
        /// <param name="password">
        /// The password to login with. This cannot be null, empty or whitespace.
        /// </param>
        /// <returns>
        /// The session ID.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// No argument can be null. <paramref name="username"/> and <paramref name="username"/> cannot be empty or whitespace.
        /// </exception>
        /// <exception cref="CommunicationException">
        /// An error occurred communicating with the Checkmarx server.
        /// </exception>
        /// <exception cref="CheckmarxErrorException">
        /// The Checkmarx API returned an unexpected error.
        /// </exception>
        public static string Login(CxSDKWebServiceSoapClient client, string username, string password)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username));
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            Credentials credentials;

            credentials = new Credentials
            {
                User = username,
                Pass = password
            };

            CxWSResponseLoginData loginData = client.Login(credentials, CheckmarxLocaleId);
            if (!loginData.IsSuccesfull)
            {
                throw new CheckmarxErrorException(loginData.ErrorMessage);
            }

            return loginData.SessionId;
        }

        /// <summary>
        /// Logout the given session.
        /// </summary>
        /// <param name="client">
        /// The <see cref="CxSDKWebServiceSoapClient"/> to use. This cannot be null.
        /// </param>
        /// <param name="sessionId">
        /// The session ID. This cannot be null, empty or whitespace.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// No argument can be null. <paramref name="sessionId"/> cannot be empty or whitespace.
        /// </exception>
        public static void Logout(CxSDKWebServiceSoapClient client, string sessionId)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new ArgumentNullException(nameof(sessionId));
            }

            CxSDKWebService.CxWSBasicRepsonse basicRepsonse = client.Logout(sessionId);
            if (!basicRepsonse.IsSuccesfull)
            {
                throw new CheckmarxErrorException(basicRepsonse.ErrorMessage);
            }
        }
    }
}
