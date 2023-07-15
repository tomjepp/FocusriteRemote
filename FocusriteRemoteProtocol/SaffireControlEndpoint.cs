using System.Net;
using System.Net.WebSockets;

namespace FocusriteRemoteProtocol;

public class SaffireControlEndpoint
{
    public IPEndPoint EndPoint { get; set; }
    public string Name { get; set; }

    public override string ToString()
    {
        return $"{Name} ({EndPoint})";
    }
}
