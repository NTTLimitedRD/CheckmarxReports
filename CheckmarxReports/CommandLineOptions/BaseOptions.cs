using CommandLine;

namespace CheckmarxReports.CommandLineOptions
{
    public abstract class BaseOptions
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
