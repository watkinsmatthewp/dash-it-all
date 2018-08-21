using DashItAll.ConsoleApp.Configuration;
using DashItAll.ConsoleApp.Extensions;
using PacketDotNet;
using SharpPcap;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DashItAll.ConsoleApp.Network
{
    class NetworkMonitor
    {
        readonly ProgramConfiguration _config;
        readonly ActionExecutor _actionExecutor;

        HashSet<string> _discoveredMacAddresses = new HashSet<string>();
        Dictionary<string, DateTime> _packetsLastReceived = new Dictionary<string, DateTime>();

        internal NetworkMonitor(ProgramConfiguration config, ActionExecutor actionExecutor)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _actionExecutor = actionExecutor ?? throw new ArgumentNullException(nameof(actionExecutor));
        }

        internal static IEnumerable<string> GetAllDeviceNames()
        {
            Console.WriteLine("Looking for available devices...");
            foreach (var captureDevice in CaptureDeviceList.Instance)
            {
                yield return captureDevice.GetName();
            }
        }

        internal void StartMonitoring()
        {
            var device = GetDevice();
            Console.WriteLine("Subscribing to monitored traffic...");
            device.OnPacketArrival += OnMonitoringPacketArrival;
            device.Open(DeviceMode.Promiscuous, 1000);
            device.Filter = CreatePcapFilter();
            Console.WriteLine("Starting capture...");
            device.StartCapture();
            Console.WriteLine("Capturing...");
        }

        internal void StartDiscovery()
        {
            var device = GetDevice();
            Console.WriteLine("Subscribing to all traffic");
            device.OnPacketArrival += OnDiscoveryPacketArrival;
            device.Open(DeviceMode.Promiscuous, 1000);
            Console.WriteLine("Starting capture...");
            device.StartCapture();
            Console.WriteLine("Capturing...");
        }

        #region Private helpers

        ICaptureDevice GetDevice()
        {
            Console.WriteLine($"Looking for device {_config.DeviceName}...");
            var discoveredDeviceNames = new HashSet<string>();
            foreach (var captureDevice in CaptureDeviceList.Instance)
            {
                var captureDeviceName = captureDevice.GetName();
                if (captureDeviceName == _config.DeviceName)
                {
                    return captureDevice;
                }
                discoveredDeviceNames.Add(captureDeviceName);
            }

            throw new Exception($"Could not find device {_config.DeviceName}. Available devices: {string.Join(", ", discoveredDeviceNames)}");
        }

        string CreatePcapFilter() => "ether host " + string.Join(" or ", _config.Triggers.Select(t => t.SourceMacAddress).Distinct().Select(m => m.FormatMacAddress()));

        async void OnMonitoringPacketArrival(object sender, CaptureEventArgs e)
        {
            var received = DateTime.UtcNow;
            if (e.Packet.LinkLayerType == LinkLayers.Ethernet)
            {
                var ethernetPacket = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data) as EthernetPacket;
                var sourceMacAddress = ethernetPacket.SourceHwAddress.ToString();

                var ignore = false;
                lock (_packetsLastReceived)
                {
                    ignore = _packetsLastReceived.ContainsKey(sourceMacAddress) && _packetsLastReceived[sourceMacAddress] + _config.IgnoreThreshold > received;
                    _packetsLastReceived[sourceMacAddress] = received;
                    _packetsLastReceived = _packetsLastReceived.Where(kvp => kvp.Value + _config.IgnoreThreshold > received).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                }

                if (ignore)
                {
                    return;
                }

                var packetMatchType = ethernetPacket.Extract(typeof(ARPPacket)) is ARPPacket ? PacketType.JoinRequest : PacketType.Any;

                var actionNamesToRun = _config.Triggers
                    .Where(t => t.SourceMacAddress == sourceMacAddress && t.PacketType == packetMatchType)
                    .Select(t => t.ActionName)
                    .Distinct();

                foreach (var actionName in actionNamesToRun)
                {
                    var action = _config.Actions.First(a => a.Name == actionName);
                    await _actionExecutor.Execute(action);
                }
            }
        }

        void OnDiscoveryPacketArrival(object sender, CaptureEventArgs e)
        {
            if (e.Packet.LinkLayerType == LinkLayers.Ethernet)
            {
                var ethernetPacket = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data) as EthernetPacket;
                if (ethernetPacket.Extract(typeof(ARPPacket)) is ARPPacket)
                {
                    var sourceMacAddress = ethernetPacket.SourceHwAddress.ToString();

                    var notify = false;
                    lock (_discoveredMacAddresses)
                    {
                        notify = !_discoveredMacAddresses.Add(sourceMacAddress);
                    }

                    if (notify)
                    {
                        Console.WriteLine(sourceMacAddress);
                    }
                }
            }
        }

        #endregion
    }
}
