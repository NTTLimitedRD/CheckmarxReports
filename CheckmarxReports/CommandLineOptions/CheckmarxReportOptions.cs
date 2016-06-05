using CommandLine;

namespace CheckmarxReports.CommandLineOptions
{
    /// <summary>
    /// Base class for Checkmarx report commands.
    /// </summary>
    public abstract class CheckmarxReportOptions: BaseOptions
    {
        /// <summary>
        /// Output path. Optional.
        /// </summary>
        [Option('o', "output-file", Required = false, HelpText = "Output file path. If omitted, outputs to stdout.", MetaValue = "OUTPUT_FILE")]
        public string OutputPath { get; set; }
    }
}
