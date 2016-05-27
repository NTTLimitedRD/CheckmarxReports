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
    public class CommandLineOptions
    {
        [Option('s', "server", Required = true, HelpText = "Checkmarx server name.", MetaValue = "SERVER")]
        public string HostName { get; set; }

        [Option('u', "user-name", Required = true, HelpText = "User name.", MetaValue = "USER_NAME")]
        public string UserName { get; set; }

        [Option('p', "password", Required = true, HelpText = "Password.", MetaValue = "PASSWORD")]
        public string Password { get; set; }

        [Option('o', "output-file", Required = false, HelpText = "Output file path.", MetaValue = "OUTPUT.TXT")]
        public string OutputPath { get; set; }
    }
}
