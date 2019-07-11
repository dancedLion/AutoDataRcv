using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using CHQ.RD.DriverBase;
using CHQ.RD.DataContract;
using GeneralOPs;
namespace SerialDriver
{
    public class SerialDriver:DriverBase
    {

        #region 变量与属性
        SerialPort m_sp=null;    //串口变量

        string errorfile = AppDomain.CurrentDomain.BaseDirectory + "logs\\SerialDriverError.log";
        string logfile = AppDomain.CurrentDomain.BaseDirectory + "logs\\SerivalDriver.log";

        #endregion




        public SerialDriver() : base()
        {
            HostType = typeof(SerialHost);
            AddressType = typeof(SerialAddress);
            
        }
        //串口号-host
        //开始位等设置-数据读取



        public override int AcceptSetting(object host, object list)
        {
            int ret = 0;
            try
            {
                m_host = (SerialHost)ParsingHost(host.ToString());

                ret = Init();
                if (ret != 0)
                {
                    throw new Exception("初始化端口参数失败！");
                }
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, this.GetType().FullName + ".AcceptSetting(" + host.ToString() + ") Error: " + ex.Message);
            }
            return ret;
        }

        public override int Init()
        {
            int ret = 0;
            try
            {
                m_sp = new SerialPort(
                    ((SerialHost)m_host).comPort,
                    ((SerialHost)m_host).baudRate,
                    (Parity)((SerialHost)m_host).parity,
                    ((SerialHost)m_host).dataBits,
                    (StopBits)((SerialHost)m_host).stopBits);

            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, this.GetType().FullName + ".AcceptSetting(" + ((SerialHost)m_host).comPort + ") Error: " + ex.Message);
            }
            return ret;
        }


        public override int Start()
        {
            int ret = 0;
            try
            {
                if (DebugMode == 0) TxtLogWriter.WriteMessage(logfile, this.GetType().FullName + ": Begin Open Port At " + DateTime.Now.ToString("hh:mm:ss fff"));
                m_sp.Open();
                if (DebugMode == 0) TxtLogWriter.WriteMessage(logfile, this.GetType().FullName + ": Port Opened(" + ((SerialHost)m_host).comPort + ") At " + DateTime.Now.ToString("hh:mm:ss fff"));
                m_sp.DataReceived += DataReceivedHandler;
            }
            catch (Exception ex) {
                ret = -1;
                TxtLogWriter.WriteErrorMessage(errorfile, this.GetType().FullName + ": Port Open(" + ((SerialHost)m_host).comPort + ") Error: " +ex.Message);
            }
            return ret;
        }

        public override int Stop()
        {
            m_sp.DataReceived -= DataReceivedHandler;
            m_sp.Close();
            return base.Stop();
        }


        void DataReceivedHandler(object sender,SerialDataReceivedEventArgs e)
        {
            byte[] buff = new byte[m_sp.ReadBufferSize];
            string s = m_sp.ReadExisting();
            parsingValue(s);
        }

        void parsingValue(string s)
        {

        }
        void parsingValue(byte[] buffer)
        {

        }
    }
}
