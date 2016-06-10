using CommandLine;

namespace CheckmarxReports.CommandLineOptions
{
    [Verb("raw-csv-scan-results", HelpText = "Raw scan result CSV for the latest scan for each project.")]
    public class RawScanResultCsvOptions : CheckmarxReportOptions
    {
    }
}
