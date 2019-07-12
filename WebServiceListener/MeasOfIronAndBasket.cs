/*此驱动用于从计量数据库中读取以下数据：
 * 废钢车间料蓝称重
 * 轨道衡铁水数据
 * 数据传送给驱动连接器后，由驱动连接器负责写入到炼钢计量系统的指定数据库中
 * 作者：徐震
 * 开始编码日期：2019年7月11日
 * 为了减少客户端上的配置，从webservice取数,每次取数成功将更新上次取数日期
 * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CHQ.RD.DriverBase;
using CHQ.RD.DataContract;
using System.Threading;
using GeneralOPs;
namespace CHQ.RD.WebServiceListener
{
    public class MeasOfIronAndBasket:DriverBase.DriverBase
    {
        #region 局部变量与属性
        private string settingsfile = AppDomain.CurrentDomain.BaseDirectory + "IronAndBasket.xml";
        string errorfile = AppDomain.CurrentDomain.BaseDirectory + "WebServiceListenerError.xml";
        DateTime currenttime = DateTime.Now;
        ManualResetEvent mre = new ManualResetEvent(false);
        Thread m_readthread = null;
        List<MeasRow> m_items = new List<MeasRow>();
        Queue<ListKeyValue> m_values = new Queue<ListKeyValue>();
        #endregion

        public MeasOfIronAndBasket() : base()
        {
            m_items.Add(new MeasRow
            {
                MaterialType = "Iron",
                LastQueryTime = getLastDate("Iron")
            });
            m_items.Add(new MeasRow
            {
                MaterialType = "Basket",
                LastQueryTime = getLastDate("Basket")
            });
            m_readthread = new Thread(ReadingValue);
        }

        public override int AcceptSetting(object host, object list)
        {
            //return base.AcceptSetting(host, list);
            return 0;
        }
        public override int Start()
        {
            int ret = 0;
            m_readthread.Start();
            mre.Set();
            return ret;
        }
        public override int Stop()
        {
            mre.Reset();
            return 0;
            //return base.Stop();
        }
        /// <summary>
        /// 根据取回的计量数据转换为字符并传给驱动连接器
        /// </summary>
        public void ReadingValue()
        {
            try
            {
                mre.WaitOne();
                
                UASV.ChqUASettingsClient client = new UASV.ChqUASettingsClient();
                //读取数据
                //如果有行则写最新的日期
                //最新的日期是指计量行中的最大的净重日期
                //currenttime=
                m_values.Enqueue(new ListKeyValue
                {

                })
            }
            catch(Exception ex)
            {

            }
        }

        public DateTime getLastDate(string materialtype)
        {
            DateTime dt = DateTime.Now;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(settingsfile);
                dt = DateTime.Parse(doc.DocumentElement.SelectSingleNode("LastTime").Attributes[materialtype].Value);
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, this.GetType().FullName + ".getLastDate Error:" + ex.Message);
            }
            return dt;
        }
        public int saveLastDate(DateTime dt,string materialtype)
        {
            int ret = -1;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(settingsfile);
                XmlElement node =(XmlElement)doc.DocumentElement.SelectSingleNode("LastTime");
                if (node != null)
                {
                    node.SetAttribute(materialtype, dt.ToString("yyyy-MM-dd hh:mm:ss fff"));
                }
                doc.Save(settingsfile);
                ret = 0;
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, this.GetType().FullName + ".saveLastDate Error:" + ex.Message);
            }
            return ret;
        }
    }
    public class MeasRow
    {
        public string MaterialType;
        public string MeasureResult;
        public DateTime LastQueryTime;
    }
}
