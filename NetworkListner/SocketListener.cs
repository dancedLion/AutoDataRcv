using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using GeneralOPs;
using CHQ.RD.DataContract;
using CHQ.RD.DriverBase;
using System.Threading;
namespace CHQ.RD.NetworkListener
{
   public class SocketListener:DriverBase.DriverBase
    {
        #region 变量和属性
        Socket sckLinstener = null;
        bool isListening = false;
        int m_itemid = -1;
        
        #endregion

        public SocketListener() : base()
        {
            HostType = typeof(NetworkListenerHostType);
            m_host = new NetworkListenerHostType();
            AddressType = typeof(NetworkListenerAddressType);
            m_datalist = new List<ConnDriverDataItem>();
            ValueList = new Queue<ListKeyValue>();
        }
        public override int AcceptSetting(object host, object list)
        {
            int ret = 0;
            try
            {
                m_host = ParsingHost(host.ToString());
                sckLinstener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                foreach (ConnDriverDataItem item in (List<ConnDriverDataItem>)list)
                {
                    m_datalist.Add(item);
                    m_itemid = item.Id;
                }
                ret = Init();
                if (ret < 0)
                {
                    throw new Exception("初始化时发生错误！");
                }
                Status = DriverStatus.Inited;
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(ErrorFile, this.GetType().FullName + ".AcceptSetting Error:" + ex.Message);
            }
            return ret;
              
        }
        public override int Init()
        {
            int ret = 0;
            try
            {
                IPEndPoint iep = new IPEndPoint(IPAddress.Any, ((NetworkListenerHostType)m_host).HostPort);
                sckLinstener.Bind(iep);
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(ErrorFile, this.GetType().FullName + ".Init Error:" + ex.Message);
                ret = -1;
            }
            return ret;
        }
        public override void StartListener()
        {
            try
            {
                sckLinstener.Listen(10);
                isListening = true;
                while (isListening)
                {
                    Thread.Sleep(1);
                    byte[] buff = new byte[1024];
                    Socket client = sckLinstener.Accept();
                    int datalen = client.Receive(buff);
                    string result = System.Text.Encoding.Default.GetString(buff, 0, datalen);
                    SendData(result);
                    client.Close();
                }
            }
            catch(Exception ex)
            {
                WriteErrorMessage("StartListening Error:" + ex.Message);
                //m_thread = new Thread(StartListener);
                //m_thread.Start();
            }
        }
        public override int SendData(object value)
        {
            int ret = 0;
            try
            {
                lock (ValueList)
                {
                    ((Queue<ListKeyValue>)ValueList).Enqueue(new ListKeyValue
                    {
                        Id = m_itemid,
                        Value = value
                    });
                }
            }
            catch (Exception ex) {
                WriteErrorMessage("SendData Error:" +ex.ToString());
                ret = -1;
            }
            return ret;
        }
        public override int Start()
        {
            int ret = 0;
            try
            {
                isListening = true;
                //m_thread = new Thread(StartListener);
                //m_thread.IsBackground = true;
                //m_thread.Start();
                Thread th = new Thread(StartListener);
                th.IsBackground = true;
                th.Start();
                Status = DriverStatus.Running;
            }
            catch(Exception ex)
            {
                WriteErrorMessage("Start Error:" + ex.Message);
                ret = -1;
            }
            return ret;
        }
        public override int Stop()
        {
            isListening = false;
            //sckLinstener.Shutdown(SocketShutdown.Receive);
            //m_thread.Abort();
            //m_thread = null;
            Status = DriverStatus.Stoped;
            //Call the Close method to free all managed and unmanaged resources associated with the Socket. Do not attempt to reuse the Socket after closing.
            //sckLinstener.Close();
            return 0;
        }
        public override void Dispose()
        {
            //sckLinstener.Shutdown(SocketShutdown.Receive);
            sckLinstener.Close();
            if (m_thread != null)
            {
                m_thread.Abort();
                m_thread = null;
            }
            base.Dispose();
        }
    }
}
