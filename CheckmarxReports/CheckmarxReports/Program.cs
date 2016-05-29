using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using CheckmarxReports.CxWsResolver;
using CommandLine;
using CommandLine.Text;

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
            ParserResult<CommandLineOptions> parserResult;

            parserResult = Parser.Default.ParseArguments<CommandLineOptions>(args);
            return parserResult.MapResult(
                options =>
                {
                    try
                    {
                        using (Stream stream = string.IsNullOrWhiteSpace(options.OutputPath)
                            ? Console.OpenStandardOutput() : new FileStream(options.OutputPath, FileMode.Create))
                        using (StreamWriter output = new StreamWriter(stream, Encoding.UTF8))
                        {
                            // We can add other reports in the future using commands or a "--report REPORT" option.
                            RunNotFalsePositiveReport(options.Server, options.UserName, options.Password, output);
                        }
                        return ExitSuccess;
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex.Message);
                        return ExitFailure;
                    }
                },
                errors =>
                {
                    Console.Error.WriteLine(errors.First());
                    return ExitFailure;
                });
        }

        /// <summary>
        /// Run the report.
        /// </summary>
        /// <param name="hostName">
        /// The Checkmarx server name. Cannot be null, empty or whitespace.
        /// </param>
        /// <param name="userName">
        /// The username to login with. Cannot be null, empty or whitespace.
        /// </param>
        /// <param name="password">
        /// The password to login with. Cannot be null.
        /// </param>
        /// <param name="output">
        /// A <see cref="TextWriter"/> to write the results to. Cannot be null.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="hostName"/> and <paramref name="userName"/> cannot be null, empty or whitespace.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="password"/> and <paramref name="output"/> cannot be null.
        /// </exception>
        private static void RunNotFalsePositiveReport(string hostName, string userName, string password, TextWriter output)
        {
            if (string.IsNullOrWhiteSpace(hostName))
            {
                throw new ArgumentException("Cannot be null, empty or whitespace", nameof(hostName));
            }
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("Cannot be null, empty or whitespace", nameof(userName));
            }
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            NotFalsePositiveResultsReportRunner reportFactory;
            ReportResultFormatter reportResultFormatter;
            IList<ScanResult> notFalsePositiveScanResults;

            reportFactory = new NotFalsePositiveResultsReportRunner();
            using (CheckmarxApiSession checkmarxApiSession = new CheckmarxApiSession(hostName, userName, password))
            {
                notFalsePositiveScanResults = reportFactory.Run(checkmarxApiSession);
            }

            reportResultFormatter = new ReportResultFormatter();
            reportResultFormatter.Format(notFalsePositiveScanResults, output, hostName, userName);
        }
    }
}
