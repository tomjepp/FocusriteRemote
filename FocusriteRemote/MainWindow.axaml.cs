using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Threading;
using FocusriteRemoteProtocol;
using FocusriteRemoteProtocol.DeviceItems;

namespace FocusriteRemote;

public partial class MainWindow : Window
{
    private Device _device;
    private bool _hadFirstUpdate = false;
    private IOutput[] Outputs
    {
        get
        {
            return _device.GetOutputs();
        }
    }
    
    public MainWindow()
    {
        InitializeComponent();

        Program.Connection.OnNewDevice += Connection_OnNewDevice;
        Program.StartConnection();
    }

    private void Connection_OnNewDevice(object? sender, Device e)
    {
        _device = e;
        _device.OnUpdate += _device_OnUpdate;
    }

    private async void _device_OnUpdate(object? sender, System.EventArgs e)
    {
        if (!_hadFirstUpdate)
        {
            _hadFirstUpdate = true;
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                TabControl.Items.Clear();
                foreach (var output in Outputs)
                {
                    var tabItem = new TabItem();
                    tabItem.Header = new TextBlock { Text = output.Name };
                    tabItem.Content = new StackPanel();
                    TabControl.Items.Add(tabItem);
                }
            });
        }
    }
}