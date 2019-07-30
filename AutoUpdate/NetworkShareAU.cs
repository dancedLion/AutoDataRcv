using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
namespace AutoUpdate
{
    public class NetworkShareAU:IAutoUpdater
    {
        
    }

    public class NetworkShareAUC : IAutoUpdateClient
    {
        #region 变量和属性
        List<AUFileInfo> m_fileinfos;
        string m_serverPath;
        #endregion

        public NetworkShareAUC()
        {
            m_fileinfos = new List<AUFileInfo>();
        }
        #region Interface Methods
        #endregion

        #region others
        #endregion
    }

    public class NetworkShareAUS : IAutoUpdateServer
    {
        string m_remotelocation = "";
        string m_fileinfofile = "";
        List<AUFileInfo> m_fileinfos;
        public List<AUFileInfo> FileInfos
        {
            get { return m_fileinfos; }
            set { m_fileinfos = value; }
        }

        public NetworkShareAUS()
        {
            m_fileinfos = new List<AUFileInfo>();
            m_fileinfos = GetAllFileInfos();
        }
        public string getServerFileName(int fileId)
        {
            string ret = "";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(m_remotelocation + m_fileinfofile);
                XmlNode node = doc.DocumentElement.SelectSingleNode("Files/File[@FileId=" + fileId + "]");
                if (node != null)
                {
                    ret = node.Attributes["FilePath"].Value + node.Attributes["FileName"].Value;
                }
            }
            catch(Exception ex)
            {
                xmloper.WriteErrorMessage(this.GetType().FullName + ".getServerFileName(FileId=" + fileId + ") Error:" + ex.Message);
            }
            return ret;
        }
        public byte[] PrepareFile(int fileId)
        {
            return PrepareFile(GetFileInfo(fileId).FileName);
        }
        public byte[] PrepareFile(string fileName)
        {
            byte[] ret = null;
            try
            {
                string filename = m_remotelocation + fileName;
                FileStream fs = new FileStream(filename, FileMode.Open , FileAccess.Read);
                ret = new byte[fs.Length];
                int numReading = 0;
                int numToReading = (int)fs.Length;
                while (numToReading > 0)
                {
                    int n= fs.Read(ret, numReading,numToReading);
                    if (n == 0)
                    {
                        break;
                    }
                    numReading += n;
                    numToReading -= n;
                }
            }
            catch(Exception ex)
            {
                xmloper.WriteErrorMessage(this.GetType().FullName + ".PrepareFile(" + fileName + ") Error:" + ex.Message);
            }
            return ret;
        }

        public List<AUFileInfo> GetAllFileInfos()
        {
            List<AUFileInfo> ret = new List<AUFileInfo>();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(m_remotelocation+m_fileinfofile);
                XmlNodeList nodes = doc.DocumentElement.SelectNodes("Files/File");
                foreach (XmlNode node in nodes)
                {
                    AUFileInfo auii = new AUFileInfo
                    {
                        FileId = int.Parse(node.Attributes["FileId"].Value),
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
                xmloper.WriteErrorMessage(this.GetType().FullName+".getServerFileInfos Error:" + ex.Message);
            }
            return ret;
        }
        public AUFileInfo GetFileInfo(int fileId)
        {
            if (m_fileinfos == null || m_fileinfos.Count == 0)
            {
                m_fileinfos = GetAllFileInfos();
            }
            AUFileInfo info = m_fileinfos.Find(t => t.FileId == fileId);
            return info;
        }
    }
}
