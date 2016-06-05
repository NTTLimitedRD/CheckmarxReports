using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace CheckmarxReports.CommandLineOptions
{
    [Verb("save", HelpText = "Save credentials for a server.")]
    public class SaveCredentialsOptions
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
    }
}
