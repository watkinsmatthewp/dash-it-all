using SharpPcap;
using System;
using System.Linq;

namespace DashItAll.ConsoleApp.Extensions
{
    public static class CaptureDeviceExtensions
    {
        public static string GetName(this ICaptureDevice captureDevice)
        {
            foreach (var line in captureDevice.ToString().Split('\n'))
            {
                if (line.Contains("FriendlyName"))
                {
                    var firstColonIndex = line.IndexOf(':');
                    return line.Substring(firstColonIndex + 1, line.Length - firstColonIndex - 1).Trim();
                }
            }

            throw new Exception("Could not get name of device");
        }
    }
}
