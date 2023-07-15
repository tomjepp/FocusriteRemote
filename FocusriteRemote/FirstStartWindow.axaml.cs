using Avalonia.Controls;
using FocusriteRemoteProtocol;
using System;

namespace FocusriteRemote;

public partial class FirstStartWindow : Window
{
    private bool CanClose = false;

    public FirstStartWindow()
    {
        InitializeComponent();

        ThisDeviceName.Text = Environment.MachineName;
        
        Closing += FirstStartWindow_Closing;
        OKButton.Click += OKButton_Click;

        foreach (var endpoint in Program.SaffireControlEndpoints)
        {
            SelectedDevice.Items.Add(endpoint);
        }
    }

    private async void OKButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (SelectedDevice.SelectedItem is null)
        {
            return;
        }

        var selectedEndpoint = SelectedDevice.SelectedItem as SaffireControlEndpoint;

        Program.Configuration.ThisDeviceName = ThisDeviceName.Text;
        Program.Configuration.ThisDeviceKey = Guid.NewGuid().ToString();
        Program.Configuration.RemoteDeviceName = selectedEndpoint.Name;
        Program.Configuration.SaveConfiguration();

        CanClose = true;
        Close();
    }

    private void FirstStartWindow_Closing(object? sender, WindowClosingEventArgs e)
    {
        e.Cancel = !CanClose;
    }
}