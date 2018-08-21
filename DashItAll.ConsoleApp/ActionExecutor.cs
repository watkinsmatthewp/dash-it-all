using DashItAll.ConsoleApp.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DashItAll.ConsoleApp
{
    class ActionExecutor
    {
        static readonly HttpClient _client = new HttpClient();

        public async Task Execute(ActionConfiguration actionConfiguration)
        {
            Console.WriteLine($"Executing action {actionConfiguration.Name}");
            var httpMethod = new HttpMethod(actionConfiguration.HttpMethod);
            var message = new HttpRequestMessage(httpMethod, actionConfiguration.URL);
            if (!string.IsNullOrWhiteSpace(actionConfiguration.Body))
            {
                message.Content = new StringContent(actionConfiguration.Body);
            }
            await _client.SendAsync(message);
            Console.WriteLine($"Executed action {actionConfiguration.Name}");
        }
    }
}
