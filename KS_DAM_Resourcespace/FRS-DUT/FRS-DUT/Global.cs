using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Data.Common;
using System.IO;

namespace FRS_DUT
{
    class Global
    {
        public static void WriteToFile(String message, Boolean bFirst)
        {
            if (message.Equals(".") || message.Equals("\n"))
                return;
            try
            {
                String strFileName = CheckLogFile();
//                Encoding encode = System.Text.UnicodeEncoding.UTF8;
                Encoding utf8WithoutBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
                StreamWriter sw = new StreamWriter(strFileName, true, utf8WithoutBom);
//                StreamWriter sw = new StreamWriter(strFileName, true, encode);
                if (bFirst)
                    sw.WriteLine();
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss   ") + message);
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex) { }
            finally
            {
                // Release lock
            }
        }

        public static String CheckLogFile()
        {
            ConfigHandler config = new ConfigHandler();
            //serviceTimer.Stop();

            String strLogFileName = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf("\\")) + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + "_log.txt";
            //serviceTimer.Start();
            return strLogFileName;
        }

    }

    public class User
    {
        public string Ref { get; set; }
        public string username { get; set; }
    }

    public class Collections
    {
        public string Ref { get; set; }
        public string name { get; set; }
    }

    public class ResourceTypes
    {
        public string Ref { get; set; }
        public string name { get; set; }
    }

    public class SearchResult
    {
        public string Ref { get; set; }
    }

    public class PicInfo
    {
        public string strPicName { get; set; }
        public string strSubTitle { get; set; }
    }
    
    public class PageInfo
    {
        public string Number { get; set; }
        public string Code { get; set; }
        public string Desc { get; set; }
        public string Category { get; set; }
    }


}
