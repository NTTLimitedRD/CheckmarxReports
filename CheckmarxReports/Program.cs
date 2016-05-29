using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

            try
            {
                Parser.Default
                    .ParseArguments<NotFalsePositiveReportOptions, RawScanResultXmlOptions>(args)
                    .WithParsed<NotFalsePositiveReportOptions>(options =>
                        {
                            result = RunReport(new NotFalsePositiveResultsReportRunner(), GetReportResultFormatter(options), 
                                options.Server, options.UserName, options.Password, options.OutputPath);
                        })
                    .WithParsed<RawScanResultXmlOptions>(options =>
                        {
                            result = RunReport(new RawScanXmlReportRunner(), new TextStringFormatter(), 
                                options.Server, options.UserName, options.Password, options.OutputPath);
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
        /// <param name="server">
        /// The Checkmarx server name. Cannot be null, empty or whitespace.
        /// </param>
        /// <param name="userName">
        /// The username to login with. Cannot be null, empty or whitespace.
        /// </param>
        /// <param name="password">
        /// The password to login with. Cannot be null.
        /// </param>
        /// <param name="outputPath">
        /// An optional file to write the output to.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="server"/> and <paramref name="userName"/> cannot be null, empty or whitespace.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reportRunner"/>, <paramref name="reportResultFormatter"/> and <paramref name="password"/> and  cannot be null.
        /// </exception>
        private static int RunReport<TReportResult>(IReportRunner<TReportResult> reportRunner, IReportResultFormatter<TReportResult> reportResultFormatter, 
            string server, string userName, string password, string outputPath)
        {
            if (reportRunner == null)
            {
                throw new ArgumentNullException(nameof(reportRunner));
            }
            if (reportResultFormatter == null)
            {
                throw new ArgumentNullException(nameof(reportResultFormatter));
            }
            if (string.IsNullOrWhiteSpace(server))
            {
                throw new ArgumentException("Cannot be null, empty or whitespace", nameof(server));
            }
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("Cannot be null, empty or whitespace", nameof(userName));
            }
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

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
    }
}
