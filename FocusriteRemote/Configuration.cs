using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FocusriteRemote
{
    internal class Configuration
    {
        private JObject _json = new JObject();

        private string ConfigurationFolderPath
        {
            get => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FocusriteRemote");
        }

        private string ConfigurationFilePath
        {
            get => Path.Combine(ConfigurationFolderPath, "config.json");
        }

        public string? ThisDeviceName
        {
            get
            {
                return _json["thisDeviceName"]?.Value<string>();
            }
            set
            {
                _json["thisDeviceName"] = value;
            }
        }

        public string? ThisDeviceKey
        {
            get
            {
                return _json["thisDeviceKey"]?.Value<string>();
            }
            set
            {
                _json["thisDeviceKey"] = value;
            }
        }

        public string? RemoteDeviceName
        {
            get
            {
                return _json["remoteDeviceName"]?.Value<string>();
            }
            set
            {
                _json["remoteDeviceName"] = value;
            }
        }

        public bool DoesConfigurationExist()
        {
            return File.Exists(ConfigurationFilePath);
        }

        public void LoadConfiguration()
        {
            if (!DoesConfigurationExist())
                return;

            _json = JObject.Parse(File.ReadAllText(ConfigurationFilePath));
        }

        public void SaveConfiguration()
        {
            if (!Directory.Exists(ConfigurationFolderPath))
            {
                Directory.CreateDirectory(ConfigurationFolderPath);
            }

            File.WriteAllText(ConfigurationFilePath, _json.ToString());
        }
    }
}
