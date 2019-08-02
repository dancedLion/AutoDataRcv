using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;
namespace AutoUpdate
{
    public class FtpAUServer : IAutoUpdateServer
    {
        #region variables and properties
        string m_host = "";
        string m_fileinfofile = "";
        Dictionary<string, string> m_settings;
        int m_status = -1;
        List<AUFileInfo> m_fileinfos;
        public List<AUFileInfo> FileInfos
        {
            get { return m_fileinfos; }
            set { m_fileinfos = value; }
        }
        public int ServerStatus
        {
            get { return m_status; }
            set { m_status = value; }
        }
        #endregion

        #region interface action
        public virtual int LoadingSetting(int serverId)
        {
            int ret = -1;
            try
            {
                m_settings = xmloper.getAUSSetting(serverId);
                m_host = m_settings["FtpServer"];
                m_fileinfofile = m_settings["FileInfoSetting"];
                ret = CheckServer();
                m_status = ret;
            }
            catch(Exception ex)
            {
                xmloper.WriteErrorMessage(this.GetType().FullName + ".LoadingSetting(serverid=" + serverId + ") Error:" + ex.Message);
            }
            return ret;
        }

        public virtual List<AUFileInfo> GetAllFileInfos()
        {
            List<AUFileInfo> ret = new List<AUFileInfo>();
            try
            {
                if (m_status < 0) throw new Exception("Server Status Error!");
                //下载到临时文件
                byte[] buff = DownLoad(m_fileinfofile);
                string fname =AppDomain.CurrentDomain.BaseDirectory+ Guid.NewGuid().ToString();
                FileStream fs = new FileStream(fname, FileMode.Create, FileAccess.Write);
                fs.Write(buff, 0, buff.Length);
                fs.Flush();
                fs.Close();
                //读取设置
                XmlDocument doc = new XmlDocument();
                doc.Load(fname);
                XmlNodeList nodes = doc.DocumentElement.SelectNodes("Files/File");
                foreach(XmlNode t in nodes)
                {
                    ret.Add(new AUFileInfo
                    {
                        FileId=t.Attributes["FileId"].Value,
                        FileName= t.Attributes["FileName"].Value,
                        FileSize=int.Parse( t.Attributes["FileSize"].Value),
                        FileVersion= t.Attributes["FileVersion"].Value,
                        FilePath= t.Attributes["FilePath"].Value
                    });
                }
                //移除临时文件
                File.Delete(fname);
            }
            catch(Exception ex)
            {
                xmloper.WriteErrorMessage(this.GetType().FullName + ".GetAllFileInfos Error:" + ex.Message);
            }
            return ret;
        }
        public virtual AUFileInfo GetFileInfo(string fileId)
        {
            return m_fileinfos.Find(t => t.FileId == fileId);
        }

        public virtual byte[] PrepareFile(string fileId)
        {
            byte[] ret = null;
            try
            {
                AUFileInfo ff = GetFileInfo(fileId);
                ret = DownLoad(ff.FilePath + ff.FileName);
                if (ret == null)
                {
                    throw new Exception("下载文件失败！");
                }
            }
            catch(Exception ex)
            {
                xmloper.WriteErrorMessage(this.GetType().FullName + ".Prepare File(file=" + fileId + ") Error:" + ex.Message);
            }
            return ret;
        }

        public virtual byte[] DownLoad(string filename)
        {
            byte[] ret = null;
            try
            {
                string uri = m_host + filename;
                //string tempFileName = AppDomain.CurrentDomain.BaseDirectory + "TempFile\\" + filename;
                FtpWebRequest req = (FtpWebRequest)WebRequest.Create(uri);
                req.Credentials = new NetworkCredential(m_settings["UserName"], m_settings["Password"]);
                req.Method = WebRequestMethods.Ftp.DownloadFile;
                FtpWebResponse resp = (FtpWebResponse)req.GetResponse();
                using(Stream respStream = resp.GetResponseStream())
                {
                    ret = new byte[respStream.Length];
                    respStream.Read(ret, 0, ret.Length);
                }
            }
            catch(Exception ex)
            {
                xmloper.WriteErrorMessage(this.GetType().FullName + ".Download(File=" + filename + ") Error:" + ex.Message);
            }
            return ret;
        }
        public virtual int Upload(string filename,byte[] buff)
        {
            int ret = -1;
            try
            {
                string uri = m_host + filename;
                FtpWebRequest req = (FtpWebRequest)WebRequest.Create(uri);
                req.Credentials = new NetworkCredential(m_settings["UserName"], m_settings["Password"]);
                req.Method = WebRequestMethods.Ftp.UploadFile;

                req.ContentLength = buff.Length;
                Stream st = req.GetRequestStream();
                st.Write(buff, 0, buff.Length);
                st.Flush();
                st.Close();
                ret = 0;
            }
            catch(Exception ex)
            {
                xmloper.WriteErrorMessage(this.GetType().FullName + ".Upload(File=" + filename + ") Error:" + ex.Message);
            }
            return ret;
        }

