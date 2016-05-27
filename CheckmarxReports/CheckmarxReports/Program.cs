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
                            RunReport(options.HostName, options.UserName, options.Password, output);
                        }
                        return 0;
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex.Message);
                        return 1;
                    }
                },
                errors =>
                {
                    Console.Error.WriteLine(errors.First());
                    return 1;
                });
        }

        /// <summary>
        /// Run the report.
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="output"></param>
        private static void RunReport(string hostName, string userName, string password, TextWriter output)
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
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            ReportFactory reportFactory;

            reportFactory = new ReportFactory(hostName, userName, password);
            reportFactory.Run(output);
        }
    }
}
