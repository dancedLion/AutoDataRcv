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
                XmlNodeList nodes = doc.DocumentElement.SelectNodes("Settings/Server[@ServerId=" + settings["ServerId"] + "]");
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
                WriteErrorMessage(errorLog, "saveAUS Settings Error:" + ex.Message);
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
                XmlNodeList nodes = doc.DocumentElement.SelectNodes("Settings/Server");
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
                XmlNode node = doc.DocumentElement.SelectSingleNode("Settings/Server[@ServerId=" + serverId + "]");
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
        /// <summary>
        /// 移除指定ID的服务器设置
        /// </summary>
        /// <param name="serverId">服务器ID</param>
        /// <returns></returns>
        public static int delAUSSetting(int serverId)
        {
            int ret = -1;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(settingFile);
                XmlNode node = doc.DocumentElement.SelectSingleNode("Settings/Server[@ServerId=" + serverId + "]");
                if (node != null)
                {
                    node.ParentNode.RemoveChild(node);
                }
                else
                {
                    throw new Exception("指定服务器未找到！");
                }
                ret = 0;
                doc.Save(settingFile);
            }
            catch(Exception ex)
            {
                WriteErrorMessage("delAUSSeting(ServerId="+serverId+") error:" + ex.Message);
            }
            return ret;
        }

        #endregion

        #region 客户端设置与获取
        /// <summary>
        /// 获取设置的当前自动更新服务器的设置
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string,string> getCurrentServerSetting()
        {
            Dictionary<string, string> ret = null;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(settingFile);
                XmlNode node = doc.DocumentElement.SelectSingleNode("Client/CurrentServer");
                if (node != null)
                {
                    ret=getAUSSetting(int.Parse(node.Attributes["Id"].Value));
                }
            }
            catch(Exception ex)
            {
                WriteErrorMessage("getCurrentServerSetting Error:" + ex.Message);
            }
            return ret;
        }

        public static string getCurrentUpdateType()
        {
            string ret = "Auto";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(settingFile);
                XmlNode node = doc.DocumentElement.SelectSingleNode("Client/CurrentServer");
                if (node == null)
                {
                    throw new Exception("未设置当前服务器设置！");
                }
                ret = node.Attributes["UpdateType"].Value;
            }
            catch(Exception ex)
            {
                WriteErrorMessage("getCurrentUpdateType Error:" + ex.Message);
            }
            return ret;
        }
        /// <summary>
        /// 保存客户端当前设定
        /// 服务器
        /// 更新方式
        /// </summary>
        /// <param name="cursetting"></param>
        /// <returns></returns>
        public static int saveCurrentSetting(Dictionary<string,string> cursetting)
        {
            int ret = -1;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(settingFile);
                XmlNode node = doc.DocumentElement.SelectSingleNode("Client/CurrentServer");
                XmlElement elem = null;
                if (node == null)
                {
                    elem = doc.CreateElement("CurrentServer");
                    doc.DocumentElement.SelectSingleNode("Client").AppendChild(elem);
                }
                else
                {
                    elem = (XmlElement)node;
                }
                foreach(KeyValuePair<string,string> kv in cursetting)
                {
                    elem.SetAttribute(kv.Key, kv.Value);
                }
                doc.Save(settingFile);
                ret = 0;
            }
            catch(Exception ex)
            {
                WriteErrorMessage("saveCurrentSetting Error:" + ex.Message);
            }
            return ret;
        }

        public static Dictionary<string,string> getClientSetting()
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(settingFile);
                XmlNode node = doc.DocumentElement.SelectSingleNode("Client/CurrentServer");
                if (node == null)
                {
                    throw new Exception("Cannot get the CurrentServer Setting!");
                }
                ret.Add("Id", node.Attributes["Id"].Value);
                ret.Add("UpdateType", node.Attributes["UpdateType"].Value);
            }
            catch(Exception ex)
            {
                WriteErrorMessage("getClientSetting Error:" + ex.Message);
            }
            return ret;
        }
        #endregion

        #region 文件操作
        /// <summary>
        /// 获取客户端所有文件信息
        /// </summary>
        /// <returns></returns>
        public static List<AUFileInfo> getClientFileInfos()
        {
            List<AUFileInfo> ret = new List<AUFileInfo>();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(settingFile);
                XmlNodeList nodes = doc.DocumentElement.SelectNodes("Files/File");
                foreach (XmlNode node in nodes)
                {
                    AUFileInfo auii = new AUFileInfo
                    {
                        FileId = node.Attributes["FileId"].Value,
                        FileName = node.Attributes["FileName"].Value,
                        FileVersion = node.Attributes["FileVersion"].Value,
                        FileSize = int.Parse(node.Attributes["FileSize"].Value),
                        FilePath = node.Attributes["FilePath"].Value
                    };
                    ret.Add(auii);
                }
            }
            catch (Exception ex)
            {
                WriteErrorMessage(errorLog, "getClientFileInfos Error:" + ex.Message);
            }
            return ret;
        }

        public static int saveClientFileInfo(AUFileInfo info)
        {
            int ret = -1;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(settingFile);
                XmlNodeList nodes = doc.DocumentElement.SelectNodes("Files/File[@FileId=" + info.FileId + "]");
                XmlElement elem = null;
                if (nodes == null || nodes.Count == 0)
                {
                    elem = doc.CreateElement("File");
                    elem.SetAttribute("FileId", info.FileId.ToString());
                    doc.DocumentElement.SelectSingleNode("Files").AppendChild(elem);
                }
                else
                {
                    elem = (XmlElement)nodes[0];
                }
                //设置属性
                elem.SetAttribute("FileName", info.FileName);
                elem.SetAttribute("FileSize", info.FileSize.ToString());
                elem.SetAttribute("FileVersion", info.FileVersion);
                elem.SetAttribute("FilePath", info.FilePath);
                doc.Save(settingFile);
            }
            catch (Exception ex)
            {
                WriteErrorMessage("saveClientFileInfo(filename=" + info.FileName + ") Error:" + ex.Message);
            }
            return ret;
        }


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
