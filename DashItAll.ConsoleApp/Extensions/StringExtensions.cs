using System.Text;

namespace DashItAll.ConsoleApp.Extensions
{
    public static class StringExtensions
    {
        public static string FormatMacAddress(this string unformattedMacAddress)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < unformattedMacAddress.Length; i++)
            {
                sb.Append(unformattedMacAddress[i]);
                if (i < unformattedMacAddress.Length - 1 && i % 2 == 1)
                {
                    sb.Append(':');
                }
            }
            return sb.ToString();
        }
    }
}
