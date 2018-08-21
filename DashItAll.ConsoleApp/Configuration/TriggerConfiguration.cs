using System;
using System.Collections.Generic;

namespace DashItAll.ConsoleApp.Configuration
{
    public class TriggerConfiguration
    {
        public string SourceMacAddress { get; set; }
        public PacketType PacketType { get; set; }
        public TimeSpan IgnoreThreshold { get; set; }
        public string ActionName { get; set; }
    }
}
