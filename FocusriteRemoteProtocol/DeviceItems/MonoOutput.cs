using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FocusriteRemoteProtocol.DeviceItems
{
    public class MonoOutput : IOutput
    {
        private XElement _xml;

        public string Name
        {
            get
            {
                if (!String.IsNullOrEmpty(_xml.Element("nickname")?.Value))
                {
                    return $"{_xml.Element("nickname").Value} ({_xml.Attribute("name").Value})";
                }
                return _xml.Attribute("name").Value;
            }
        }

        public MonoOutput(XElement xml)
        {
            _xml = xml;
        }
    }
}
