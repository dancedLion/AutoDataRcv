using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using GeneralOPs;
using CHQ.RD.DataContract;
using CHQ.RD.DriverBase;
namespace CHQ.RD.NetworkListener
{
   public class SocketListener:DriverBase.DriverBase
    {
        #region 变量和属性
        Socket sckLinstener = null;
        bool isListening = false;
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
                foreach (ConnDriverDataItem item in (List<ConnDriverDataItem>)list)
                {
                    m_datalist.Add(item);
                }
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
            sckLinstener.Listen(10);
            isListening = true;
            while (isListening)
            {
                byte[] buff = new byte[1024];
                Socket client = sckLinstener.Accept();
                int datalen = client.Receive(buff);
                string result = System.Text.Encoding.Default.GetString(buff, 0, datalen);
                //ParsingValues(result);
                client.Close();
            }
        }
        public override int Start()
        {
            isListening = true;
            StartListener();
            return 0;
        }
        public override int Stop()
        {
            isListening = false;
            sckLinstener.Shutdown(SocketShutdown.Both);
            //Call the Close method to free all managed and unmanaged resources associated with the Socket. Do not attempt to reuse the Socket after closing.
            //sckLinstener.Close();
            return 0;
        }
        public override void Dispose()
        {
            sckLinstener.Shutdown(SocketShutdown.Both);
            sckLinstener.Close();
            base.Dispose();
        }
    }
}
