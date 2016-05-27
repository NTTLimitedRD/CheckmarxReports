using System;
using System.Collections.Generic;
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
                        ReportFactory reportFactory;

                        reportFactory = new ReportFactory(options.HostName, options.UserName, options.Password);
                        reportFactory.Run();

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
    }
}
