using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace FRS_DUT
{
    class SettingClass
    {
        ConfigHandler config;
        Setting setting;
        public SettingClass()
        {
            config = new ConfigHandler();
        }

        public Setting GetSetting(String strViewName)
        {
            config = new ConfigHandler();
            setting = new Setting();
            LoadSetting(strViewName);

            return setting;
        }

        public void SaveSetting(Setting setting)
        {

            this.setting = setting;
            String strValue = "";
            string strApp = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            strValue = "";
            strValue = setting.strRSUserName;
            config.UpdateKeyValue(strValue, "RSUsername");

            strValue = "";
            strValue = setting.strRSUrl;
            config.UpdateKeyValue(strValue, "RSUrl");

            strValue = "";
            strValue = setting.strRSKey;
            config.UpdateKeyValue(strValue, "RSKey");

            strValue = "";
            strValue = setting.strRSSourceDir;
            config.UpdateKeyValue(strValue, "RSSourceDir");

            strValue = "";
            strValue = setting.strMetaIDContent;
            config.UpdateKeyValue(strValue, "MetaIDContent");

            strValue = "";
            strValue = setting.strMetaIDTitle;
            config.UpdateKeyValue(strValue, "MetaIDTitle");

            //            strValue = "";
            //          strValue = setting.strMetaIDUser;
            //        config.UpdateKeyValue(strValue, "MetaIDUser");

            strValue = "";
            strValue = setting.strMetaIDSubTitle;
            config.UpdateKeyValue(strValue, "MetaIDSubTitle");

            strValue = "";
            strValue = setting.strMetaIDPubDate;
            config.UpdateKeyValue(strValue, "MetaIDPubDate");

            strValue = "";
            strValue = setting.strMetaIDPage;
            config.UpdateKeyValue(strValue, "MetaIDPage");

            strValue = "";
            strValue = setting.strMetaIDDept;
            config.UpdateKeyValue(strValue, "MetaIDDept");

            strValue = "";
            strValue = setting.strMetaIDCreateDate;
            config.UpdateKeyValue(strValue, "MetaIDCreateDate");

            strValue = "";
            strValue = setting.strPicCollection;
            config.UpdateKeyValue(strValue, "PicCollection");

            strValue = "";
            strValue = setting.strTextCollection;
            config.UpdateKeyValue(strValue, "TextCollection");

            strValue = "";
            strValue = setting.strResourceType;
            config.UpdateKeyValue(strValue, "ResourceType");

            strValue = "";
            strValue = setting.strRelateAll;
            config.UpdateKeyValue(strValue, "RelateAll");
            strValue = "";
            strValue = setting.strRoot;
            config.UpdateKeyValue(strValue, "RootDir");
            strValue = "";
            strValue = setting.strLinuxUserName;
            config.UpdateKeyValue(strValue, "LinuxUsername");
            strValue = "";
            strValue = PasswordEncryption(setting.strLinuxPassword);
            config.UpdateKeyValue(strValue, "LinuxPassword");
            strValue = "";
            strValue = setting.strLinuxServer;
            config.UpdateKeyValue(strValue, "LinuxServer");
            strValue = "";
            strValue = setting.strLinuxPort;
            config.UpdateKeyValue(strValue, "LinuxPort");

/*
            strValue = "";
            strValue = setting.strDocExt;
            config.UpdateKeyValue(strValue, "Document_Ext");

            strValue = "";
            strValue = setting.strImageExt;
            config.UpdateKeyValue(strValue, "Image_Ext");

            strValue = "";
            strValue = setting.strAudioExt;
            config.UpdateKeyValue(strValue, "Audio_Ext");

            strValue = "";
            strValue = setting.strPDFExt;
            config.UpdateKeyValue(strValue, "PDF_Ext");

            strValue = "";
            strValue = setting.strVideoExt;
            config.UpdateKeyValue(strValue, "Video_Ext");
  */      
        }
        private void LoadSetting(String strViewName)
        {
            String strTemp = "";
            string strApp = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            strTemp = config.GetKeyValue("RSUsername");
            setting.strRSUserName = strTemp;

            strTemp = config.GetKeyValue("RSUrl");
            setting.strRSUrl = strTemp;

            strTemp = config.GetKeyValue("RSKey");
            setting.strRSKey = strTemp;

            strTemp = config.GetKeyValue("RSSourceDir");
            setting.strRSSourceDir = strTemp;

            strTemp = config.GetKeyValue("MetaIDTitle");
            setting.strMetaIDTitle = strTemp;

            strTemp = config.GetKeyValue("MetaIDContent");
            setting.strMetaIDContent = strTemp;

            strTemp = config.GetKeyValue("MetaIDSubTitle");
            setting.strMetaIDSubTitle = strTemp;

            strTemp = config.GetKeyValue("MetaIDPubDate");
            setting.strMetaIDPubDate = strTemp;

            strTemp = config.GetKeyValue("MetaIDPage");
            setting.strMetaIDPage = strTemp;

            strTemp = config.GetKeyValue("MetaIDDept");
            setting.strMetaIDDept = strTemp;

            strTemp = config.GetKeyValue("MetaIDCreateDate");
            setting.strMetaIDCreateDate = strTemp;

            strTemp = config.GetKeyValue("PicCollection");
            setting.strPicCollection = strTemp;

            strTemp = config.GetKeyValue("TextCollection");
            setting.strTextCollection = strTemp;

            strTemp = config.GetKeyValue("ResourceType");
            setting.strResourceType = strTemp;

            strTemp = config.GetKeyValue("RelateAll");
            setting.strRelateAll = strTemp;

            strTemp = config.GetKeyValue("LinuxUsername");
            setting.strLinuxUserName = strTemp;
            strTemp = config.GetKeyValue("LinuxPassword");
            setting.strLinuxPassword = PasswordDecryption(strTemp);
            strTemp = config.GetKeyValue("LinuxServer");
            setting.strLinuxServer = strTemp;
            strTemp = config.GetKeyValue("LinuxPort");
            setting.strLinuxPort = strTemp;

            strTemp = config.GetKeyValue("RootDir");
            setting.strRoot = strTemp;

/*
            strTemp = config.GetKeyValue("Document_Ext");
            setting.strDocExt = strTemp;

            strTemp = config.GetKeyValue("Image_Ext");
            setting.strImageExt = strTemp;

            strTemp = config.GetKeyValue("Audio_Ext");
            setting.strAudioExt = strTemp;

            strTemp = config.GetKeyValue("PDF_Ext");
            setting.strPDFExt = strTemp;

            strTemp = config.GetKeyValue("Video_Ext");
            setting.strVideoExt = strTemp;
*/
        }


        private bool LoadTrueFalseSetting(String strKey)
        {
            bool bResult = false;
            try
            {
                bResult = bool.Parse(config.GetKeyValue(strKey));
            }
            catch (Exception ex)
            {
                bResult = false;
            }
            return bResult;
        }

        private String PasswordEncryption(String strPassword)
        {
            String strDone = SSTCryptographer.Encrypt(strPassword, "founder");
            return strDone;
        }

        private String PasswordDecryption(String strPassword)
        {

            if (strPassword.Length == 0)
                return "";
            String strDone = SSTCryptographer.Decrypt(strPassword, "founder"); ;
            return strDone;
        }

        public void SaveSerialKey(String strSerialKey)
        {
            String strKey = config.GetKeyValue("SerialKey");
            config.UpdateKeyValue(strSerialKey, "SerialKey");
        }
    }
}

