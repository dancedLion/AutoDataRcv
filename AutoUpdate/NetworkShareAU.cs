using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Diagnostics;
namespace AutoUpdate
{

    public class AUCLient : IAutoUpdateClient
    {
        #region 变量和属性
        List<AUFileInfo> m_fileinfos;
        IAutoUpdateServer m_server;
        Dictionary<string, string> m_clientsetting;
        string m_localpath = AppDomain.CurrentDomain.BaseDirectory;
        public IAutoUpdateServer AUServer
        {
            get { return m_server; }
            set { m_server = value; }
        }
        public List<AUFileInfo> FileInfos
        {
            get { return m_fileinfos; }
            set { m_fileinfos = value; }
        }
        public Dictionary<string,string> ClientSetting
        {
            get { return m_clientsetting; }
            set { m_clientsetting = value; }
        }
        #endregion

        public AUCLient()
        {
            m_fileinfos = new List<AUFileInfo>();
            GetAllFileInfos();
            m_clientsetting = xmloper.getClientSetting();
            InitServer();
        }
        #region Interface Methods
        /// <summary>
        /// 自动更新
        /// </summary>
        /// <returns></returns>
        public virtual int AutoRunUpdate()
        {
            int ret = -1;
            try
            {
                if (m_server.ServerStatus > -1)
                {
                    foreach(AUFileInfo info in GetDownloadList())
                    {
                        if (DownloadFile(info) < 0)
                        {
                            xmloper.WriteErrorMessage(this.GetType().FullName + ".AutoRunupdate(Update(filename=" + info.FileName + ")) Error!");
                        }
                    }
                    ret = 0;
                }
                else
                {
                    throw new Exception("初始化服务器失败！");
                }
            }
            catch(Exception ex)
            {
                xmloper.WriteErrorMessage(this.GetType().FullName + ".AutoRunUpdate Error:" + ex.Message);
            }
            return ret;
        }

