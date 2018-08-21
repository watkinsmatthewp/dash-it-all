using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DashItAll.ConsoleApp.Configuration
{
    public class ProgramConfiguration
    {
        public string DeviceName { get; set; }
        public int IgnoreThresholdSeconds { get; set; } = 5;
        public List<TriggerConfiguration> Triggers { get; set; } = new List<TriggerConfiguration>();
        public List<ActionConfiguration> Actions { get; set; } = new List<ActionConfiguration>();

        [JsonIgnore]
        public TimeSpan IgnoreThreshold => TimeSpan.FromSeconds(IgnoreThresholdSeconds);
    }
}
