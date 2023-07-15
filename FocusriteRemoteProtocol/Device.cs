using FocusriteRemoteProtocol.DeviceItems;
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

    public event EventHandler OnUpdate;

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

        if (OnUpdate != null)
        {
            OnUpdate(this, new EventArgs());
        }
    }

    private IOutput[]? _outputs;

    public IOutput[] GetOutputs()
    {
        if (_outputs != null)
        {
            return _outputs;
        }

        var outputs = new List<IOutput>();

        var xmlOutputs = _xml.Element("outputs").Elements();
        for (int i = 0; i < xmlOutputs.Count(); i++)
        {
            var element = xmlOutputs.ElementAt(i);

            bool isStereo = element.Element("stereo")?.Value == "true";
            if (isStereo)
            {
                outputs.Add(new StereoOutput(element, xmlOutputs.ElementAt(i + 1)));
                i++;
            }
            else
            {
                outputs.Add(new MonoOutput(element));
            }
        }

        _outputs = outputs.ToArray();
        return _outputs;
    }
}
