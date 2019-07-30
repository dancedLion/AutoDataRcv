using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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

    }
}
