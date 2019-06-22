using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CHQ.RD.DataContract;
using CHQ.RD.DriverBase;
using System.Net;
using System.Net.Sockets;
using GeneralOPs;
namespace CHQ.RD.NetworkListener
{
    public class NetworkListener:DriverBase.DriverBase
    {

        int m_listenerType = 0;
        int m_port = 0;
        int m_via = 0;



        public NetworkListener() : base()
        {
            HostType = typeof(NetworkListenerHostType);
            AddressType = typeof(NetworkListenerAddressType);
            m_valuelist = new Dictionary<int, object>();
        }

        public override int Init()
        {
            return base.Init();
        }
        public override int Start()
        {
            return base.Start();
        }

        public override int Stop()
        {
            return base.Stop();
        }
        public override void StartListener()
        {
            base.StartListener();
        }

        public override int SendData(object value)
        {
            return base.SendData(value);
        }

        public virtual void startSocketListener()
        {

        }
        public override object ParsingHost(string host)
        {
            object ret = null;

            return ret;
        }
        public override int AcceptSetting(object host, object list)
        {
            int ret = -1;
            try
            {
                foreach (ConnDriverDataItem item in (List<ConnDriverDataItem>)list)
                {
                    m_valuelist.Add(item.Id, null);
                }
                ParsingHost(host.ToString());
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
