using System.Net;
using System.Net.Sockets;
using System.Xml.Linq;

namespace FocusriteRemoteProtocol;

public class Discovery
{
    public SaffireControlEndpoint[] DiscoverEndpoints(int timeout = 5)
    {
        var udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
        var discoveryMessage = new Message();
        discoveryMessage.Xml = new XElement(
            "client-discovery",
            new XAttribute("app", "SAFFIRE-CONTROL"),
            new XAttribute("version", "4"),
            new XAttribute("device", "iOS")
        );
        var messageBytes = discoveryMessage.GetMessageBytes();

        udpClient.Send(messageBytes, new IPEndPoint(IPAddress.Broadcast, 30096));
        udpClient.Send(messageBytes, new IPEndPoint(IPAddress.Broadcast, 30097));
        udpClient.Send(messageBytes, new IPEndPoint(IPAddress.Broadcast, 30098));

        var saffireControlEndpoints = new List<SaffireControlEndpoint>();

        var timedOutAt = DateTime.Now.AddSeconds(timeout);

        while (true)
        {
            IPEndPoint source = null;
            if (udpClient.Available == 0)
            {
                if (DateTime.Now > timedOutAt)
                {
                    break;
                }

                continue;
            }

            var receivedBytes = udpClient.Receive(ref source);

            if (receivedBytes.Length == 0)
            {
                continue;
            }

            var message = Message.FromDatagram(receivedBytes);
            if (message.Xml.Name != "server-announcement")
            {
                // explode?
                throw new Exception("Unexpected discovery response.");
            }

            string hostname = message.Xml.Attribute("hostname").Value;
            int port = int.Parse(message.Xml.Attribute("port").Value);
            var serverEndPoint = new IPEndPoint(source.Address, port);
            saffireControlEndpoints.Add(new SaffireControlEndpoint() {EndPoint = serverEndPoint, Name = hostname});
        }

        return saffireControlEndpoints.ToArray();
    }
}
