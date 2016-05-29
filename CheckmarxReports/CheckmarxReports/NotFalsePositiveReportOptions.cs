using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace CheckmarxReports
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
        [Option('f', "output-format", Required = false, Default = OutputFormat.Html, HelpText = "The output format, either HTML (default) or CSV.", MetaValue = "html | csv")]
        public OutputFormat OutputFormat{ get; set; }
    }
}
