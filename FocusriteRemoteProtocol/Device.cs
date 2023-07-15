using System.Xml.Linq;

namespace FocusriteRemoteProtocol;

public class Device
{
    public string ID
    {
        get
        {
            return _xml.Attribute("id").Value;
        }
    }
    public string Protocol
    {
        get
        {
            return _xml.Attribute("protocol").Value;
        }
    }
    public string Model
    {
        get
        {
            return _xml.Attribute("model").Value;
        }
    }
    public string Class
    {
        get
        {
            return _xml.Attribute("class").Value;
        }
    }
    public string BusId
    {
        get
        {
            return _xml.Attribute("bus-id").Value;
        }
    }
    public string SerialNumber
    {
        get
        {
            return _xml.Attribute("serial-number").Value;
        }
    }
    public string Version
    {
        get
        {
            return _xml.Attribute("version").Value;
        }
    }

    private XElement _xml;

    public XElement Xml
    {
        get
        {
            return _xml;
        }
    }

    public Device(XElement xml)
    {
        _xml = xml;
    }

    public XElement GetElementByID(string id)
    {
        foreach (var descendant in _xml.Descendants())
        {
            if (descendant.Attribute("id")?.Value == id)
            {
                return descendant;
            }
        }

        return null;
    }

    public void UpdateFromSetXml(XElement xml)
    {
        foreach (var element in xml.Elements())
        {
            if (element.Name.LocalName != "item")
            {
                throw new NotImplementedException();
            }

            string targetId = element.Attribute("id").Value;
            string newValue = element.Attribute("value").Value;

            if (targetId == "0")
            {
                continue;
            }

            var targetElement = this.GetElementByID(targetId);
            if (targetElement == null)
            {
                continue;
            }

            targetElement.Value = newValue;
        }
    }

    public void GetOutputs()
    {
        foreach (var element in _xml.Element("outputs").Elements())
        {
            switch (element.Name.LocalName)
            {
                case "analogue":
                    break;
                case "spdif-rca":
                    break;
                case "loopback":
                    break;
            }
        }
    }
}
