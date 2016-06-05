using CheckmarxReports.Reports;
using CommandLine;

namespace CheckmarxReports.CommandLineOptions
{
    /// <summary>
    /// Command line options.
    /// </summary>
    [Verb("not-false-positives", HelpText = "Show all results that are not false positives from the most recent scans for each project.")]
    public class NotFalsePositiveReportOptions: CheckmarxReportOptions
    {
        /// <summary>
        /// Output format. Optional.
        /// </summary>
        [Option('f', "output-format", Required = false, Default = OutputFormat.Html, HelpText = "The output format, either Html (default) or Csv.", MetaValue = "Html | Csv")]
        public OutputFormat OutputFormat{ get; set; }
    }
}
