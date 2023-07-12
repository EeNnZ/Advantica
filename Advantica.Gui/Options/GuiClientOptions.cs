using Advantica.GrpcServiceProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advantica.Gui.Options
{
    class GuiClientOptions : IOptions
    {
        private string _url = "";

        public string Url 
        {
            get => _url; 
            set => _url = value ?? "";
        }
    }
}
