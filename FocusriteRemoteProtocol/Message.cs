using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;

namespace FocusriteRemoteProtocol;

using System.Text;

public class Message
{
    public int Length { get; set; }
    public XElement Xml { get; set; }

    public static Message FromDatagram(byte[] datagram)
    {
        var message = new Message();
        string receivedString = Encoding.UTF8.GetString(datagram);
        Trace.WriteLine($"RECV: {receivedString}");
        if (receivedString.StartsWith("Length="))
        {
            message.Length = int.Parse(receivedString.Substring(7, 6), NumberStyles.HexNumber);
            message.Xml = XElement.Parse(receivedString.Substring(14));
        }
        else
        {
            throw new InvalidDataException($"Unable to parse message: {receivedString}");
        }

        return message;
    }

    public static async Task<Message> FromStream(Stream stream)
    {
        var buffer = new byte[32768];
        int totalToRead = 14;
        int totalReceived = 0;
        while (totalReceived < totalToRead)
        {
            totalReceived += await stream.ReadAsync(buffer, totalReceived, totalToRead - totalReceived);
        }
        string lengthString = Encoding.UTF8.GetString(buffer, 0, 14);
        if (!lengthString.StartsWith("Length="))
        {
            throw new InvalidDataException($"Unable to parse message! Received {lengthString}");
        }

        var message = new Message();
        message.Length = int.Parse(lengthString.Substring(7, 6), NumberStyles.HexNumber);
        if (message.Length > buffer.Length)
        {
            buffer = new byte[message.Length];
        }

        totalToRead = message.Length;
        totalReceived = 0;
        while (totalReceived < totalToRead)
        {
            totalReceived += await stream.ReadAsync(buffer, totalReceived, totalToRead - totalReceived);
        }
        string xmlString = Encoding.UTF8.GetString(buffer, 0, message.Length);
        Trace.WriteLine($"RECV: {lengthString}{xmlString}");
        message.Xml = XElement.Parse(xmlString);

        return message;
    }

    public byte[] GetMessageBytes()
    {
        var utf8Encoding = new UTF8Encoding(false);
        var xmlString = Xml.ToString(SaveOptions.DisableFormatting | SaveOptions.OmitDuplicateNamespaces).Trim().Replace(" />", "/>");
        var xmlBytes = utf8Encoding.GetBytes(xmlString);
        var lengthString = String.Format("Length={0:x6} ", xmlBytes.Length);
        Trace.WriteLine($"SEND: {lengthString}{xmlString}");
        var lengthBytes = utf8Encoding.GetBytes(lengthString);
        var messageBytes = new byte[lengthBytes.Length + xmlBytes.Length];
        Array.Copy(lengthBytes, 0, messageBytes, 0, lengthBytes.Length);
        Array.Copy(xmlBytes, 0, messageBytes, lengthBytes.Length, xmlBytes.Length);
        return messageBytes;
    }
}
