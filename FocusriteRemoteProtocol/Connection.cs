using System.Net;
using System.Net.Sockets;
using System.Xml.Linq;

namespace FocusriteRemoteProtocol;

public class Connection
{
    private TcpClient _client;
    private SaffireControlEndpoint _saffireControlEndpoint;
    private string _clientHostname;
    private string _clientKey;

    /// <summary>
    /// This is set by the remote end.
    /// </summary>
    private string? _clientId;

    private Dictionary<string, Device> _devices;

    private bool _authorised = false;

    public event EventHandler<Device> OnNewDevice;

    public Connection(SaffireControlEndpoint saffireControlEndpoint, string clientHostname, string clientKey)
    {
        _client = new TcpClient(AddressFamily.InterNetwork);
        _devices = new Dictionary<string, Device>();
        _saffireControlEndpoint = saffireControlEndpoint;
        _clientHostname = clientHostname;
        _clientKey = clientKey;
    }

    public async Task Connect()
    {
        await _client.ConnectAsync(_saffireControlEndpoint.EndPoint);
        var clientDetailsMessage = new Message()
        {
            Xml = new XElement(
                "client-details",
                new XAttribute("hostname", _clientHostname),
                new XAttribute("client-key", _clientKey)
            )
        };
        await SendMessage(clientDetailsMessage);
    }

    public async Task SendMessage(Message message)
    {
        await _client.GetStream().WriteAsync(message.GetMessageBytes());
        await _client.GetStream().FlushAsync();
    }

    public async Task HandleMessage()
    {
        if (_client.Available <= 14)
        {
            return;
        }

        var message = await Message.FromStream(_client.GetStream());

        switch (message.Xml.Name.LocalName)
        {
            case "client-details":
                _clientId = message.Xml.Attribute("id").Value;
                break;

            case "device-arrival":
                var newDevices = message.Xml.Elements("device");
                foreach (var deviceXml in newDevices)
                {
                    var device = new Device(deviceXml);
                    if (_devices.ContainsKey(device.ID))
                    {
                        _devices[device.ID] = device;
                    }
                    else
                    {
                        _devices.Add(device.ID, device);
                    }

                    if (OnNewDevice != null)
                    {
                        OnNewDevice(this, device);
                    }

                    var deviceSubscribeMessage = new Message()
                    {
                        Xml = new XElement(
                            "device-subscribe",
                            new XAttribute("devid", device.ID),
                            new XAttribute("subscribe", "true")
                        )
                    };
                    await SendMessage(deviceSubscribeMessage);
                }
                break;

            case "set":
                var deviceId = message.Xml.Attribute("devid").Value;
                if (!_devices.ContainsKey(deviceId))
                {
                    // unexpected device?
                    return;
                }
                _devices[deviceId].UpdateFromSetXml(message.Xml);
                File.WriteAllText($"dump_{deviceId}.xml", _devices[deviceId].Xml.ToString());

                break;

            case "approval":
                if (message.Xml.Attribute("id").Value != _clientId)
                {
                    // it's not us!
                    return;
                }

                _authorised = bool.Parse(message.Xml.Attribute("authorised").Value);
                break;

            case "keep-alive":
                break;

            default:
                File.WriteAllText($"dump_{message.Xml.Name.LocalName}.xml", message.Xml.ToString());
                throw new NotImplementedException($"Unexpected xml element {message.Xml.Name}");
        }
    }
}
