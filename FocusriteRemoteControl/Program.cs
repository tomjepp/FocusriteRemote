using System.Xml.Linq;
using FocusriteRemoteProtocol;

Console.WriteLine("Running discovery...");
var discovery = new Discovery();
var endpoints = discovery.DiscoverEndpoints(1);

SaffireControlEndpoint endpoint = null;

foreach (var candidateEndpoint in endpoints)
{
    if (candidateEndpoint.Name == "mini-desktop")
    {
        endpoint = candidateEndpoint;
        break;
    }
}

if (endpoint == null)
{
    throw new Exception("Could not find mini-desktop!");
}

string clientHostname = "tom-mini";
string clientKey = "7b64b496-c396-49c6-bcc7-470351858a0c";

Console.WriteLine("Connecting...");
var connection = new Connection(endpoint, clientHostname, clientKey);
await connection.Connect();

Console.WriteLine("Handling messages...");
var nextKeepAlive = DateTime.Now.AddSeconds(2);
while (true)
{
    await connection.HandleMessage();
    if (DateTime.Now > nextKeepAlive)
    {
        await connection.SendMessage(new Message() { Xml = new XElement("keep-alive") });
        nextKeepAlive = DateTime.Now.AddSeconds(3);
    }
}
