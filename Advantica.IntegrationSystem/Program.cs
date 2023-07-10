using Advantica.IntegrationSystem.Services;
using Advantica.IntegrationSystem.Protos;
using Microsoft.Extensions.Configuration;
using Advantica.IntegrationSystem.Options;

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

            string? url = config.GetSection("url").Value;
            bool minMsParsed = int.TryParse(config.GetSection("min_ms").Value, out int minMs);
            bool maxMsParsed = int.TryParse(config.GetSection("max_ms").Value, out int maxMs);

            if (!minMsParsed || !maxMsParsed || string.IsNullOrEmpty(url))
            {
                Console.WriteLine("appsettings.json invalid, press Enter to exit...");
                Console.ReadLine();
                return;
            }

            var options = new IntegrationServiceOptions()
            {
                Url = url,
                MinimumInactiveTimePeriodMilliseconds = minMs,
                MaximumInactiveTimePeriodMilliseconds = maxMs
            };


            var service = new IntegrationService(options);
            service.Start();

            Console.ReadLine();
            service.Dispose();

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }
    }
}