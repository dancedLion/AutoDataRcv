using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace CHQ.RD.ConnectorBase
{
    public class ConnectorBase:IConnectorBase
    {
        protected string XmlSettingFile =AppDomain.CurrentDomain.BaseDirectory+ "\\connector.xml";
        protected Thread readingThread;
        protected Thread manageThread;
        protected Thread sendingThread;
        protected Thread listenThread;
        
        protected List<ConnDriverBase> connDriverList;
        int m_id= -1;
        public int ConnectorId
        {
            get { return m_id; }
        }

        public ConnectorBase(int id)
        {
            m_id = id;
        }

        public virtual int ReadValue(int itemid)
        {
            return 1;
        }

        public virtual int InitConnDriver(ConnDriverBase conn)
        {

        }
        public virtual int RunConnDriver(ConnDriverBase conn)
        {

        }
        public virtual int TestConnDriver(ConnDriverBase conn)
        {

        }
        public virtual int StopConnDriver(ConnDriverBase conn)
        {

        }
        public virtual int PauseConnDriver(ConnDriverBase conn)
        {

        }
        public virtual int CloseConnDriver(ConnDriverBase conn)
        {

        }
    }
}
