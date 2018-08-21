using CommandLine;
using DashItAll.ConsoleApp.Configuration;
using DashItAll.ConsoleApp.Network;
using DashItAll.ConsoleApp.Options;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DashItAll.ConsoleApp
{
    public static class Program
    {
        public static object ConfigurationManager { get; private set; }

        public static int Main(string[] args) => Parser.Default.ParseArguments<ConfigureActionOptions, RunActionOptions>(args).MapResult
        (
            (ConfigureActionOptions configureActionOptions) => Configure(configureActionOptions),
            (RunActionOptions runActionOptions) => Run(runActionOptions),
            errs => Error()
        );

        static int Configure(ConfigureActionOptions configureActionOptions)
        {
            var configuration = ConfigurationRepository.Exists(configureActionOptions.ConfigFilePath)
                ? ConfigurationRepository.Load(configureActionOptions.ConfigFilePath)
                : new ProgramConfiguration();

            AddEdit(configuration, c => c.DeviceName, () => NetworkMonitor.GetAllDeviceNames().ToArray());

            if (configuration.Actions.Count == 0 && configuration.Triggers.Count == 0)
            {
                configuration.Actions.Add(new ActionConfiguration
                {
                    Name = "be_a_thoughtful_husband",
                    URL = "https://maker.ifttt.com/trigger/send_email/with/key/your-key-here",
                    HttpMethod = "POST",
                    Body = "{ \"value1\": \"my-wife@gmail.com\", \"value2\": \"I love you\", \"value3\": \"I just pressed this button to show my love for you\" }"
                });
                configuration.Triggers.Add(new TriggerConfiguration
                {
                    ActionName = "be_a_thoughtful_husband",
                    PacketType = PacketType.Any,
                    SourceMacAddress = "B47C9C94C67F"
                });
            }

            Console.WriteLine($"Editing the triggers and actions are not yet supported. But examples have been added to {configureActionOptions.ConfigFilePath} so edit that file directly");

            ConfigurationRepository.Save(configureActionOptions.ConfigFilePath, configuration);

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

        static int Error()
        {
            Console.WriteLine("Usage: `DashItAll configure` or `DashItAll run`. Must use --c with the file path of the JSON");
            return 1;
        }

        static void AddEdit<T, TProp>(T obj, Expression<Func<T, TProp>> propertyExpression, Func<TProp[]> getOptions = null)
        {
            var memberExpression = propertyExpression.Body as MemberExpression;
            var propertyInfo = memberExpression.Member as PropertyInfo;
            var currentValue = propertyInfo.GetValue(obj);
            var currentValueString = currentValue?.ToString();
            var prompt = string.IsNullOrEmpty(currentValueString) ? "no value" : $"the value of \"{currentValueString}\"";
            Console.WriteLine($"{propertyInfo.Name} currently has {prompt}. Enter a new value, or enter to leave it as-is:");

            TProp[] options = null;
            if (getOptions != null)
            {
                options = getOptions();
                for (var i = 0; i < options.Length; i++)
                {
                    Console.WriteLine($"[{i + 1}] \"{options[i]}\"");
                }
            }

            var input = Console.ReadLine();

            if (input.Length > 0)
            {
                if (getOptions == null)
                {
                    var newValueConverted = Convert.ChangeType(input, typeof(TProp));
                    propertyInfo.SetValue(obj, newValueConverted);
                }
                else
                {
                    var selectedOption = options[int.Parse(input) - 1];
                    propertyInfo.SetValue(obj, selectedOption);
                }
            }
        }
    }
}