        public virtual int SetServerFile(AUFileInfo info,byte[] buff)
        {
            int ret = -1;
            try
            {
                if (m_status == -1) throw new Exception("Server Status Error!");
                //上传
                int i = Upload(info.FilePath + info.FileName, buff);
                if (i < 0) throw new Exception("Upload File Failed!");
                //修改信息
                AUFileInfo tinfo = m_fileinfos.Find(t => t.FileId == info.FileId);
                if (tinfo == null)
                {
                    m_fileinfos.Add(info);
                }
                else
                {
                    tinfo = info;
                }
                //修改设置文件
                //下载并保存为临时文件
                byte[] tffs = DownLoad(m_fileinfofile);
                string fname =AppDomain.CurrentDomain.BaseDirectory+ Guid.NewGuid().ToString();
                FileStream fs = new FileStream(fname, FileMode.Create, FileAccess.Write);
                fs.Write(tffs, 0, tffs.Length);
                fs.Flush();
                fs.Close();
                //读取并写入设置
                XmlDocument doc = new XmlDocument();
                doc.Load(fname);
                XmlNode node = doc.DocumentElement.SelectSingleNode("Files/File[@FileId=" + info.FileId + "]");
                XmlElement elem = null;
                if (node == null)
                {
                    elem = doc.CreateElement("File");
                    elem.SetAttribute("FileId", info.FileId);
                    doc.DocumentElement.SelectSingleNode("Files").AppendChild(elem);
                }
                else
                {
                    elem = (XmlElement)node;
                }
                //设置属性
                elem.SetAttribute("FileName", info.FileName);
                elem.SetAttribute("FileSize", info.FileSize.ToString());
                elem.SetAttribute("FilePath", info.FilePath);
                elem.SetAttribute("FileVersion", info.FileVersion);
                doc.Save(fname);
                //上传到服务器
                fs = new FileStream(fname, FileMode.Open, FileAccess.Read);
                buff = new byte[fs.Length];
                int numsToRead = 0;
                int numsReading = buff.Length;
                while (true)
                {
                    i = fs.Read(buff, numsToRead, numsReading);
                    if (i == 0) break;
                    numsToRead += i;
                    numsReading -= i;
                }
                fs.Close();
                //上传并删除文件
                i=Upload(m_fileinfofile, buff);
                if (i < 0) throw new Exception("上传文件信息设置文件发生错误！");
                File.Delete(fname);
            }
            catch(Exception ex)
            {
                xmloper.WriteErrorMessage(this.GetType().FullName + ".SetServerFile(file=" + info.FileId + ") Error:" + ex.Message);
            }
            return ret;
        }

        public virtual int CheckServer()
        {
            int ret = -1;
            try
            {
                //连接FTP
                FtpWebRequest req = (FtpWebRequest)FtpWebRequest.Create(m_host);
                req.Credentials = new NetworkCredential(m_settings["UserName"], m_settings["Password"]);
                req.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse resp = (FtpWebResponse)req.GetResponse();
                //读取文件
                Stream st = resp.GetResponseStream();
                StreamReader sr = new StreamReader(st, Encoding.Default);
                string line = sr.ReadLine();
                bool isfind = false;
                while (line!=null)
                {
                    if (line == m_fileinfofile)
                    {
                        isfind = true;
                        break;
                    }
                    line = sr.ReadLine();
                }
                //如果未有信息文件，则上传一个文件
                if (!isfind)
                {
                    string filetext = @"<?xml version=""1.0"" encoding=""utf-8"" ?>" +
                        @"<FileInfos>" +
                        @"<Files>" +
                        @"</Files>" +
                        @"</FileInfos>";
                    byte[] buff = Encoding.Default.GetBytes(filetext);
                    string filename = m_host + m_fileinfofile;
                    req = (FtpWebRequest)WebRequest.Create(filename);
                    req.Credentials=new NetworkCredential(m_settings["UserName"], m_settings["Password"]);
                    req.Method = WebRequestMethods.Ftp.UploadFile;
                    req.ContentLength = buff.Length;
                    st = req.GetRequestStream();
                    st.Write(buff, 0, buff.Length);
                    st.Flush();
                    st.Close();
                }
                ret = 0;
            }
            catch(Exception ex)
            {
                xmloper.WriteErrorMessage(this.GetType().FullName + ".CheckServer Error:" + ex.Message);
            }
            return ret;
        }
        #endregion

        #region others
        #endregion
    }
}
