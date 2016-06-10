using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CheckmarxReports.Checkmarx;
using CheckmarxReports.CommandLineOptions;
using CheckmarxReports.Credentials;
using CheckmarxReports.Reports;
using CommandLine;

namespace CheckmarxReports
{
    /// <summary>
    /// Entry point class.
    /// </summary>
    internal class Program
    {
        private const int ExitSuccess = 0;
        private const int ExitFailure = 1;

        /// <summary>
        /// Entry point.
        /// </summary>
        /// <param name="args">
        /// Command line args.
        /// </param>
        /// <returns>
        /// Program return value.
        /// </returns>
        private static int Main(string[] args)
        {
            int result = ExitFailure;
            ICredentialRepository credentialRepository = new FileCredentialRepository();

            try
            {
                Parser.Default
                    .ParseArguments<NotFalsePositiveReportOptions, RawScanResultXmlOptions, SaveCredentialsOptions, RawScanResultCsvOptions>(args)
                    .WithParsed<NotFalsePositiveReportOptions>(options =>
                        {
                            result = RunReport(new NotFalsePositiveResultsReportRunner(), GetReportResultFormatter(options),
                                credentialRepository, options.Server, options.UserName, options.Password, options.OutputPath);
                        })
                    .WithParsed<RawScanResultXmlOptions>(options =>
                        {
                            result = RunReport(new RawScanXmlReportRunner(), new TextStringFormatter(),
                                credentialRepository, options.Server, options.UserName, options.Password, options.OutputPath);
                        })
                    .WithParsed<RawScanResultCsvOptions>(options =>
                    {
                        result = RunReport(new RawScanCsvReportRunner(), new TextStringFormatter(),
                            credentialRepository, options.Server, options.UserName, options.Password, options.OutputPath);
                    })
                    .WithParsed<SaveCredentialsOptions>(options =>
                        {
                            result = SaveCredentials(credentialRepository, options.Server, options.UserName, options.Password);
                        })
                    .WithNotParsed(
                        errors =>
                        {
                            Console.Error.WriteLine(errors.First());
                        });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Run the report.
        /// </summary>
        /// <param name="reportRunner">
        /// A <see cref="IReportRunner{TReportResult}"/> to run the report. This cannot be null.
        /// </param>
        /// <param name="reportResultFormatter">
        /// A <see cref="IReportResultFormatter{TReportResult}"/> to format the report results. This cannot be null.
        /// </param>
        /// <param name="credentialRepository">
        /// Used to load previously saved credentials. Cannot be null.
        /// </param>
        /// <param name="server">
        /// The Checkmarx server name. Cannot be null, empty or whitespace.
        /// </param>
        /// <param name="suppliedUserName">
        /// The username to login with. Use the loaded value if null.
        /// </param>
        /// <param name="suppliedPassword">
        /// The password to login with. Use the loaded value if null.
        /// </param>
        /// <param name="outputPath">
        /// An optional file to write the output to.
        /// </param>
        /// <returns>
        /// The process return value.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="server"/> and <paramref name="suppliedUserName"/> cannot be null, empty or whitespace.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reportRunner"/>, <paramref name="reportResultFormatter"/> and <paramref name="credentialRepository"/> 
        /// and  cannot be null.
        /// </exception>
        private static int RunReport<TReportResult>(IReportRunner<TReportResult> reportRunner, IReportResultFormatter<TReportResult> reportResultFormatter, 
            ICredentialRepository credentialRepository, string server, string suppliedUserName, string suppliedPassword, string outputPath)
        {
            if (reportRunner == null)
            {
                throw new ArgumentNullException(nameof(reportRunner));
            }
            if (reportResultFormatter == null)
            {
                throw new ArgumentNullException(nameof(reportResultFormatter));
            }
            if (credentialRepository == null)
            {
                throw new ArgumentNullException(nameof(credentialRepository));
            }
            if (string.IsNullOrWhiteSpace(server))
            {
                throw new ArgumentException("Cannot be null, empty or whitespace", nameof(server));
            }

            string userName;
            string password;

            GetCredentials(credentialRepository, server, suppliedUserName, suppliedPassword, out userName, out password);

            using (Stream stream = string.IsNullOrWhiteSpace(outputPath)
                ? Console.OpenStandardOutput() : new FileStream(outputPath, FileMode.Create))
            using (StreamWriter output = new StreamWriter(stream, Encoding.UTF8))
            using (CheckmarxApiSession checkmarxApiSession = new CheckmarxApiSession(server, userName, password))
            {
                reportResultFormatter.Format(reportRunner.Run(checkmarxApiSession), output, server, userName);
            }

            return ExitSuccess;
        }

        /// <summary>
        /// Save user credentials.
        /// </summary>
        /// <param name="credentialRepository">
        /// The <see cref="ICredentialRepository"/> to save credentials to.
        /// </param>
        /// <param name="server">
        /// The Checkmarx server name. Cannot be null, empty or whitespace.
        /// </param>
        /// <param name="userName">
        /// The username to login with. Cannot be null, empty or whitespace.
        /// </param>
        /// <param name="password">
        /// The password to login with. Cannot be null.
        /// </param>
        /// <returns>
        /// The process return value.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="server"/>,  <paramref name="userName"/> and <paramref name="password"/> cannot 
        /// be null, empty or whitespace.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="credentialRepository"/> cannot be null.
        /// </exception>
        private static int SaveCredentials(ICredentialRepository credentialRepository, string server, string userName, string password)
        {
            if (credentialRepository == null)
            {
                throw new ArgumentNullException(nameof(credentialRepository));
            }
            if (string.IsNullOrWhiteSpace(server))
            {
                throw new ArgumentException("Argument is null or whitespace", nameof(server));
            }
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("Argument is null or whitespace", nameof(userName));
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Argument is null or whitespace", nameof(password));
            }
            
            credentialRepository.Save(server, userName, password);

            return ExitSuccess;
        }

        /// <summary>
        /// Get the appropruate <see cref="IReportResultFormatter{TReportResult}"/>.
        /// </summary>
        /// <param name="options">
        /// The command line options.
        /// </param>
        /// <returns>
        /// The requested <see cref="IReportResultFormatter{TReportResult}"/>.
        /// </returns>
        private static IReportResultFormatter<ScanResult> GetReportResultFormatter(NotFalsePositiveReportOptions options)
        {
            IReportResultFormatter<ScanResult> reportResultFormatter;

            switch (options.OutputFormat)
            {
                case OutputFormat.Html:
                    reportResultFormatter = new HtmlScanResultFormatter();
                    break;
                case OutputFormat.Csv:
                    reportResultFormatter = new CsvScanResultFormatter();
                    break;
                default:
                    throw new NotSupportedException("Unknown formatter");
            }

            return reportResultFormatter;
        }

        /// <summary>
        /// Use saved credentials if omitted.
        /// </summary>
        /// <param name="credentialRepository">
        /// A <see cref="ICredentialRepository"/> to load saved credentials from.
        /// </param>
        /// <param name="server">
        /// The server name. This cannot be null, empty or whitespace.
        /// </param>
        /// <param name="suppliedUserName">
        /// The user name given on the command line. Use the stored one if null.
        /// </param>
        /// <param name="suppliedPassword">
        /// The password given on the command line. Use the stored one if null.
        /// </param>
        /// <param name="userName">
        /// Receives the user name.
        /// </param>
        /// <param name="password">
        /// Received the password.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="server"/> cannot be null, empty or whitespace.
        /// </exception>
        /// <exception cref="CredentialNotFoundException">
        /// No credentials are specified or saved for that server.
        /// </exception>
        private static void GetCredentials(ICredentialRepository credentialRepository, 
            string server, string suppliedUserName, string suppliedPassword,
            out string userName, out string password)
        {
            if (string.IsNullOrWhiteSpace(suppliedUserName) || string.IsNullOrWhiteSpace(suppliedPassword))
            {
                credentialRepository.Load(server, out userName, out password);
            }
            else
            {
                userName = suppliedUserName;
                password = suppliedPassword;
            }
        }
    }
}
