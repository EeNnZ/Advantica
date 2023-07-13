using Advantica.GrpcServiceProvider;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Advantica.Gui.Options
{
    public class OptionsProvider : IOptionsProvider
    {
        private IOptions? _options;


        public OptionsProvider() { }
        public IOptions GetOptions()
        {
            if (_options == null)
            {
                var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
                string url = config.GetSection("settings")["url"] ?? "";
                _options = new GuiClientOptions() { Url = url };
            }
            return _options;
        }
    }
}
