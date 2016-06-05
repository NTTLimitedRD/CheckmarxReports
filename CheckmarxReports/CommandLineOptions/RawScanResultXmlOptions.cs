using CommandLine;

namespace CheckmarxReports.CommandLineOptions
{
    [Verb("raw-xml-scan-results", HelpText = "Raw scan result XML for the latest scan for each project.")]
    public class RawScanResultXmlOptions: CheckmarxReportOptions
    {
    }
}
