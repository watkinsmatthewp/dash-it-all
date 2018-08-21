using CommandLine;

namespace DashItAll.ConsoleApp.Options
{
    class ProgramOptions
    {
        [Option('c', "config-file", Required = true, HelpText = "Location of the config file")]
        public string ConfigFilePath { get; set; }
    }
}
