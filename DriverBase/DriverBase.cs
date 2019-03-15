using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using CHQ.RD.DataContract;
namespace CHQ.RD.DriverBase
{
    public class DriverBase:IDriverBase
    {
        #region 变量和属性
        protected Thread m_thread;
        //protected List<IAddressSetting> m_items;
        //protected object m_host;
        //protected string m_port;
        int m_transmode;
        int m_readmode;
        int m_readinterval;
        Dictionary<int, int> m_errorCount;
        protected Timer m_datareader;
        protected Timer m_errortransact;
        DriverStatus m_status = DriverStatus.None;
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
        public DriverStatus Status
        {
            get { return m_status; }
            set { SetStatus(value); }
        }
        #endregion
        public DriverBase()
        {
            
        }


        #region Operations
        public virtual int SetStatus()
        {
            int ret = -1;
            return ret;
        }
        public virtual int Init()
        {
            int ret = -1;
            return ret;
        }
        public virtual int Started()
        {
            int ret = -1;
            return ret;
        }
        public virtual int Stop()
        {
            int ret = -1;
            return ret;
        }
        public virtual int Restart()
        {
            int ret = -1;
            return ret;
        }
        #endregion

        public void Dispose()
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
