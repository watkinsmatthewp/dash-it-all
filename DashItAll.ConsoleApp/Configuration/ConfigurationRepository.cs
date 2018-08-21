using DashItAll.ConsoleApp.Extensions;
using System.IO;

namespace DashItAll.ConsoleApp.Configuration
{
    public static class ConfigurationRepository
    {
        public static bool Exists(string filePath)
            => File.Exists(filePath);

        public static ProgramConfiguration Load(string filePath)
            => File.ReadAllText(filePath).ParseJson<ProgramConfiguration>();

        public static void Save(string filePath, ProgramConfiguration configuration)
            => File.WriteAllText(filePath, configuration.ToJson(true, true));
    }
}