public class Setting
{
    public String strDBServer { get; set; }
    public String strDBUserName { get; set; }
    public String strDBPassword { get; set; }
    public String strDatabase { get; set; }
    public String strType { get; set; }
    public String strRoot { get; set; }
    public String strOutputDir { get; set; }
    public String strSourceDir { get; set; }
    public DateTime dtStart { get; set; }
    public DateTime dtEnd { get; set; }
    public String strLinuxServer { get; set; }
    public String strLinuxPort { get; set; }
    public String strLinuxUserName { get; set; }
    public String strLinuxPassword { get; set; }
    public String strRSUserName { get; set; }
    public String strRSKey { get; set; }
    public String strRSUrl { get; set; }
    public String strRSSourceDir { get; set; }
//    public String strMetaIDUser { get; set; }
    public String strMetaIDTitle { get; set; }
    public String strMetaIDContent { get; set; }
    public String strMetaIDSubTitle { get; set; }
    public String strMetaIDPubDate { get; set; }
    public String strMetaIDCreateDate { get; set; }
    public String strTextCollection { get; set; }
    public String strPicCollection { get; set; }
    public String strMetaIDPage { get; set; }
    public String strMetaIDDept { get; set; }
    public String strResourceType { get; set; }
    public String strRelateAll { get; set; }
   // public String strImageExt { get; set; }
//    public String strPDFExt { get; set; }
  //  public String strDocExt { get; set; }
   // public String strVideoExt { get; set; }
   // public String strAudioExt { get; set; }
}