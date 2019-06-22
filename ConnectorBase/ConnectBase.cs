using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CHQ.RD.DataContract;
using GeneralOPs;
using System.Net;
using System.Net.Sockets;
namespace CHQ.RD.ConnectorBase
{
    /// <summary>
    /// 连接管理器基础
    /// 用来管理ConnDriver动用
    /// ConnDriver用来管理Driver动作
    /// </summary>
    public class ConnectorBase:IConnectorBase
    {

        #region variables and properties
        protected string XmlSettingFile =AppDomain.CurrentDomain.BaseDirectory+ "\\ConnectorSetting.xml";
        string errorlogfile = AppDomain.CurrentDomain.BaseDirectory + "\\log\\connectorError.log";
        protected Thread readingThread;
        protected Thread manageThread;
        protected Thread sendingThread;
        protected Thread listenThread;
        public Dictionary<int, object> ValueList;
        protected List<ConnDriverBase> connDriverList;

        public List<ConnDriverBase> ConnDrivers
        {
            get { return connDriverList; }
        }


        int m_id= -1;
        public int ConnectorId
        {
            get { return m_id; }
        }
        List<DataSendingSet> m_sendins;
        public List<DataSendingSet> DataSendingHosts
        {
            get { return m_sendins; }
            set { m_sendins = value; }
        }
        #endregion
        public ConnectorBase(int id)
        {
            m_id = id;
            ValueList = new Dictionary<int, object>();
            connDriverList = new List<ConnDriverBase>();
        }
        /// <summary>
        /// 初始化
        /// 1、自身建立host
        /// 2、加载驱动连接器
        /// 3、加载接收数据主机
        /// </summary>
        /// <returns></returns>
        public virtual int Init()
        {
            int ret = 0;
            try
            {
                //初始化各驱动连接器
                List<ConnDriverSetting> allcds = Ops.getConnDriverSettingList();
                foreach (ConnDriverSetting cds in allcds)
                {
                    try
                    {
                        ConnDriverBase cdb = new ConnDriverBase(cds.Id, this);
                        InitConnDriver(cdb);
                    }
                    catch(Exception ex)
                    {
                        TxtLogWriter.WriteErrorMessage(this.GetType().ToString() + ":Init(ConnDriverId=" + cds.Id.ToString() + ":" + cds.Name + ") Error:" + ex.Message);
                    }
                }
                ret = 1;
                //TODO:初始化自己，以供外部程序取数

                //TODO:初始化数据读取
                m_sendins = Ops.getDataSendingList(m_id);

                //TODO:初始化数据发送
            }
            catch(Exception ex)
            {
                ret = -1;
            }
            return ret;
        }


        public virtual int ReadValue(int itemid)
        {
            return 1;
        }

        public virtual int InitConnDriver(ConnDriverBase conn)
        {
            int ret = -1;
            try
            {
                conn.Init();
                foreach(ConnDriverDataItem item in conn.DataItems)
                {
                    ValueList.Add(item.Id, null);
                }
                connDriverList.Add(conn);
                ret = 0;
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorlogfile, this.GetType().ToString() + ".InitConnDriverError:" + ex.Message);
            }
            return ret;
        }
        public virtual int RunConnDriver(ConnDriverBase conn)
        {
            int ret = -1;
            try
            {
                ret=conn.Start();
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorlogfile, this.GetType().ToString() + ".RunConnDrvier(" + conn.ID.ToString() + ") Error:" + ex.Message);
            }
            return ret;
        }
        public virtual int TestConnDriver(ConnDriverBase conn)
        {
            int ret = -1;
            //ConnDriverBase cdb = new ConnDriverBase(conn.ID, this);
            conn.Init();
            foreach(ConnDriverDataItem item in conn.DataItems)
            {
                ValueList.Add(item.Id, null);
            }
            conn.Start();
            connDriverList.Add(conn);
            ret = 0;
            return ret;
        }
        public virtual int StopConnDriver(ConnDriverBase conn)
        {
            int ret = -1;
            return ret;
        }
        public virtual int CloseConnDriver(ConnDriverBase conn)
        {
            int ret = -1;
            return ret;
        }

        public virtual int SendData(ConnectorDataItem item,object value)
        {
            int ret = 0;
            foreach(DataSendingSet dss in m_sendins)
            {
                //本发送只发送有变化的数据
                if (dss.SendInterval == 0)
                {
                    switch (dss.Via)
                    {
                        case 0: //socket
                            sendViaSocket(dss, item, value);
                            break;
                        case 1: //UDP
                            sendViaUDP(dss, item, value);
                            break;
                        case 2: //TCP
                            sendViaTCP(dss, item, value);
                            break;
                    }
                }
            }
            return ret;
        }

        #region datasending
        void sendViaUDP(DataSendingSet dss,ConnectorDataItem item,object value)
        {

        }
        void sendViaSocket(DataSendingSet dss, ConnectorDataItem item, object value)
        {
            try
            {
                IPEndPoint iep = new IPEndPoint(IPAddress.Parse(dss.Host), dss.HostPort);
                Socket sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                sck.Connect(iep);
                byte[] buffer = Encoding.UTF8.GetBytes(item.ConnId+";"+item.Id.ToString() + ";" + value.ToString()+";"+DateTime.Now.ToString());
                sck.Send(buffer);
                sck.Close();
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorlogfile, this.GetType().ToString() + ".SendViaSocket(" + dss.Name + ") Error:" + ex.Message);
            }
        }
        void sendViaTCP(DataSendingSet dss, ConnectorDataItem item, object value)
        {

        }
        #endregion



        //TODO:数据变化时的事件
        //附加的事件
    }
}
