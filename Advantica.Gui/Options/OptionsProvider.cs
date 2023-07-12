using Advantica.GrpcServiceProvider;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advantica.Gui.Options
{
    public class OptionsProvider
    {
        private IOptions? _options = null;


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
