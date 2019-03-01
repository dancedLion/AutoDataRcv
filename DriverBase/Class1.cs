using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace CHQ.RD.DriverBase
{
    public class DriverBase
    {
        protected Thread m_thread;
        protected List<IAddressSetting> m_items;
        //protected object m_host;
        //protected string m_port;
        public DriverBase()
        {
            
        }

        public virtual int SettingAddress()
        {
            return 1;
        }

        public virtual int TryConnectToDevice()
        {
            return 1;
        }
        public virtual object ReadData(int ItemId)
        {
            return 1;
        }

        public virtual object ReadDeviceData(object Item)
        {
            return 1;
        }
        public virtual int SetStatus(object status)
        {
            return 1;
        }
    }
}
