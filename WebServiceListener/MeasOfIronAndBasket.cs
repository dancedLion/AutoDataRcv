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
using System.Data;
namespace CHQ.RD.WebServiceListener
{
    public class MeasOfIronAndBasket:DriverBase.DriverBase
    {
        #region 局部变量与属性
        private string settingsfile = AppDomain.CurrentDomain.BaseDirectory + "IronAndBasket.xml";
        string errorfile = AppDomain.CurrentDomain.BaseDirectory + "logs\\WebServiceListenerError.xml";
        DateTime currenttime = DateTime.Now;
        ManualResetEvent mre = new ManualResetEvent(false);
        Thread m_readthread = null;
        List<MeasRow> m_items = new List<MeasRow>();
        Queue<ListKeyValue> m_values = new Queue<ListKeyValue>();

        public override object ValueList
        {
            get { return (object)m_values; }
            set { m_values = (Queue<ListKeyValue>)value; }
        }
        #endregion



        public MeasOfIronAndBasket() : base()
        {
            HostType = null;
            AddressType = typeof(IronAndBasketAddress);
            m_datalist = new List<ConnDriverDataItem>();
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
            m_readthread.Start();
        }

        public override int AcceptSetting(object host, object list)
        {
            int ret = 0;
            try
            {
                foreach(ConnDriverDataItem item in (List<ConnDriverDataItem>)list)
                {
                    m_datalist.Add(item);
                }
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, this.GetType().FullName + ".AcceptSetting Error:" + ex.Message);
            }
            //return base.AcceptSetting(host, list);
            return ret;
        }
        public override int Start()
        {
            int ret = 0;
            mre.Set();
            if (DebugMode == 0) TxtLogWriter.WriteMessage(this.GetType().FullName + ".Started at" + DateTime.Now.ToString("hh:mm:ss"));
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
            while (true)
            {
                mre.WaitOne();
                try
                {
                    if (DebugMode == 0) TxtLogWriter.WriteMessage(this.GetType().FullName + ".Start Reading at" + DateTime.Now.ToString("hh:mm:ss"));
                    UASV.ChqUASettingsClient client = new UASV.ChqUASettingsClient();
                    //读取铁水数据
                    MeasRow row = m_items.Find((MeasRow x) => x.MaterialType == "Iron");
                    DataTable dt = client.getLastMeasureDataOfIron(row.LastQueryTime);
                    //如果有行则写最新的日期
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            string s = dr["carno"].ToString()+";"+
                                dr["materialname"].ToString() + ";" +
                                dr["sourceName"].ToString() + ";" +
                                dr["targetName"].ToString() + ";" +
                                dr["gross"].ToString() + ";" +
                                dr["tare"].ToString() + ";" +
                                dr["suttle"].ToString() + ";" +
                                dr["grosstime"].ToString()+";"+
                                dr["suttletime"].ToString();
                            lock (m_values)
                            {
                                m_values.Enqueue(new ListKeyValue
                                {
                                    Id = m_datalist.Find((ConnDriverDataItem x) => x.Address.IndexOf("Iron") >= 0).Id,
                                    Value = s
                                });
                                if (row.LastQueryTime < DateTime.Parse(dr["suttletime"].ToString()))
                                {
                                    row.LastQueryTime = DateTime.Parse(dr["suttletime"].ToString());
                                }
                            }
                        }
                        //最新的日期是指计量行中的最大的净重日期
                        saveLastDate(row.LastQueryTime, row.MaterialType);
                    }
                    //取料篮数据
                    row = m_items.Find((MeasRow x) => x.MaterialType == "Basket");
                    dt = client.getLastMeasureDataOfBasket(row.LastQueryTime);
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            string s = dr["basketno"].ToString()+";"+
                                dr["materialname"].ToString() + ";" +
                                dr["sourceName"].ToString() + ";" +
                                dr["targetName"].ToString() + ";" +
                                dr["gross"].ToString() + ";" +
                                dr["tare"].ToString() + ";" +
                                dr["suttle"].ToString() + ";" +
                                dr["grosstime"].ToString()+";"+
                                dr["suttletime"].ToString();
                            lock (m_values)
                            {
                                m_values.Enqueue(new ListKeyValue
                                {
                                    Id = m_datalist.Find((ConnDriverDataItem x) => x.Address.IndexOf("Basket") >= 0).Id,
                                    Value = s
                                });
                            }
                            if (row.LastQueryTime < DateTime.Parse(dr["suttletime"].ToString()))
                            {
                                row.LastQueryTime = DateTime.Parse(dr["suttletime"].ToString());
                            }
                        }
                        //最新的日期是指计量行中的最大的净重日期
                        saveLastDate(row.LastQueryTime, row.MaterialType);
                    }
                    if (DebugMode == 0) TxtLogWriter.WriteMessage(this.GetType().FullName + ".End Reading at" + DateTime.Now.ToString("hh:mm:ss"));

                    //执行完成后休息一会
                    Thread.Sleep(ReadInterval);
                }
                catch (Exception ex)
                {
                    TxtLogWriter.WriteErrorMessage(errorfile, this.GetType().FullName + ".ReadValue Error:" + ex.Message);
                }
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
                    node.SetAttribute(materialtype, dt.ToString("yyyy-MM-dd HH:mm:ss"));
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

        public override void Dispose()
        {
            m_items.Clear();
            mre.Reset();
            m_readthread.Abort();
            m_readthread = null;
            m_values.Clear();
        }
    }
    public class MeasRow
    {
        public string MaterialType;
        public string MeasureResult;
        public DateTime LastQueryTime;
    }
}
