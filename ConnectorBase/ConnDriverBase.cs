using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CHQ.RD.DriverBase;
using CHQ.RD.DataContract;
using System.Reflection;    //创建驱动实例用
namespace CHQ.RD.ConnectorBase
{
    public class ConnDriverBase
    {
        public ConnDriverBase(int id)
        {

        }
        int m_id = -1;
        public int ID
        {
            get { return m_id; }
        }
        IDriverBase m_driver;
        public IDriverBase Driver
        {
            get { return m_driver; }
        }
        Type m_driverclass;
        public Type DriverClass
        {
            get { return m_driverclass; }
            set { m_driverclass = value; }
        }
        ConnDriverStatus m_status = ConnDriverStatus.None;
        public ConnDriverStatus Status
        {
            get { return m_status; }
            set { SetStatus(value); }
        }
        Dictionary<int, int> m_errorCount;
        string errorfile = AppDomain.CurrentDomain.BaseDirectory + "\\logs\\ConnDriverError.log";
        string logfile = AppDomain.CurrentDomain.BaseDirectory + "\\logs\\ConnDriver.log";

        public virtual int SetStatus(ConnDriverStatus status)
        {

            return -1;
        }
        public virtual int EstableItemList()
        {
            return -1;
        }
        public virtual int ConnectToDriver()
        {
            return -1;
        }

        public virtual object ReadData()
        {
            return null;
        }
        public virtual object ReadData(int itemid)
        {
            return null;
        }

        public virtual int Init()
        {
            //加载驱动
            
            return -1;
        }
        public virtual int Start()
        {
            return -1;
        }
        public virtual int Pause()
        {
            return -1;
        }
        public virtual int Stop()
        {
            return -1;
        }
        public virtual int Close()
        {
            return -1;
        }
        public virtual int Run()
        {
            return -1;
        }
    }
}
