using Advantica.IntegrationSystem.Services;
using Advantica.IntegrationSystem.Protos;
using Microsoft.Extensions.Configuration;
using Advantica.IntegrationSystem.Options;
using System.Reflection;

namespace Advantica.IntegrationSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new IntegrationServiceOptions()
            {
                Url = config.GetSection("settings")["url"],
                MinimumInactiveTimePeriodMilliseconds = int.TryParse(config.GetSection("settings")["min_ms"], out int minMs) ? minMs : 0,
                MaximumInactiveTimePeriodMilliseconds = int.TryParse(config.GetSection("settings")["max_ms"], out int maxMs) ? maxMs : 0,
            };
            if (string.IsNullOrEmpty(options.Url) ||  options.MinimumInactiveTimePeriodMilliseconds <= 0
                || options.MaximumInactiveTimePeriodMilliseconds <= 0)
            {
                Console.WriteLine("invalid appsettings.json\npress Enter to exit...");
                Console.ReadLine();
                return;
            }


            var service = new IntegrationService(options);
            service.Start();

            Console.ReadLine();
            service.Dispose();

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }
    }
}