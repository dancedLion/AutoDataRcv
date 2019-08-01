using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoUpdate
{
    public class LoadAutoUpdate
    {
        //如果角度为admin,那就显示更新，否则直接根据
        //手工更新，则显示手动更新界面
        //自动更新，则运行自动更新
        public LoadAutoUpdate()
        {
            IAutoUpdateClient client = (IAutoUpdateClient)new AUCLient();
            if (client.ClientSetting["UpdateType"] == "Admin")
            {
                (new FrmStarter(client)).ShowDialog();
            }
            else
            {
                if (client.ClientSetting["UpdateType"] == "Auto")
                {
                    client.AutoRunUpdate();
                }
                else
                {
                    //手工下载
                }
            }
        }
    }

    public class AUFileInfo
    {
        public string FileId;
        public string FileName;
        public string FileVersion;
        public string FilePath;
        public int FileSize;
    }
    public class AUSSetting
    {
        public int Id;
        public Dictionary<string, string> Settings;
    }
    public class AutoUpdater
    {
        public AutoUpdater()
        {
            client= (IAutoUpdateClient)new AUCLient();
            
        }
        public IAutoUpdateClient client;
        public void LoadUpdate()
        {
            if (client.ClientSetting["UpdateType"] == "Admin")
            {
                (new FrmStarter(client)).ShowDialog();
            }
            else
            {
                if (client.ClientSetting["UpdateType"] == "Auto")
                {
                    client.AutoRunUpdate();
                }
                else
                {
                    //手工下载
                    (new FrmUpdate(client)).ShowDialog();
                }
            }
        }
    }
}
