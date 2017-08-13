using System.Configuration;
using System.IO;
using System.Xml;

namespace ModMonitor.Utils
{
    internal class PortableSettingsProvider : SettingsProvider
    {
        const string SETTINGSROOT = "Settings";
        
        public PortableSettingsProvider(string baseDir)
        {
            BaseDir = baseDir;
        }

        public override string ApplicationName { get; set; }

        public string BaseDir { get; private set; }

        public override string Name
        {
            get { return "PortableSettingsProvider"; }
        }

        public string FileName
        {
            get
            {
                return ApplicationName + ".user.config";
            }
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection propvals)
        {
            foreach (SettingsPropertyValue propval in propvals)
            {
                SetValue(propval);
            }

            try
            {
                SettingsXML.Save(Path.Combine(BaseDir, FileName));
            }
            catch { } // Swallow
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection props)
        {
            SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();
            foreach (SettingsProperty setting in props)
            {
                SettingsPropertyValue value = new SettingsPropertyValue(setting);
                value.IsDirty = false;
                value.SerializedValue = GetValue(setting);
                values.Add(value);
            }
            return values;
        }

        private XmlDocument _settingsXML = null;

        private XmlDocument SettingsXML
        {
            get
            {
                if (_settingsXML == null)
                {
                    _settingsXML = new XmlDocument();

                    try
                    {
                        _settingsXML.Load(Path.Combine(BaseDir, FileName));
                    }
                    catch
                    {
                        XmlDeclaration dec = _settingsXML.CreateXmlDeclaration("1.0", "utf-8", string.Empty);
                        _settingsXML.AppendChild(dec);
                        XmlNode nodeRoot = default(XmlNode);
                        nodeRoot = _settingsXML.CreateNode(XmlNodeType.Element, SETTINGSROOT, "");
                        _settingsXML.AppendChild(nodeRoot);
                    }
                }

                return _settingsXML;
            }
        }

        private string GetValue(SettingsProperty setting)
        {
            string returnVal = "";

            try
            {
                returnVal = SettingsXML.SelectSingleNode(SETTINGSROOT + "/" + setting.Name).InnerText;
            }
            catch
            {
                if (setting.DefaultValue != null)
                {
                    returnVal = setting.DefaultValue.ToString();
                }
                else
                {
                    returnVal = "";
                }
            }

            return returnVal;
        }

        private void SetValue(SettingsPropertyValue propVal)
        {
            XmlElement SettingNode = default(XmlElement);
           
            try
            {
                SettingNode = (XmlElement)SettingsXML.SelectSingleNode(SETTINGSROOT + "/" + propVal.Name);
            }
            catch
            {
                SettingNode = null;
            }

            if (SettingNode != null)
            {
                SettingNode.InnerText = propVal.SerializedValue.ToString();
            }
            else
            {
                SettingNode = SettingsXML.CreateElement(propVal.Name);
                SettingNode.InnerText = propVal.SerializedValue.ToString();
                SettingsXML.SelectSingleNode(SETTINGSROOT).AppendChild(SettingNode);
            }
        }
    }
}
