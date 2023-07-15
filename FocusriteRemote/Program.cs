using Avalonia;
using FocusriteRemoteProtocol;
using System;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FocusriteRemote;

class Program
{
    public static Connection Connection;
    public static Configuration Configuration;
    public static SaffireControlEndpoint[] SaffireControlEndpoints;

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        Configuration = new Configuration();
        var discovery = new Discovery();
        SaffireControlEndpoints = discovery.DiscoverEndpoints();

        var app = BuildAvaloniaApp();
        app.StartWithClassicDesktopLifetime(args);
    }

    public static void StartConnection()
    {
        Task.Run(ConnectionHandler);
    }

    private async static Task ConnectionHandler()
    {
        await Connection.Connect();
        var nextKeepAlive = DateTime.Now.AddSeconds(2);
        while (true)
        {
            await Connection.HandleMessage();
            if (DateTime.Now > nextKeepAlive)
            {
                await Connection.SendMessage(new Message() { Xml = new XElement("keep-alive") });
                nextKeepAlive = DateTime.Now.AddSeconds(3);
            }
        }

    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}