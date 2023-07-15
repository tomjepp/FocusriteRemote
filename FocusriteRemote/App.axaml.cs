using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FocusriteRemoteProtocol;
using HarfBuzzSharp;
using System.Linq;
using System.Threading.Tasks;

namespace FocusriteRemote;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Program.Configuration.LoadConfiguration();
            var endpoint = Program.SaffireControlEndpoints.Where(x => x.Name == Program.Configuration.RemoteDeviceName).FirstOrDefault();

            if (Program.Configuration.DoesConfigurationExist() && endpoint != null)
            {
                Program.Connection = new Connection(endpoint, Program.Configuration.ThisDeviceName, Program.Configuration.ThisDeviceKey);
                desktop.MainWindow = new MainWindow();
            }
            else
            {
                desktop.MainWindow = new FirstStartWindow();
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}