using CommandLine;
using DashItAll.ConsoleApp.Configuration;
using DashItAll.ConsoleApp.Network;
using DashItAll.ConsoleApp.Options;
using System;
using System.Collections.Generic;

namespace DashItAll.ConsoleApp
{
    public static class Program
    {
        public static object ConfigurationManager { get; private set; }

        public static int Main(string[] args) => Parser.Default.ParseArguments<ConfigureActionOptions, RunActionOptions>(args).MapResult
        (
            (ConfigureActionOptions configureActionOptions) => Configure(configureActionOptions),
            (RunActionOptions runActionOptions) => Run(runActionOptions),
            errs => 1
        );

        static int Configure(ConfigureActionOptions configureActionOptions)
        {
            if (ConfigurationRepository.Exists(configureActionOptions.ConfigFilePath))
            {
                Console.WriteLine($"{configureActionOptions.ConfigFilePath} already exists. Overwrite (Y/N)?");
                if (char.ToLowerInvariant(Console.ReadKey().KeyChar) != 'y')
                {
                    return 0;
                }
                Console.WriteLine();
            }

            var exampleConfiguration = new ProgramConfiguration
            {
                Actions = new List<ActionConfiguration>
                {
                    new ActionConfiguration
                    {
                        Name = "be_a_thoughtful_husband",
                        URL = "https://maker.ifttt.com/trigger/send_email/with/key/your-key-here",
                        HttpMethod = "POST",
                        Body = "{ \"value1\": \"my-wife@gmail.com\", \"value2\": \"I love you\", \"value3\": \"I just pressed this button to show my love for you\" }"
                    }
                },
                Triggers = new List<TriggerConfiguration>
                {
                    new TriggerConfiguration
                    {
                        ActionName = "be_a_thoughtful_husband",
                        PacketType = PacketType.Any,
                        SourceMacAddress = "B47C9C94C67F"
                    }
                }
            };

            Console.WriteLine($"Writing example configuration to {configureActionOptions.ConfigFilePath}");
            ConfigurationRepository.Save(configureActionOptions.ConfigFilePath, exampleConfiguration);
            Console.WriteLine("Done");

            return 0;
        }

        static int Run(RunActionOptions runActionOptions)
        {
            Console.WriteLine("Running...");
            var configuration = ConfigurationRepository.Load(runActionOptions.ConfigFilePath);
            var networkMonitor = new NetworkMonitor(configuration, new ActionExecutor());
            networkMonitor.Start();
            return 0;
        }
    }
}
