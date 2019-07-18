/*作者：徐震 开始代码日期：2019-7-8
 * 此类基于snap 7，主要用于连续块的批量读取，从而实现性能的优化
 * 初始化时使用 readinterval,readmode
 * 需要建立值表，用于连接管理器读取
 * 需要对变量进行分组，以实现片区的管理
 * 需要设定连续读取的参数
 * m_readmode=1;间隔、传输模式由设置决定
 * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CHQ.RD.DataContract;
using CHQ.RD.DriverBase;
using System.Threading;
using GeneralOPs;
using System.Net;
namespace CHQ.RD.S7Sharp7Driver
{

    public class S7SharpDriverWithMultiVar:CHQ.RD.DriverBase.DriverBase,IDisposable
    {
        #region 变量及属性
        Dictionary<S7DataType, int> m_valuetype;    //S7数据类型对应的常数
        Dictionary<S7DataType, int> m_datalen;  //S7数据类型对应的数据长度值，需要经过s7.datatypebyte方法转换
        Dictionary<S7BlockType, int> m_area;    //块类型对应的常数

        List<S7SharpReadItem> m_items;      //需要读取值的变量列表
        Dictionary<int, object> m_values;   //值列表，用于返回值 

        

        S7Client m_client;      //连接对象，连接和处理数据

        Timer m_readtimer;  //读取数据的定时器

        List<S7MultiVarExp> m_blocktypes;   //存储类型及起始；

        string errorfile = AppDomain.CurrentDomain.BaseDirectory + "logs\\S7Sharp7DriverError.log";
        string logfile = AppDomain.CurrentDomain.BaseDirectory + "logs\\S7Sharp7Driver.log";



        #endregion
        public S7SharpDriverWithMultiVar() : base()
        {
            DebugMode = 0;

            HostType = typeof(S7TCPHost);
            AddressType = typeof(S7Address);

            //初始化列表
            m_items = new List<S7SharpReadItem>();
            m_values = new Dictionary<int, object>();
            m_blocktypes = new List<S7MultiVarExp>();
            //m_items = new List<IAddressSetting>();
            m_host = new S7TCPHost();
            m_client = new S7Client();
            m_area = new Dictionary<S7BlockType, int>();
            m_area.Add(S7BlockType.DB, 0x84);
            m_area.Add(S7BlockType.MB, 0x83);
            m_area.Add(S7BlockType.PE, 0x81);   //IB



            m_valuetype = new Dictionary<S7DataType, int>();
            m_valuetype.Add(S7DataType.BIT, 0x02);
            m_valuetype.Add(S7DataType.BYTE, 0x02);
            m_valuetype.Add(S7DataType.REAL, 0x08);
            m_valuetype.Add(S7DataType.TEXT, 0x02);
            m_valuetype.Add(S7DataType.INT16, 0x04);
            m_valuetype.Add(S7DataType.INT, 0x06);
            m_valuetype.Add(S7DataType.UINT16, 0x04);
            m_valuetype.Add(S7DataType.UINT32, 0x06);

            m_datalen = new Dictionary<S7DataType, int>();
            m_datalen.Add(S7DataType.BIT, 1);
            m_datalen.Add(S7DataType.BYTE, 1);
            m_datalen.Add(S7DataType.REAL, 4);
            m_datalen.Add(S7DataType.TEXT, 1);
            m_datalen.Add(S7DataType.INT16, 2);
            m_datalen.Add(S7DataType.UINT16, 2);
            m_datalen.Add(S7DataType.UINT32, 4);
            m_datalen.Add(S7DataType.INT, 4);
        }
        /// <summary>
        /// 获取设置并初始化
        /// readinterval和tranmode来自于驱动连接器设置
        /// </summary>
        /// <param name="host"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public override int AcceptSetting(object host, object list)
        {
            int ret=-1;
            try
            {
                m_host = (S7TCPHost)ParsingHost(host.ToString());
                m_items.Clear();
                m_values.Clear();
                m_blocktypes.Clear();
                //获取readinterval 及sendmode
                //转换列表
                foreach (ConnDriverDataItem item in (List<ConnDriverDataItem>)list)
                {
                    S7Address sad = (S7Address)(new S7Address()).Parsing(item.Address);
                    S7SharpReadItem t = new S7SharpReadItem
                    {
                        Id = item.Id,
                        Address = new S7SharpReadAddress {
                             BlockArea=m_area[sad.BlockType],
                             BlockNo=sad.BlockNo,
                             Start=sad.Start,
                              DataLen=sad.DataLen,
                               WordLen=sad.WordLen
                        },
                        ValueType = (S7DataType)Enum.Parse(typeof(S7DataType), item.ValueType)
                    };
                    //t.Address.BlockArea = m_area[(S7BlockType)t.Address.BlockArea]; //从枚举转换为BLOCK类型
                    m_items.Add(t);
                    m_values.Add(t.Id, null);
                }
                //对数据划块处理
                //对每条数据，比较相同blocktype,相同blockno，根据valuetype+值
                foreach(S7SharpReadItem item in m_items)
                {
                    S7MultiVarExp exp = m_blocktypes.Find((S7MultiVarExp x) => x.BlockType == item.Address.BlockArea&&x.BlockNo==item.Address.BlockNo);
                    if (exp == null)
                    {
                        m_blocktypes.Add(new S7MultiVarExp
                        {
                            BlockType= item.Address.BlockArea,
                            BlockNo= item.Address.BlockNo,
                            Start=item.Address.Start,
                            End=item.Address.Start+item.Address.DataLen
                        });
                    }
                    else
                    {
                        if (item.Address.Start<exp.Start)
                        {
                            exp.Start = item.Address.Start;
                        }
                        else
                        {
                            if (item.Address.Start + item.Address.DataLen > exp.End)
                            {
                                exp.End = item.Address.Start + item.Address.DataLen;
                            }
                        }
                    }
                }
                //如果为DEBUG，则写
                if (DebugMode == 0)
                {
                    TxtLogWriter.WriteMessage(logfile, this.GetType().FullName+":Rows in Blocks");
                    foreach(S7MultiVarExp exp in m_blocktypes)
                    {
                        TxtLogWriter.WriteMessage(logfile,
                            "BlockType=" + exp.BlockType.ToString() + ";" +
                            "BlockNo=" + exp.BlockNo + ";" +
                            "Start=" + exp.Start + ";" +
                            "End=" + exp.End + "");
                    }

                }
                m_client = new S7Client();
                ret = m_client.ConnectTo(((S7TCPHost)m_host).IPAddress, ((S7TCPHost)m_host).RackNo, ((S7TCPHost)m_host).SlotNo);
                if (ret != 0) throw new Exception("Try ConnectTo Host(" + ((S7TCPHost)m_host).IPAddress.ToString() + ") Error, Errcode=" + ret.ToString());
                ret = Init();
                if (ret == 0)
                {
                    Status = DriverStatus.Inited;
                }
                else
                {
                    throw new Exception("初始化时错误！");
                }
            }
            catch (Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, this.GetType().ToString() + ".AcceptSetting Error:" + ex.Message);
            }
            return ret;
        }

        public override object ReadData(int ItemId)
        {
            return m_values[ItemId];
        }

        public override int Init()
        {
            int ret = 0;
            try
            {
                if (ReadInterval == 0) throw new Exception("主动读取模式下需要设置读取间隔！");              
            }
            catch (Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, this.GetType().ToString() + ".Init Error:" + ex.Message);
                ret = -1;
            }
            return ret;
        }
        public override int Start()
        {
            int ret = 0;
            try
            {
                m_readtimer = new Timer(ReadAndParsing, null, ReadInterval, ReadInterval);
                Status = DriverStatus.Running;
            }
            catch(Exception ex)
            {
                ret = -1;
                TxtLogWriter.WriteErrorMessage(errorfile, this.GetType().ToString() + ".Start Error:" + ex.Message);
            }
            return ret;
        }
        public override int Stop()
        {
            int ret = 0;
            try
            {
                m_readtimer = null;
                m_readtimer.Dispose();
                Status = DriverStatus.Stoped;
            }
            catch(Exception ex)
            {
                ret = -1;
                TxtLogWriter.WriteErrorMessage(errorfile, this.GetType().ToString() + ".Stop Error:" + ex.Message);
            }
            return ret;
        }
        public override void Dispose()
        {
            if (Status == DriverStatus.Running)
            {
                Stop();
            }
            if (Status == DriverStatus.Inited||Status==DriverStatus.Stoped)
            {
                m_client.Disconnect();
                m_client = null;
                m_blocktypes.Clear();
                m_blocktypes = null;
                m_values.Clear();
                m_values = null;
                m_items.Clear();
                m_items = null;
                m_datalen.Clear();
                m_datalen = null;
                m_valuetype.Clear();
                m_valuetype = null;
                m_area.Clear();
                m_area = null;
            }
            base.Dispose();
        }

        public override object ParsingAddress(string address)
        {
            return base.ParsingAddress(address);
        }
        /// <summary>
        /// 根据字符串解析主机地址
        /// </summary>
        /// <param name="host"></param>
        /// <returns>null-不成功</returns>
        public override object ParsingHost(string host)
        {
            S7TCPHost ret = new S7TCPHost
            {
                SlotNo = -1,
                RackNo = -1
            };
            try
            {
                string[] settings = host.Split(';');
                if (settings.Length < 4) { throw new Exception("设置格式不正确"); }
                for (int i = 0; i < settings.Length; i++)
                {
                    string[] row = settings[i].Split('=');
                    switch (row[0].ToLower())
                    {
                        case "ipaddress":
                            IPAddress.Parse(row[1]);
                            ret.IPAddress = row[1];
                            break;
                        case "port":
                            ret.Port = int.Parse(row[1]);
                            break;
                        case "slotno":
                            ret.SlotNo = int.Parse(row[1]);
                            break;
                        case "rackno":
                            ret.RackNo = int.Parse(row[1]);
                            break;
                    }

                }
                if (ret.SlotNo == -1 || ret.RackNo == -1)
                {
                    throw new Exception("解析失败！请确认设置是否正确！");
                }
            }
            catch (Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, "ParsingHost(" + host + ")发生错误:" + ex.Message);
                ret = null;
            }
            return ret;
        }
        /// <summary>
        /// 读取PLC数据并更新值列表
        /// </summary>
        /// <param name="state"></param>
        void ReadAndParsing(object state)
        {
            if (DebugMode == 0) { TxtLogWriter.WriteMessage(logfile, "Begin Read Prepare at" + DateTime.Now.ToString("hh:mm:ss fff")); }
            foreach(S7MultiVarExp exp in m_blocktypes)
            {
                //S7MultiVar mv = new S7MultiVar(m_client);
                byte[] buff = new byte[exp.End - exp.Start];
                //mv.Add(exp.BlockType, S7Consts.S7WLByte, exp.BlockNo, exp.Start, exp.End,ref buff);
                if (DebugMode == 0) { TxtLogWriter.WriteMessage(logfile, "Begin Read at" + DateTime.Now.ToString("hh:mm:ss fff")); }
                //mv.Read();
                int i=m_client.ReadArea(exp.BlockType, exp.BlockType, exp.Start, exp.End - exp.Start, S7Consts.S7WLByte, buff);
                if (i != 0)
                {
                    if (ErrorCount.ContainsKey(i))
                    {
                        ErrorCount[i] += 1;
                    }
                    else
                    {
                        ErrorCount.Add(i, 1);
                    }
                }
                if (DebugMode == 0) { TxtLogWriter.WriteMessage(logfile, "End Read At " + DateTime.Now.ToString("hh:mm:ss fff")); }
                foreach (S7SharpReadItem item in m_items)
                {
                    if (item.Address.BlockArea == exp.BlockType && item.Address.BlockNo == exp.BlockNo)
                    {
                        switch (item.ValueType)
                        {
                            case S7DataType.BIT:
                                m_values[item.Id] = S7.GetBitAt(buff, item.Address.Start-exp.Start, item.Address.DataLen);
                                break;
                            case S7DataType.BYTE:
                                m_values[item.Id] = S7.GetByteAt(buff, item.Address.Start-exp.Start);
                                break;
                            case S7DataType.REAL:
                                m_values[item.Id] = S7.GetRealAt(buff, item.Address.Start-exp.Start);
                                break;
                            case S7DataType.INT:
                                m_values[item.Id]=S7.GetDIntAt(buff,item.Address.Start-exp.Start);
                                break;
                            case S7DataType.INT16:
                                m_values[item.Id] = S7.GetIntAt(buff, item.Address.Start-exp.Start);
                                break;
                            case S7DataType.UINT16:
                                m_values[item.Id] = S7.GetUIntAt(buff, item.Address.Start-exp.Start);
                                break;
                            case S7DataType.UINT32:
                                m_values[item.Id] = S7.GetUDIntAt(buff, item.Address.Start-exp.Start);
                                break;
                            case S7DataType.TEXT:
                                m_values[item.Id] = Encoding.UTF8.GetString(buff, item.Address.Start-exp.Start + 2, item.Address.DataLen-2);
                                break;
                        }
                    }
                }
            }
            if (DebugMode == 0) { TxtLogWriter.WriteMessage(logfile, "end Read Work at" + DateTime.Now.ToString("hh:mm:ss fff")); }
        }
    }

    #region 连续区域类
    public class S7MultiVarExp
    {
        public int BlockType;   //直接转换
        public int BlockNo;
        public int Start;
        public int End;
    }
    #endregion

}
