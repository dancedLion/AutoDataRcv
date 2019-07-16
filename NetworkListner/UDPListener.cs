using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CHQ.RD.DataContract;
using CHQ.RD.DriverBase;
using GeneralOPs;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace CHQ.RD.NetworkListener
{
    public class UDPListener:DriverBase.DriverBase
    {
        #region 变量与属性
        static ManualResetEvent m_mre = new ManualResetEvent(false);
        UdpClient m_server = null;
        #endregion

        public UDPListener() : base()
        {
            HostType = typeof(NetworkListenerHostType);
            AddressType = typeof(NetworkListenerAddressType);
            m_host = new NetworkListenerHostType();
            m_datalist = new List<ConnDriverDataItem>();
            ValueList = new Queue<ListKeyValue>();
        }

        #region 驱动操作
        public override int Start()
        {
            m_mre.Set();
            Status = DriverStatus.Running;
            return 0;
        }
        public override int Stop()
        {
            m_mre.Reset();
            Status = DriverStatus.Stoped;
            return 0;
        }
        public override void Dispose()
        {
            if (m_thread != null)
            {
                m_thread.Abort();
                m_thread = null;
            }
            m_datalist.Clear();
            ValueList = null;
        }
        #endregion


        public override int AcceptSetting(object host, object list)
        {
            int ret = 0;
            try
            {
                m_host = ParsingHost(host.ToString());
                foreach(ConnDriverDataItem item in (List<ConnDriverDataItem>)list)
                {
                    m_datalist.Add(item);
                }
                m_server = new UdpClient(((NetworkListenerHostType)m_host).HostPort);
                m_thread = new Thread(StartLinstener);
                Status = DriverStatus.Inited;
            }
            catch(Exception ex)
            {
                WriteErrorMessage("AcceptSetting Error:" + ex.Message);
            }
            return ret;
        }

        public virtual void StartLinstener()
        {
            try
            {
                IPEndPoint ipe = new IPEndPoint(IPAddress.Any, 0);
                while (true)
                {
                    m_mre.WaitOne();
                    byte[] buffer = m_server.Receive(ref ipe);
                    string value = Encoding.Default.GetString(buffer);
                    ParsingValue(value);        
                }
            }
            catch(Exception ex)
            {
                WriteErrorMessage("StartListener Error:" + ex.Message);
                m_thread = new Thread(StartLinstener);
            }
        }

        public virtual void ParsingValue(string value)
        {
            try
            {
                int id = 1;
                foreach(ConnDriverDataItem item in m_datalist)
                {
                    id = item.Id;
                }
                lock (ValueList)
                {
                    ((Queue<ListKeyValue>)ValueList).Enqueue(
                        new ListKeyValue
                        {
                            Id=id,
                            Value=value
                        }
                        );
                }
            }
            catch(Exception ex)
            {
                WriteErrorMessage("ParsingValue Error:" + ex.Message + "\r\n" + value);
            }
        }
    }
}
