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
        [Option('h', "host-name", Required = true, HelpText = "Checkmarx server name.")]
        public string HostName { get; set; }

        [Option('u', "user-name", Required = true, HelpText = "User name.")]
        public string UserName { get; set; }

        [Option('p', "password", Required = true, HelpText = "Password.")]
        public string Password { get; set; }
    }
}
