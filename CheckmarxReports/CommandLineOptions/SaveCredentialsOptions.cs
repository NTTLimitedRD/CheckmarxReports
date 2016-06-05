using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace CheckmarxReports.CommandLineOptions
{
    [Verb("save", HelpText = "Save credentials for a server.")]
    public class SaveCredentialsOptions : BaseOptions
    {
    }
}