        /// <summary>
        /// 获取下载清单
        /// </summary>
        /// <returns></returns>
        public virtual List<AUFileInfo> GetDownloadList()
        {
            List<AUFileInfo> ret = new List<AUFileInfo>();
            try
            {
                foreach(AUFileInfo t in m_server.FileInfos)
                {
                    AUFileInfo l = m_fileinfos.Find(x => x.FileId == t.FileId);
                    if (l == null)
                    {
                        ret.Add(t);
                    }
                    else
                    {
                        if (l.FileVersion.CompareTo(t.FileVersion)<0)
                        {
                            ret.Add(t);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                xmloper.WriteErrorMessage(this.GetType().FullName + ".GetDownloadList Error:" + ex.Message);
            }
            return ret;
        }

        /// <summary>
        /// 净文件上传至服务器
        /// </summary>
        /// <param name="info">文件信息</param>
        /// <param name="fs">从本地读取的文件的缓冲</param>
        /// <returns></returns>
        public virtual int UploadFile(AUFileInfo info,byte[] fs)
        {
            return m_server.SetServerFile(info, fs);
        }
        /// <summary>
        /// 从服务器下载文件
        /// </summary>
        /// <param name="info">服务器配置文件信息</param>
        /// <returns></returns>
        //下载写到本地文件
        //同时更新本地XML文件版本
        public virtual int DownloadFile(AUFileInfo info)
        {
            int ret = -1;
            try
            {
                byte[] buff = m_server.PrepareFile(info.FileId);
                if (buff == null || buff.Length == 0)
                {
                    throw new Exception("获取文件失败！");
                }
                //准备写文件
                if (!Directory.Exists(m_localpath + info.FilePath))
                {
                    Directory.CreateDirectory(m_localpath + info.FilePath);
                }
                FileStream fs = new FileStream(m_localpath + info.FilePath + info.FileName, FileMode.OpenOrCreate, FileAccess.Write);
                fs.Write(buff, 0, buff.Length);
                fs.Flush();
                fs.Close();
                //更新本地信息
                AUFileInfo linfo = m_fileinfos.Find(t => t.FileId == info.FileId);
                if (linfo == null)
                {
                    m_fileinfos.Add(info);
                }
                else
                {
                    linfo = info;
                }
                //更新XML设置文件
                if (xmloper.saveClientFileInfo(info) < 0)
                {
                    throw new Exception("文件已经成功写入，但保存设置时出现错误！");
                }
                ret = 0;
            }
            catch(Exception ex)
            {
                xmloper.WriteErrorMessage(this.GetType().FullName + ".DownloadFile(" + info.FileName + "): Error:" + ex.Message);
            }
            return ret;
        }

        public virtual List<AUFileInfo> GetAllFileInfos()
        {
            return xmloper.getClientFileInfos();
        }
        public virtual AUFileInfo GetFileInfo(string fileId)
        {
            return m_fileinfos.Find(t => t.FileId == fileId);
        }
        
        public virtual int InitServer()
        {
            int ret = -1;
            try
            {
                Dictionary<string, string> serversettings = xmloper.getCurrentServerSetting();
                if(string.IsNullOrEmpty(serversettings["Type"]))
                {
                    throw new Exception("当前服务器类型未设置！");
                }
                Type servertype = Type.GetType(serversettings["Type"]);
                m_server = (IAutoUpdateServer)servertype.Assembly.CreateInstance(servertype.FullName);
                ret=m_server.LoadingSetting(int.Parse(serversettings["ServerId"]));
            }
            catch(Exception ex)
            {
                xmloper.WriteErrorMessage(this.GetType().FullName + ".InitServer Error:" + ex.Message);
            }
            return ret;
        }

        #endregion

        #region others
        #endregion
    }

    public class NetworkShareAUS : IAutoUpdateServer
    {
        #region properties
        string m_remotelocation = "";
        string m_fileinfofile = "";
        string m_remotepath = "";
        List <AUFileInfo> m_fileinfos;
        int m_status = -1;
        public int ServerStatus
        {
            get { return m_status; }
            set { m_status = value; }
        }
        public List<AUFileInfo> FileInfos
        {
            get { return m_fileinfos; }
            set { m_fileinfos = value; }
        }
        #endregion
        public NetworkShareAUS()
        {
            m_fileinfos = new List<AUFileInfo>();
        }

        #region Interface method and events
        public int LoadingSetting(int serverId)
        {
            int ret = -1;
            try
            {
                Dictionary<string, string> settings = xmloper.getAUSSetting(serverId);
                m_remotelocation = settings["Path"];
                m_remotepath = m_remotelocation + @"\";
                m_fileinfofile = settings["FileInfoSetting"];
                ret = PrepareEnv();
                m_status = ret;
                if (ret >= 0)
                {
                    m_fileinfos = GetAllFileInfos();
                }
            }
            catch(Exception ex)
            {
                xmloper.WriteErrorMessage(this.GetType().FullName + ".LoadingSetting (" + serverId + ") Error:"+ex.Message);
            }
            return ret;
        }
        /// <summary>
        /// 将上传的文件保存到服务器中
        /// </summary>
        /// <param name="into"></param>
        /// <param name="fs"></param>
        /// <returns></returns>
        public int SetServerFile(AUFileInfo info ,byte[] fs)
        {
            int ret = -1;
            try
            {
                int editmode =1;    //0-更新 1-新增
                foreach(AUFileInfo fi in m_fileinfos)
                {
                    if (fi.FileId == info.FileId) {
                        editmode = 0;
                        break;
                    }
                }
                
                //先写文件
                string filename = m_remotepath + info.FilePath + info.FileName;
                if (!Directory.Exists(m_remotepath + info.FilePath))
                {
                    Directory.CreateDirectory(m_remotepath + info.FilePath);
                }
                FileStream fstream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fstream.Write(fs, 0, fs.Length);
                fstream.Flush();
                fstream.Close();
                //写全局信息文件
                if (editmode == 0)
                {
                    AUFileInfo rinfo = m_fileinfos.Find(t => t.FileId == info.FileId);
                    rinfo.FileName = info.FileName;
                    rinfo.FilePath = info.FilePath;
                    rinfo.FileSize = info.FileSize;
                    rinfo.FileVersion = info.FileVersion;
                }
                else
                {
                    m_fileinfos.Add(new AUFileInfo
                    {
                        FileId=info.FileId,
                        FileName=info.FileName,
                        FileSize=info.FileSize,
                        FilePath=info.FilePath,
                        FileVersion=info.FileVersion
                    });
                }
                //写配置文件
                XmlDocument doc = new XmlDocument();
                doc.Load(m_remotepath + m_fileinfofile);
                XmlElement elem = null;
                XmlNode node = doc.SelectSingleNode("Files/File[@FileId=" + info.FileId + "]");
                if (node == null)
                {
                    elem = doc.CreateElement("File");
                    elem.SetAttribute("FileId", info.FileId.ToString());
                    doc.DocumentElement.SelectSingleNode("Files").AppendChild(elem);
                }
                else
                {
                    elem = (XmlElement)node;
                }
                elem.SetAttribute("FileName", info.FileName);
                elem.SetAttribute("FileSize", info.FileSize.ToString());
                elem.SetAttribute("FileVersion", info.FileVersion);
                elem.SetAttribute("FilePath", info.FilePath);
                doc.Save(m_remotepath + m_fileinfofile);
                ret = 0;
            }
            catch(Exception ex)
            {
                xmloper.WriteErrorMessage(this.GetType().FullName + ".SetServerFile(FileId=" + info.FileName + ") Error:" + ex.Message);
            }
            return ret;
        }
        public byte[] PrepareFile(string fileId)
        {
            byte[] ret = null;
            try
            {
                AUFileInfo auinfo = GetFileInfo(fileId);
                string filename = m_remotepath + auinfo.FilePath + auinfo.FileName;
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                ret = new byte[fs.Length];
                int numReading = 0;
                int numToReading = (int)fs.Length;
                while (numToReading > 0)
                {
                    int n = fs.Read(ret, numReading, numToReading);
                    if (n == 0)
                    {
                        break;
                    }
                    numReading += n;
                    numToReading -= n;
                }
                fs.Close();
            }
            catch (Exception ex)
            {
                xmloper.WriteErrorMessage(this.GetType().FullName + ".PrepareFile(" + fileId + ") Error:" + ex.Message);
            }
            return ret;
        }
        public int PrepareEnv()
        {
            int ret = -1;
            try
            {
                //可以访问
                if (!Cmd(@"net use "+m_remotelocation))
                {
                    throw new Exception("目标目录不存在或者不可访问！");
                }
                //文件信息文件存在,如不存在则创建新文件
                if (!File.Exists(m_remotepath + m_fileinfofile))
                {
                    File.Create(m_remotepath+m_fileinfofile).Close();
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8"" ?>" +
                        @"<FileInfos>" +
                        @"<Files>" +
                        @"</Files>" +
                        @"</FileInfos>");
                    doc.Save(m_remotepath + m_fileinfofile);
                }
                ret = 0;
            }
            catch(Exception ex)
            {
                xmloper.WriteErrorMessage(this.GetType().FullName + ".PrepareEnv Error:" + ex.Message);
            }
            return ret;
        }
        public List<AUFileInfo> GetAllFileInfos()
        {
            List<AUFileInfo> ret = new List<AUFileInfo>();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(m_remotepath + m_fileinfofile);
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
                xmloper.WriteErrorMessage(this.GetType().FullName + ".getServerFileInfos Error:" + ex.Message);
            }
            return ret;
        }
        public AUFileInfo GetFileInfo(string fileId)
        {
            if (m_fileinfos == null || m_fileinfos.Count == 0)
            {
                m_fileinfos = GetAllFileInfos();
            }
            AUFileInfo info = m_fileinfos.Find(t => t.FileId == fileId);
            return info;
        }
        #endregion

        #region others
        public string getServerFileName(int fileId)
        {
            string ret = "";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(m_remotepath + m_fileinfofile);
                XmlNode node = doc.DocumentElement.SelectSingleNode("Files/File[@FileId=" + fileId + "]");
                if (node != null)
                {
                    ret = node.Attributes["FilePath"].Value + node.Attributes["FileName"].Value;
                }
            }
            catch (Exception ex)
            {
                xmloper.WriteErrorMessage(this.GetType().FullName + ".getServerFileName(FileId=" + fileId + ") Error:" + ex.Message);
            }
            return ret;
        }

        /// <summary>
        /// 测试是否可以连接共享文件夹
        /// </summary>
        /// <param name="cmdLine"></param>
        /// <returns></returns>
        bool Cmd(string cmdLine)
        {

            using (var process = new Process
            {
                StartInfo =
                        {
                            FileName = "cmd.exe",
                            UseShellExecute = false,
                            RedirectStandardInput = true,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true,
                            RedirectStandardError = true
                        }
            })
            {
                process.Start();
                process.StandardInput.AutoFlush = true;
                process.StandardInput.WriteLine(cmdLine);
                process.StandardInput.WriteLine("exit");

                Debug.WriteLine(process.StandardOutput.ReadToEnd());

                String errorMessage = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (String.IsNullOrEmpty(errorMessage))
                {
                    return true;
                }

                Debug.WriteLine(errorMessage);

                return false;

            }
        }
        #endregion
    }
}
