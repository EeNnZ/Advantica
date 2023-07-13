using Advantica.Gui.Options;
using Advantica.Gui.ViewModels;
using Advantica.Gui.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace Advantica.Gui
{

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost? AppHost { get; private set; }
        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<AdvanticaWindow>();
                    services.AddSingleton<MainViewModel>();
                    services.AddTransient<IOptionsProvider, OptionsProvider>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();
            var startupWindow = AppHost.Services.GetRequiredService<AdvanticaWindow>();
            startupWindow.Show();

            base.OnStartup(e);
        }
        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();
            base.OnExit(e);
        }
    }
}
