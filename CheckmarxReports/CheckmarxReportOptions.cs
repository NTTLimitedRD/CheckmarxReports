using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace CheckmarxReports
{
    /// <summary>
    /// Base class for Checkmarx report commands.
    /// </summary>
    public abstract class CheckmarxReportOptions
    {
        /// <summary>
        /// Server name.
        /// </summary>
        [Option('s', "server", Required = true, HelpText = "Checkmarx server name. Assumes https.", MetaValue = "SERVER")]
        public string Server { get; set; }

        /// <summary>
        /// User name.
        /// </summary>
        [Option('u', "user-name", Required = true, HelpText = "User name.", MetaValue = "USER_NAME")]
        public string UserName { get; set; }

        /// <summary>
        /// Password.
        /// </summary>
        [Option('p', "password", Required = true, HelpText = "Password.", MetaValue = "PASSWORD")]
        public string Password { get; set; }

        /// <summary>
        /// Output path. Optional.
        /// </summary>
        [Option('o', "output-file", Required = false, HelpText = "Output file path. If omitted, outputs to stdout.", MetaValue = "OUTPUT_FILE")]
        public string OutputPath { get; set; }
    }
}
