using CommandLine;

namespace DashItAll.ConsoleApp.Options
{
    [Verb("discover", HelpText = "Listens to new devices joining the network")]
    class DiscoverActionOptions : ProgramOptions
    {
    }
}