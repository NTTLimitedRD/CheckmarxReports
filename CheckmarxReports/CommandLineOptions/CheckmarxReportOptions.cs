using System.Collections;
using CommandLine;
using System.Collections.Generic;

namespace CheckmarxReports.CommandLineOptions
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
        [Option('u', "user-name", Required = false, HelpText = "User name. Uses previously saved credentials for this server if omitted.", MetaValue = "USER_NAME")]
        public string UserName { get; set; }

        /// <summary>
        /// Password.
        /// </summary>
        [Option('p', "password", Required = false, HelpText = "Password. Uses previously saved credentials for this server if omitted.", MetaValue = "PASSWORD")]
        public string Password { get; set; }

        /// <summary>
        /// Output path. Optional.
        /// </summary>
        [Option('o', "output-file", Required = false, HelpText = "Output file path. If omitted, outputs to stdout.", MetaValue = "OUTPUT_FILE")]
        public string OutputPath { get; set; }

        /// <summary>
        /// Project to exclude from results.
        /// </summary>
        [Option('x', "exclude-project", Required = false, HelpText = "Project to exclude from results", MetaValue="PROJECT_NAME")]
        public IEnumerable<string> ExcludeProjects { get; set; }
    }
}
