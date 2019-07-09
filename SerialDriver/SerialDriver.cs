using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using CHQ.RD.DriverBase;
using CHQ.RD.DataContract;
namespace SerialDriver
{
    public class SerialDriver:DriverBase
    {
        public SerialDriver() : base()
        {
            HostType = typeof(SerialHost);
            AddressType = typeof(SerialAddress);
            
        }
        //串口号-host
        //开始位等设置-数据读取

        //起始之间的数据解析
        public override object ParsingHost(string host)
        {
            return base.ParsingHost(host);
        }

        public override int AcceptSetting(object host, object list)
        {
            int ret = 0;
            try
            {
                m_host = (SerialHost)ParsingHost(host.ToString());
            }
            catch(Exception ex)
            {

            }
            return ret;
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


    }
}
