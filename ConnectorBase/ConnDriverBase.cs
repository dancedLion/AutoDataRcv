using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CHQ.RD.DriverBase;
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
        public virtual int EstableItemList()
        {
            return -1;
        }
        public virtual int ConnectToDriver()
        {
            return -1;
        }

        
    }
}
