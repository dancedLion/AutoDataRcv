using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ConnectionBase
{
    public class DriverBase:IConnectionDriver
    {
        public DriverBase()
        {
        }
        string m_connectionstring = "";
        List<string> m_loglist;
        Dictionary<int, object> m_valuelist;
        List<object> m_itemlist;
        List<Thread> m_processlist;

        protected Thread m_listening;
        protected Thread m_transactingrequest;
        protected Thread m_transactingdevicerequest;
        protected Thread m_transactinglocaldata;



        public string ConnectionString
        {
            get { return m_connectionstring; }
            set { m_connectionstring = value; }
        }
        public List<string> LogList
        {
            get { return m_loglist; }
        }
        public Dictionary<int,object> ValueList
        {
            get { return m_valuelist; }
        }
        public List<object> ItemList
        {
            get { return m_itemlist; }
            set { m_itemlist = value; }
        }

        public virtual int TryConnection()
        {
            return 1;
        }
        public virtual int EstablishDataList()
        {
            return 1;
        }
        public virtual int ReadData(int ItemId) {
            return 1;
        }
        public virtual int ReadData()
        {
            return 1;
        }
        public virtual int AcceptRequest(RequestType requesttype, object requestdata)
        {
            return 1;
        }
        public virtual int SendData(int ItemId)
        {
            return 1;
        }
        public virtual int SendData()
        {
            int i = 0;
            foreach(KeyValuePair<int,object> item in ValueList)
            {
                i=SendData(item.Key);
                if(i < 0)
                {
                    m_loglist.Add("DriverBase.sendData:发送数据时发生错误,ItemID=" + item.Key.ToString());
                    return i;
                }
            }
            return 1;
        }
        public virtual int TransactRequest()
        {
            return 1;
        }   
        public virtual int ConnectToDevice()
        {
            return 1;
        }
        public virtual int ParsingConnectionString()
        {
            return 1;
        }

        protected virtual int Initialize()
        {
            try
            {
                if (string.IsNullOrEmpty(m_connectionstring))
                {
                    throw new Exception(DriverErrors.ParsingConnectionStringError.ToString() + "未设置驱动字符串!");
                }
                int i = ParsingConnectionString();
                if (i > 0)
                {
                    if (TryConnection() > 0)
                    {
                        //初始化连接成功
                        m_listening = new Thread(new ThreadStart(AcceptRequest),);
                        m_processlist.Add(m_listening);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return 1;
        }

    }
}
