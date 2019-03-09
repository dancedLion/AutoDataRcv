using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace CHQ.RD.DriverBase
{
    public class DriverBase:IDriverBase
    {
        protected Thread m_thread;
        //protected List<IAddressSetting> m_items;
        //protected object m_host;
        //protected string m_port;
        int m_transmode;
        int m_readmode;
        int m_readinterval;
        public int TransMode
        {
            get { return m_transmode; }
            set { m_transmode = value; }
        }
        public int ReadMode
        {
            get { return m_readmode; }
            set { m_readmode = value; }
        }
        public int ReadInterval
        {
            get { return m_readinterval; }
            set { m_readinterval = value; }
        }
        public DriverBase()
        {
            
        }

        public virtual int AcceptSetting(object host,object list)
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
