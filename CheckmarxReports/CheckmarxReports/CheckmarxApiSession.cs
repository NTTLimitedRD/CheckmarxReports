using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using CheckmarxReports.CxSDKWebService;
using CheckmarxReports.CxWsResolver;

namespace CheckmarxReports
{
    /// <summary>
    /// Wrappers for the Checkmarx SDK web API methods.
    /// </summary>
    public class CheckmarxApiSession: ICheckmarxApiSession
    {
        private const int CheckmarxApiVersion = 1;
        private const int CheckmarxLocaleId = 1033; // US English
        private bool _disposed = false;

        /// <summary>
        /// Create a new <see cref="CheckmarxApiSession"/>.
        /// </summary>
        /// <param name="server">
        /// The Checkmarx server name. This cannot be null, empty or whitespace.
        /// </param>
        /// <param name="username">
        /// The username to login with. This cannot be null, empty or whitespace.
        /// </param>
        /// <param name="password">
        /// The password to login with. This cannot be null, empty or whitespace.
        /// </param>
        /// <exception cref="ArgumentException">
        /// No argument can be null, empty or whitespace.
        /// </exception>
        /// <exception cref="CheckmarxErrorException">
        /// The Checkmarx API returned an unexpected error.
        /// </exception>
        /// <exception cref="CheckmarxCommunicationException">
        /// An error occurred communicating with the Checkmarx server.
        /// </exception>
        public CheckmarxApiSession(string server, string username, string password)
        {
            if (string.IsNullOrWhiteSpace(server))
            {
                throw new ArgumentException("Cannot be null, empty or whitespace", nameof(server));
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Cannot be null, empty or whitespace", nameof(username));
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Cannot be null, empty or whitespace", nameof(password));
            }

            Uri sdkUrl;

            sdkUrl = GetSdkUrl(server);
            SoapClient = GetSoapClient(sdkUrl);
            SessionId = Login(username, password);
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
                try
                {
                    Logout();
                }
                catch
                {
                    // Ignore any errors. We tried. :-)
                }

                SessionId = null;

                try
                {
                    SoapClient.Close();
                }
                catch
                {
                    // Ignore any errors. We tried. :-)
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// The <see cref="CxSDKWebServiceSoapClient"/> used to call Checkmarx APIs.
        /// </summary>
        internal CxSDKWebServiceSoapClient SoapClient { get; }

        /// <summary>
        /// The ID of the current session returned from <see cref="Login"/>.
        /// </summary>
        internal string SessionId { get; set; }

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
        public long CreateScanReport(long scanId, CxWSReportType reportType)
        {
            CxWSReportRequest reportRequest;

            reportRequest = new CxWSReportRequest()
            {
                ScanID = scanId,
                Type = reportType
            };

            return CallCheckmarxApi(() => SoapClient.CreateScanReport(SessionId, reportRequest)).ID;
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
        /// <exception cref="CheckmarxCommunicationException">
        /// An error occurred communicating with the Checkmarx server.
        /// </exception>
        public ProjectDisplayData[] GetProjects()
        {
            return CallCheckmarxApi(() => SoapClient.GetProjectsDisplayData(SessionId)).projectList;
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
        /// <exception cref="CheckmarxCommunicationException">
        /// An error occurred communicating with the Checkmarx server.
        /// </exception>

        public ProjectScannedDisplayData[] GetProjectScans()
        {
            return CallCheckmarxApi(() => SoapClient.GetProjectScannedDisplayData(SessionId)).ProjectScannedList;
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
        /// <exception cref="CheckmarxCommunicationException">
        /// An error occurred communicating with the Checkmarx server.
        /// </exception>
        public byte[] GetScanReport(long reportId)
        {
            return CallCheckmarxApi(() => SoapClient.GetScanReport(SessionId, reportId)).ScanResults;
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
        /// <exception cref="CheckmarxCommunicationException">
        /// An error occurred communicating with the Checkmarx server.
        /// </exception>
        public CxWSReportStatusResponse GetScanReportStatus(long reportId)
        {
            return CallCheckmarxApi(() => SoapClient.GetScanReportStatus(SessionId, reportId));
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
        /// <exception cref="CheckmarxErrorException">
        /// The Checkmarx API returned an unexpected error.
        /// </exception>
        /// <exception cref="CheckmarxCommunicationException">
        /// An error occurred communicating with the Checkmarx server.
        /// </exception>
        private Uri GetSdkUrl(string server)
        {
            if (string.IsNullOrWhiteSpace(server))
            {
                throw new ArgumentNullException(nameof(server));
            }

            BasicHttpBinding binding;
            EndpointAddress endpointAddress;
            CxWSResponseDiscovery response;

            binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            endpointAddress = new EndpointAddress(string.Format($"https://{Uri.EscapeUriString(server)}/Cxwebinterface/CxWsResolver.asmx"));

            try
            {
                using (CxWSResolverSoapClient client = new CxWSResolverSoapClient(binding, endpointAddress))
                {
                    response = client.GetWebServiceUrl(CxWsResolver.CxClientType.SDK, CheckmarxApiVersion);
                    if (!response.IsSuccesfull)
                    {
                        throw new CheckmarxErrorException(response.ErrorMessage);
                    }

                    return new Uri(response.ServiceURL, UriKind.Absolute);
                }
            }
            catch (CommunicationException ex)
            {
                throw new CheckmarxCommunicationException(ex.Message, ex);
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
            binding.MaxReceivedMessageSize = 4*1024*1024; // 4 MB
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

            return CallCheckmarxApi(() => SoapClient.Login(credentials, CheckmarxLocaleId)).SessionId;
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
        /// <exception cref="CheckmarxCommunicationException">
        /// An error occurred communicating with the Checkmarx server.
        /// </exception>
        private void Logout()
        {
            CallCheckmarxApi(() => SoapClient.Logout(SessionId));
        }

        /// <summary>
        /// Wrap Checkmarx API calls to handle errors and convert exceptions.
        /// </summary>
        /// <typeparam name="TResult">
        /// The return type of the call. Must inherit from <see cref="CxSDKWebService.CxWSBasicRepsonse"/>.
        /// </typeparam>
        /// <param name="apiCall">
        /// A <see cref="Func{TResult}"/> that actually does the API call. This cannot be null.
        /// </param>
        /// <returns>
        /// The call response.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// No argument can be null.
        /// </exception>
        /// <exception cref="CheckmarxErrorException">
        /// The Checkmarx API returned an unexpected error.
        /// </exception>
        /// <exception cref="CheckmarxCommunicationException">
        /// An error occurred communicating with the Checkmarx server.
        /// </exception>
        private TResult CallCheckmarxApi<TResult>(Func<TResult> apiCall)
            where TResult: CxSDKWebService.CxWSBasicRepsonse
        {
            if (apiCall == null)
            {
                throw new ArgumentNullException(nameof(apiCall));
            }

            try
            {
                TResult t = apiCall();
                if (!t.IsSuccesfull)
                {
                    throw new CheckmarxErrorException(t.ErrorMessage);
                }

                return t;
            }
            catch (CommunicationException ex)
            {
                // Wrap this exception so callers do not need WCF references.
                throw new CheckmarxCommunicationException(ex.Message, ex);
            }
        }
    }
}
