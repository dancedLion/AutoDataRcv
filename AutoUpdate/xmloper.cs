using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
namespace AutoUpdate
{
    class xmloper
    {
        public static string settingFile = AppDomain.CurrentDomain.BaseDirectory + "UpdateSettings.xml";
        public static string errorLog = AppDomain.CurrentDomain.BaseDirectory + "logs\\AUError.log";
        #region 服务器设置与获取
        /// <summary>
        /// 保存自动更新服务器设置
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static int saveAUSSetting(Dictionary<string,string> settings)
        {
            int ret = -1;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(settingFile);
                XmlNodeList nodes = doc.DocumentElement.SelectNodes("Settings/Server[@Id=" + settings["Id"] + "]");
                XmlElement elem = null;
                if (nodes == null || nodes.Count > 0)
                {
                    elem = (XmlElement)nodes[0];
                }
                else
                {
                    elem = doc.CreateElement("Server");
                    doc.DocumentElement.SelectSingleNode("Settings").AppendChild(elem);
                }
                foreach(KeyValuePair<string,string> pair in settings)
                {
                    elem.SetAttribute(pair.Key, pair.Value);
                }
                doc.Save(settingFile);
                ret = 0;
            }
            catch(Exception ex)
            {
                WriteErrorMessage(errorLog, "saveNetworkShareAUS Settings Error:" + ex.Message);
            }
            return ret;
        }
        /// <summary>
        /// 获取全部服务器设置列表
        /// </summary>
        /// <returns></returns>
        public static List<Dictionary<string,string>> getAUSSetingsList()
        {
            List<Dictionary<string, string>> aussList = new List<Dictionary<string, string>>();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(settingFile);
                Dictionary<string, string> auss;
                XmlNodeList nodes = doc.SelectNodes("Settings/Server");
                foreach(XmlNode node in nodes)
                {
                    auss = new Dictionary<string, string>();
                    foreach(XmlAttribute att in node.Attributes)
                    {
                        auss.Add(att.Name, att.Value);
                    }
                    aussList.Add(auss);
                }
            }
            catch(Exception ex)
            {
                WriteErrorMessage(errorLog, "getAUSSetingsIDList Error:" + ex.Message);
            }
            return aussList;
        }
        /// <summary>
        /// 根据ID获取单个服务器设置
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public static Dictionary<string,string> getAUSSetting(int serverId)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(settingFile);
                XmlNode node = doc.SelectSingleNode("Settings/Server[@Id=" + serverId + "]");
                if (node != null)
                {
                    foreach(XmlAttribute att in node.Attributes)
                    {
                        ret.Add(att.Name, att.Value);
                    }
                }
            }
            catch(Exception ex)
            {
                WriteErrorMessage(errorLog, "getAUSSetings Error(Id="+serverId+"):" + ex.Message);
            }
            return ret;
        }
        #endregion

        #region 客户端设置与获取

        #endregion

        #region 文件操作

        #endregion

        #region 日志写入
        static bool isFileExists(string filename)
        {
            try
            {
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "logs"))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "logs");
                }
                if (!File.Exists(filename))
                {
                    File.Create(filename).Close();  //文件创建后默认是打开的，一定要关闭
                }
                //如果文件大于50M
                FileInfo fileinfo = new FileInfo(filename);
                if (fileinfo.Length > 10 * 1024 * 1024)
                {
                    File.Move(filename,
                        AppDomain.CurrentDomain.BaseDirectory + "logs\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "log.txt");
                    //然后创建:
                    if (!File.Exists(filename))
                    {
                        File.Create(filename).Close();  //文件创建后默认是打开的，一定要关闭
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 写入默认文件
        /// </summary>
        /// <param name="message"></param>
        public static void WriteMessage(string message)
        {
            WriteMessage(errorLog, message);
        }
        public static void WriteErrorMessage(string message)
        {
            WriteMessage(errorLog, message);
        }
        public static void WriteErrorMessage(string filename, string message)
        {
            WriteMessage(filename, message);
        }
        /// <summary>
        /// 写入指定文件
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="message"></param>
        public static void WriteMessage(string filename, string message)
        {
            try
            {
                if (isFileExists(filename))
                {
                    FileStream fs = new FileStream(filename, FileMode.Append, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.Write(DateTime.Now.ToString() + "::" + message + "\r\n");
                    sw.Flush();
                    sw.Close();
                    fs.Close();
                }
            }
            catch
            {
            }
        }
        #endregion

    }
}
