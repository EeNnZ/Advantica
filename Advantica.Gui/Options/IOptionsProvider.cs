using Advantica.GrpcServiceProvider;

namespace Advantica.Gui.Options
{
    public interface IOptionsProvider
    {
        IOptions GetOptions();
    }
}