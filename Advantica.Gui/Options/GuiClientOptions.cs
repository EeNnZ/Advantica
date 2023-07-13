using Advantica.GrpcServiceProvider;

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
