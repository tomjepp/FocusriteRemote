using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FocusriteRemoteProtocol.DeviceItems
{
    public class StereoOutput : IOutput
    {
        private XElement _leftChannelXml;
        private XElement _rightChannelXml;

        public string Name
        {
            get
            {
                if (!String.IsNullOrEmpty(_leftChannelXml.Element("nickname")?.Value))
                {
                    return $"{_leftChannelXml.Element("nickname").Value} ({_leftChannelXml.Attribute("stereo-name").Value})";
                }
                return _leftChannelXml.Attribute("stereo-name").Value;
            }
        }

        public StereoOutput(XElement leftChannelXml, XElement rightChannelXml)
        {
            _leftChannelXml = leftChannelXml;
            _rightChannelXml = rightChannelXml;
        } 
    }
}
