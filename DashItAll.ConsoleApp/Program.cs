using CommandLine;
using DashItAll.ConsoleApp.Configuration;
using DashItAll.ConsoleApp.Network;
using DashItAll.ConsoleApp.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace DashItAll.ConsoleApp
{
    public static class Program
    {
        public static int Main(string[] args) => Parser.Default.ParseArguments<ConfigureActionOptions, DiscoverActionOptions, RunActionOptions>(args).MapResult
        (
            (ConfigureActionOptions configureActionOptions) => Configure(configureActionOptions),
            (DiscoverActionOptions discoverActionOptions) => Discover(discoverActionOptions),
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

            Console.WriteLine("Available devices:");
            var availableDevices = NetworkMonitor.GetAllDeviceNames().ToArray();
            for (var i = 0; i < availableDevices.Length; i++)
            {
                Console.WriteLine($"[{i + 1}] {availableDevices[i]}");
            }

            Console.WriteLine("Make a selection: ");
            var selectedDeviceIndex = int.Parse(Console.ReadLine());

            var exampleConfiguration = new ProgramConfiguration
            {
                DeviceName = availableDevices[selectedDeviceIndex - 1],
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

        static int Discover(DiscoverActionOptions discoverActionOptions)
        {
            Console.WriteLine("Discovering...");
            var configuration = ConfigurationRepository.Load(discoverActionOptions.ConfigFilePath);
            var networkMonitor = new NetworkMonitor(configuration, new ActionExecutor());
            networkMonitor.StartDiscovery();
            return 0;
        }

        static int Run(RunActionOptions runActionOptions)
        {
            Console.WriteLine("Running...");
            var configuration = ConfigurationRepository.Load(runActionOptions.ConfigFilePath);

            Console.WriteLine("Triggers:");
            foreach (var trigger in configuration.Triggers)
            {
                Console.WriteLine($"{trigger.SourceMacAddress} => {trigger.ActionName}");
            }
            Console.WriteLine();

            var networkMonitor = new NetworkMonitor(configuration, new ActionExecutor());
            networkMonitor.StartMonitoring();
            return 0;
        }
    }
}
