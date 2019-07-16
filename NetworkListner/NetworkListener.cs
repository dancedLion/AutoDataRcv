using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CHQ.RD.DataContract;
using CHQ.RD.DriverBase;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using GeneralOPs;
namespace CHQ.RD.NetworkListener
{
    public class NetworkListener:DriverBase.DriverBase
    {

        int m_listenerType = 0;
        int m_port = 0;
        int m_via = -1;
        List<ConnDriverDataItem> m_datalist = new List<ConnDriverDataItem>();
        Queue<ListKeyValue> m_listvalue;
        static ManualResetEvent mre = new ManualResetEvent(false);
        public NetworkListener() : base()
        {
            HostType = typeof(NetworkListenerHostType);
            AddressType = typeof(NetworkListenerAddressType);
            m_listvalue = new Queue<ListKeyValue>();
        }

        public override object ValueList
        {
            get { return (object)m_listvalue; }
            set { m_listvalue = (Queue < ListKeyValue > )value; }
        }

        public override int Init()
        {
            return base.Init();
           
        }
        public override int Start()
        {
            if (m_via < 0 || m_port < 100)
            {
                return -1;
            }
            else
            {
                m_thread = new System.Threading.Thread(StartListener);
                m_thread.Start();
                mre.Set();
                return 0;
            }
        }

        public override int Stop()
        {
            mre.Reset();
            return 1;
        }
        public override void StartListener()
        {
            switch (m_via)
            {
                case 0:
                    startSocketListener();
                    break;
                case 1:
                    startUDPListener();
                    break;
                case 2:
                    startTCPListener();
                    break;
            }
            base.StartListener();
        }
        public virtual void ListenerContinue()
        {
            mre.Set();
        }
        public virtual void ListenerStop()
        {
            mre.Reset();
        }
        public override int SendData(object value)
        {
            return base.SendData(value);
        }

        public virtual void startSocketListener()
        {
            Socket sck = new Socket(AddressFamily.InterNetwork  , SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, m_port);
            sck.Bind(iep);
            sck.Listen(10);
            while (true)
            {   //启动接收
                mre.WaitOne();
                byte[] buff = new byte[1024];
                Socket client = sck.Accept();
                int datalen=client.Receive(buff);
                string result = System.Text.Encoding.Default.GetString(buff,0,datalen);
                ParsingValues(result);
                client.Close();
            }
        }
        public virtual void startUDPListener()
        {
            UdpClient client = new UdpClient(m_port);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                mre.WaitOne();
                byte[] buffer = client.Receive(ref iep);
                string result = Encoding.Default.GetString(buffer);
                ParsingValues(result);
            }
        }
        public virtual void startTCPListener()
        {
            TcpListener lnsr = new TcpListener(IPAddress.Any, m_port);
            lnsr.Start();
            while (true)
            {
                mre.WaitOne();
                TcpClient client = lnsr.AcceptTcpClient();
                NetworkStream ns = client.GetStream();
                byte[] buffer = new byte[1024];
                int datalen = ns.Read(buffer, 0, buffer.Length);
                string s = Encoding.Default.GetString(buffer, 0, datalen);
                ParsingValues(s);
                client.Close();
            }
        }
        public virtual void ParsingValues(string valuestring)
        {
            try
            {
                string[] rows = valuestring.Split(';');
                for(int i = 0; i < rows.Length; i++)
                {
                    string[] kvp = rows[i].Split('=');
                    ListKeyValue v = new ListKeyValue();
                    v.Id = int.Parse(kvp[0]);
                    if (!string.IsNullOrEmpty(kvp[1]))
                    {
                        switch (m_datalist.Find(x => (x.Id == int.Parse(kvp[0]))).ValueType)
                        {
                            case "TEXT":
                                v.Value = kvp[1];
                                break;
                            case "INT":
                            case "INT32":
                            case "BIT":
                                v.Value = int.Parse(kvp[1]);
                                break;
                            case "REAL":
                                v.Value = float.Parse(kvp[1]);
                                break;

                        }
                    }
                    else
                    {
                        v.Value = null;
                    }
                    m_listvalue.Enqueue(v);
                }
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(ErrorFile, this.GetType().ToString() + ".ParsingValues(" + valuestring + ") Error" + ex.Message);
            }
        }
        public override object ParsingHost(string host)
        {
            object ret = null;
            try
            {
                string[] rows = host.Split(';');
                for(int i = 0; i < rows.Length; i++)
                {
                    string[] kvp = rows[i].Split('=');
                    if (!string.IsNullOrEmpty(kvp[1]))
                    {
                        switch (kvp[0].ToLower())
                        {
                            case "via":
                                m_via = int.Parse(kvp[1]);
                                break;
                            case "hostport":
                                m_port = int.Parse(kvp[1]);
                                break;
                        }
                    }
                }
            }
            catch
            {

            }
            return ret;
        }
        public override int AcceptSetting(object host, object list)
        {
            int ret = -1;
            try
            {
                ParsingHost(host.ToString());
                foreach(ConnDriverDataItem item in (List<ConnDriverDataItem>)list)
                {
                    m_datalist.Add(item);
                }
                ret = 0;
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(ErrorFile, this.GetType().ToString() + ".AcceptSetting Error:" + ex.Message);
            }
            return ret;
        }
    }
}
