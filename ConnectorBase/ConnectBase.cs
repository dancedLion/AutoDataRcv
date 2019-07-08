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
        protected string XmlSettingFile =AppDomain.CurrentDomain.BaseDirectory+ "ConnectorSetting.xml";
        string errorlogfile = AppDomain.CurrentDomain.BaseDirectory + "logs\\connectorError.log";
        protected Thread readingThread;
        protected Thread manageThread;
        protected Thread sendingThread;
        protected Thread listenThread;
        DataChangeEventHandler m_dchandler;
        public Dictionary<int, object> ValueList;
        protected List<IConnDriverBase> connDriverList;
        /// <summary>
        /// 本地数据存储设置
        /// </summary>
        List<ConnectorLocalData> m_localdata;

        public List<IConnDriverBase> ConnDrivers
        {
            get { return connDriverList; }
        }

        public event DataChangeEventHandler DataChange
        {
            add { m_dchandler += value; }
            remove { m_dchandler -= value; }
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
            connDriverList = new List<IConnDriverBase>();
            startDataTransact();
        }



        #region Connector Actions
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
                        IConnDriverBase cdb = 
                            (IConnDriverBase)cds.ConnDriverClass.Assembly.
                            CreateInstance(cds.ConnDriverClass.FullName,true,
                            System.Reflection.BindingFlags.Default,null,
                            new object[] {cds.Id,this},null,null
                            );
                            //new ConnDriverBase(cds.Id, this);
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
                TxtLogWriter.WriteErrorMessage(errorlogfile, this.GetType().ToString() + ".Init Error:" + ex.Message);
                ret = -1;
            }
            return ret;
        }


        public virtual int ReadValue(int itemid)
        {
            return 1;
        }

        public virtual int InitConnDriver(IConnDriverBase conn)
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
        public virtual int RunConnDriver(IConnDriverBase conn)
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
        public virtual int TestConnDriver(IConnDriverBase conn)
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
        public virtual int StopConnDriver(IConnDriverBase conn)
        {
            int ret = -1;
            return ret;
        }
        public virtual int CloseConnDriver(IConnDriverBase conn)
        {
            int ret = -1;
            try {
                conn.Close();
                ret = 0;
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorlogfile, this.GetType().FullName + ".CloseConnDriver Error(id=" + conn.ID + "):" + ex.Message);
                
            }
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
            //20190626添加，驱动连接器触发的实时数据变更产生的发送数据时，触发数据变更事件
            onDataChanged(this, new DataChangeEventArgs(item.Id, value));
            return ret;
        }
        #endregion
        #region datasending
        void sendViaUDP(DataSendingSet dss,ConnectorDataItem item,object value)
        {
            try
            {
                //m_connection.Id + ";" +
                //e.ItemId + ";" +
                //Convert.ToString(e.Value) + ";" +
                //DateTime.Now.ToString()
                string msg = m_id + ";" +
                    item.Id.ToString() + ";" +
                    Convert.ToString(value) + ";" +
                    DateTime.Now.ToString();
                UdpClient client = new UdpClient();
                client.Connect(dss.Host, dss.HostPort);
                byte[] buff = System.Text.Encoding.Default.GetBytes(msg);
                client.Send(buff, buff.Length);
                client.Close();
            }
            catch (Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorlogfile, this.GetType().ToString() + ".sendViaUDP Error:" + ex.Message);
            }
        }
        void sendViaSocket(DataSendingSet dss, ConnectorDataItem item, object value)
        {
            try
            {
                IPEndPoint iep = new IPEndPoint(IPAddress.Parse(dss.Host), dss.HostPort);
                Socket sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                sck.Connect(iep);
                byte[] buffer = Encoding.UTF8.GetBytes(item.TransSig + ";" + value.ToString()+";"+DateTime.Now.ToString());
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
            try
            {

            }
            catch (Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorlogfile, this.GetType().ToString() + ".sendViaUDP Error:" + ex.Message);
            }
        }
        #endregion

        public void Dispose()
        {
            foreach(IConnDriverBase icdb in connDriverList)
            {
                icdb.Dispose();
            }
        }
        #region 本地数据存储处理
        SqlRealTimeDataTransact m_tran = null;
        int startDataTransact()
        {
            int ret = 0;
            m_localdata = Ops.getConnectorLocalDataList();
            foreach (ConnectorLocalData cld in m_localdata)
            {
                switch (cld.RDType)
                {
                    case 0:   //实时数据
                              //TODO: 暂时只处理SQLSERVER
                        m_tran = new SqlRealTimeDataTransact(cld.ConnectString);
                        if (m_tran.startDataTransact() < 0)
                        {
                            m_tran = null;
                        }
                        break;

                }
            }
            return ret;
        }
        #endregion
        //TODO:数据变化时的事件
        //附加的事件
        #region 内部事件和方法
        void onDataChanged(object sender, DataChangeEventArgs e)
        {
            //写值   
            //触发handler
            if (m_dchandler != null)
            {
                m_dchandler(sender, e);
            }

            //如果m_tran不为null,则处理数据
            if (m_tran != null)
            {
                m_tran.TransactData(e.ItemId, e.Value);
            }
        }
        #endregion
    }
}
