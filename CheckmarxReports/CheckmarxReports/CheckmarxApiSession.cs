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
    public class CheckmarxApiSession: IDisposable
    {
        private const int CheckmarxApiVersion = 1;
        private const int CheckmarxLocaleId = 1033; // US English
        private bool _disposed = false;

        /// <summary>
        /// Create a new <see cref="CheckmarxApiSession"/>.
        /// </summary>
        /// <param name="server">
        /// The name of the Checkmarx server. This cannot be null, empty or whitespace.
        /// </param>
        /// <param name="username">
        /// The user name to login with. This cannot be null, empty or whitespace.
        /// </param>
        /// <param name="password">
        /// The password to login with. This cannot be null, empty or whitespace.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// No argument can be null, empty or whitespace.
        /// </exception>
        /// <exception cref="CommunicationException">
        /// An error occurred communicating with the Checkmarx server.
        /// </exception>
        /// <exception cref="CheckmarxErrorException">
        /// The Checkmarx API returned an unexpected error.
        /// </exception>
        public CheckmarxApiSession(string server, string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(server))
            {
                throw new ArgumentNullException(nameof(server));
            }
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            Uri sdkUrl;

            sdkUrl = GetSdkUrl(server);
            SoapClient = GetSoapClient(sdkUrl);
            SessionId = Login(userName, password);
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~CheckmarxApiSession()
        {
            Disposing(false);
        }

        /// <summary>
        /// IDisposable implementation.
        /// </summary>
        public void Dispose()
        {
            Disposing(true);    
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposing pattern.
        /// </summary>
        /// <param name="disposing">
        /// True if called from <see cref="Dispose"/>, false if called from the finalizer.
        /// </param>
        protected virtual void Disposing(bool disposing)
        {
            if (!_disposed)
            {
                Logout();
                SessionId = null;
                SoapClient.Close();
                _disposed = true;
            }
        }

        /// <summary>
        /// The <see cref="CxSDKWebServiceSoapClient"/> used to call Checkmarx APIs.
        /// </summary>
        private CxSDKWebServiceSoapClient SoapClient { get; }

        /// <summary>
        /// The ID of the current session returned from <see cref="Login"/>.
        /// </summary>
        private string SessionId { get; set; }

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

        public long CreateScanReport(long scanId, CxWSReportType reportType)
        {
            CxWSReportRequest reportRequest;

            reportRequest = new CxWSReportRequest()
            {
                ScanID = scanId,
                Type = reportType
            };

            CxWSCreateReportResponse response = SoapClient.CreateScanReport(SessionId, reportRequest);
            if (!response.IsSuccesfull)
            {
                throw new CheckmarxErrorException(response.ErrorMessage);
            }

            return response.ID;
        }

        /// <summary>
        /// Get the projects.
        /// </summary>
        /// <returns>
        /// The <see cref="ProjectDisplayData"/> containing project information.
        /// </returns>
        /// <exception cref="CheckmarxErrorException">
        /// The Checkmarx API returned an unexpected error.
        /// </exception>

        public ProjectDisplayData[] GetProjects()
        {
            CxWSResponseProjectsDisplayData response = SoapClient.GetProjectsDisplayData(SessionId);
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
        /// <exception cref="CheckmarxErrorException">
        /// The Checkmarx API returned an unexpected error.
        /// </exception>

        public ProjectScannedDisplayData[] GetProjectScans()
        {
            CxWSResponseProjectScannedDisplayData response = SoapClient.GetProjectScannedDisplayData(SessionId);
            if (!response.IsSuccesfull)
            {
                throw new CheckmarxErrorException(response.ErrorMessage);
            }

            return response.ProjectScannedList;
        }

        /// <summary>
        /// Get the report contents.
        /// </summary>
        /// <param name="reportId">
        /// The report ID returned from <see cref="CreateScanReport"/>.
        /// </param>
        /// <exception cref="CheckmarxErrorException">
        /// The Checkmarx API returned an unexpected error.
        /// </exception>

        public byte[] GetScanReport(long reportId)
        {
            CxWSResponseScanResults response = SoapClient.GetScanReport(SessionId, reportId);
            if (!response.IsSuccesfull)
            {
                throw new CheckmarxErrorException(response.ErrorMessage);
            }

            return response.ScanResults;
        }

        /// <summary>
        /// See if a report has been generated yet or has failed.
        /// </summary>
        /// <param name="reportId">
        /// The report ID returned from <see cref="CreateScanReport"/>.
        /// </param>
        /// <exception cref="CheckmarxErrorException">
        /// The Checkmarx API returned an unexpected error.
        /// </exception>
        public CxWSReportStatusResponse GetScanReportStatus(long reportId)
        {
            CxWSReportStatusResponse response = SoapClient.GetScanReportStatus(SessionId, reportId);
            if (!response.IsSuccesfull)
            {
                throw new CheckmarxErrorException(response.ErrorMessage);
            }

            return response;
        }

        /// <summary>
        /// Get the Checkmarx SDK URL.
        /// </summary>
        /// <param name="server">
        /// The name of the Checkmarx server. This cannot be null, empty or whitespace.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="server"/> cannot be null, empty or whitespace.
        /// </exception>
        /// <exception cref="CommunicationException">
        /// An error occurred communicating with the Checkmarx server.
        /// </exception>
        /// <exception cref="CheckmarxErrorException">
        /// The Checkmarx API returned an unexpected error.
        /// </exception>
        private Uri GetSdkUrl(string server)
        {
            if (string.IsNullOrWhiteSpace(server))
            {
                throw new ArgumentNullException(nameof(server));
            }

            BasicHttpBinding binding;
            EndpointAddress endpointAddress;

            binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            endpointAddress = new EndpointAddress(string.Format($"https://{Uri.EscapeUriString(server)}/Cxwebinterface/CxWsResolver.asmx"));

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
        private CxSDKWebServiceSoapClient GetSoapClient(Uri sdkUrl)
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
        private string Login(string username, string password)
        {
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

            CxWSResponseLoginData loginData = SoapClient.Login(credentials, CheckmarxLocaleId);
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
        /// <exception cref="CheckmarxErrorException">
        /// The Checkmarx API returned an unexpected error.
        /// </exception>
        private void Logout()
        {
            CxSDKWebService.CxWSBasicRepsonse basicRepsonse = SoapClient.Logout(SessionId);
            if (!basicRepsonse.IsSuccesfull)
            {
                throw new CheckmarxErrorException(basicRepsonse.ErrorMessage);
            }
        }
    }
}
