using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace DAM_Export
{
    class ConfigHandler
    {
        public ConfigHandler()
        {

        }

        public String GetKeyValue(String strKey)
        {
            String strKeyValue = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings[strKey]);
            if (strKeyValue == null)
            {
                strKeyValue = "";
                AddKeySettingToConfigFile("", strKey);
            }
            return strKeyValue;
        }
        
        public void RemoveSetting(String strKeyName)
        {
            RemoveKeySettingOnConfigFile(strKeyName);
        }

        public void UpdateKeyValue(String strValue, String strKey)
        {
            UpdateKeyValueOnConfigFile(strValue, strKey);
        }

        private void RemoveKeySettingOnConfigFile(String strKeyName)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove(strKeyName);
            config.Save(ConfigurationSaveMode.Full);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void AddKeySettingToConfigFile(String strValue, String strKeyName)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Add(strKeyName, strValue);
            config.Save(ConfigurationSaveMode.Full);
            ConfigurationManager.RefreshSection("appSettings");
            config.AppSettings.Settings[strKeyName].Value = strValue;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void DeleteKeyOnConfigFile(String strKeyName)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove(strKeyName);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void UpdateKeyValueOnConfigFile(String strValue, String strKeyName)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[strKeyName].Value = strValue;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }


    }
}
