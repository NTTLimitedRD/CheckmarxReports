using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace CheckmarxReports
{
    [Verb("raw-xml-scan-results", HelpText = "Raw scan result XML for the latest scan for each project.")]
    public class RawScanResultXmlOptions: CheckmarxReportOptions
    {
    }
}
