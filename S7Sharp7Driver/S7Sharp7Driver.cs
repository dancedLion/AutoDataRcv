using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using CHQ.RD.DriverBase;
using CHQ.RD.DataContract;
using GeneralOPs;
namespace CHQ.RD.S7Sharp7Driver
{
    

    public class S7Sharp7Driver:DriverBase.DriverBase,IDisposable
    {
        //主机连接
        //S7TCPHost m_host=new S7TCPHost();
        //变量列表
        List<S7SharpReadItem> m_itemlist;
        S7TCPHost m_host;
        S7Client m_client;
        Dictionary<S7DataType, int> m_valuetype;
        Dictionary<string, int> m_dbtype;
        Dictionary<string, int> m_datalen;
        int writedatetime = 0;
        //Dictionary<int, int> m_errorcount;
        string errorfile = AppDomain.CurrentDomain.BaseDirectory + "logs\\S7Sharp7DriverError.log";
        string logfile = AppDomain.CurrentDomain.BaseDirectory + "logs\\S7Sharp7Driver.log";

        public override int SetStatus(DriverStatus status)
        {
            return base.SetStatus(status);
        }

        public S7Sharp7Driver():base()
        {
            HostType = typeof(S7TCPHost);
            AddressType = typeof(S7Address);

            //初始化列表
            m_itemlist = new List<S7SharpReadItem>();
            //m_items = new List<IAddressSetting>();
            m_host = new S7TCPHost();
            m_client = new S7Client();
            m_dbtype = new Dictionary<string, int>();
            m_dbtype.Add("DB", 0x84);
            m_dbtype.Add("MB", 0x83);
            m_dbtype.Add("PE", 0x81);   //IB



            m_valuetype = new Dictionary<S7DataType, int>();
            m_valuetype.Add(S7DataType.BIT, 0x02);
            m_valuetype.Add(S7DataType.BYTE, 0x02);
            m_valuetype.Add(S7DataType.REAL, 0x08);
            m_valuetype.Add(S7DataType.TEXT, 0x02);
            m_valuetype.Add(S7DataType.INT16, 0x04);
            m_valuetype.Add(S7DataType.INT, 0x06);
            m_valuetype.Add(S7DataType.UINT16, 0x04);
            m_valuetype.Add(S7DataType.UINT32, 0x06);

            m_datalen = new Dictionary<string, int>();
            m_datalen.Add("BIT", 1);
            m_datalen.Add("BYTE", 1);
            m_datalen.Add("REAL", 4);
            m_datalen.Add("TEXT", 1);
            m_datalen.Add("INT16", 2);
            m_datalen.Add("UINT16", 2);
            m_datalen.Add("UINT32", 4);
            m_datalen.Add("INT", 4);
        }
        public override void Dispose()
        {
            m_valuetype.Clear();
            m_dbtype.Clear();
            m_datalen.Clear();
            ErrorCount.Clear();
            if (m_itemlist != null)
            {
                m_itemlist.Clear();
            }
            if (m_client.Connected)
            {
                m_client.Disconnect();
            }
            m_client = null;
            m_valuetype = null;
            m_dbtype = null;
            m_datalen = null;
            m_host = null;
            m_itemlist = null;
            ErrorCount = null;
        }
        /// <summary>
        /// 对传入的设置进行转化
        /// </summary>
        /// <param name="host">主机</param>
        /// <param name="list">变量列表</param>
        /// <returns></returns>
        public override int AcceptSetting(object host,object list)
        {
            int ret = -1;
            try
            {
                m_itemlist.Clear();
                m_host = (S7TCPHost)ParsingHost(host.ToString());
                IPAddress hostip = IPAddress.Parse(m_host.IPAddress);
                List<S7SharpItem> tmp_list = new List<S7SharpItem>();
                foreach (ConnDriverDataItem cditem in (List<ConnDriverDataItem>)list) {
                    tmp_list.Add(new S7SharpItem
                    {
                        Id = cditem.Id,
                        ValueType = cditem.ValueType,
                        Address = cditem.Address
                    });
                }
                ret = m_client.ConnectTo(m_host.IPAddress, m_host.RackNo, m_host.SlotNo);
                if (ret != 0)
                {
                    throw new Exception("Connect to Host(" + m_host.IPAddress + ") error,error code=" + ret.ToString());
                }
                foreach(S7SharpItem ssi in tmp_list)
                {
                    S7SharpReadItem ssri = new S7SharpReadItem();
                    ssri.Address = new S7SharpReadAddress();
                    ssri.Id = ssi.Id;
                    ssri.ValueType =(S7DataType)Enum.Parse(typeof(S7DataType), ssi.ValueType);
                    S7Address s7add = (S7Address)ParsingAddress(ssi.Address);
                    ssri.Address.BlockArea = m_dbtype[s7add.BlockType.ToString()];
                    ssri.Address.BlockNo = s7add.BlockNo;
                    ssri.Address.Start = s7add.Start;
                    ssri.Address.WordLen = m_valuetype[ssri.ValueType];//s7add.WordLen; //m_valuetype[ssi.ValueType.ToString()];
                    ssri.Address.DataLen = s7add.DataLen;// (ssri.ValueType == S7DataType.BIT ? 1 : s7add.DataLen / S7.DataSizeByte(m_valuetype[ssi.ValueType.ToString()]));
                    m_itemlist.Add(ssri);
                }
                //每个值的试读
                //ret = SettingAddress();
                //地址设置的错误不影响整 体的使用
                Status = DriverStatus.Inited;
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, "AcceptSetting(" + host.ToString() + "):" + ex.Message);
                //ret = -1;
                Status = DriverStatus.Error;
            }
            return ret;
        }

        public override int Start()
        {
            int ret = 0;
            try
            {
                if (m_client.Connected)
                {

                }
                else
                {
                    m_client.ConnectTo(m_host.IPAddress, m_host.RackNo, m_host.SlotNo);
                }
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, this.GetType().FullName + ".Start Error:" + ex.Message);
                ret = -1;
            }
            return ret;
        }
        public override int Stop()
        {
            int ret = -1;
            try
            {
                m_client.Disconnect();
                ret = 1;
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, this.GetType().FullName + ".Stop Error:" + ex.Message);
            }
            return ret;
        }

        /// <summary>
        /// 读取数据，直接转换为相应的类型设置
        /// </summary>
        /// <param name="ItemId">数据ID</param>
        /// <returns></returns>
        public override object ReadData(int ItemId)
        {
            //if (Status != DriverStatus.Inited) { return null; }
            object ret = null;
            try
            {
                lock (this)
                {
                    S7SharpReadItem s7item = m_itemlist.Find((S7SharpReadItem x) => x.Id == ItemId);
                    if (s7item == null)
                    {
                        throw new Exception("指定ID的变量不存在(" + ItemId.ToString() + ")");
                    }

                    ///
                    int amount = s7item.ValueType == S7DataType.BIT ? 1 : s7item.Address.DataLen / S7.DataSizeByte(s7item.Address.WordLen);
                    byte[] buffer = new byte[12];
                    //if (writedatetime == 0) TxtLogWriter.WriteMessage(logfile, "ItemID=" + s7item.Id + " begin at" + DateTime.Now.ToString("hh:mm:ss fff"));
                    int i = m_client.ReadArea(
                           s7item.Address.BlockArea,
                           s7item.Address.BlockNo,
                           s7item.Address.Start,
                           amount,
                           s7item.Address.WordLen,
                           buffer
                            );
                    //if (writedatetime == 0) TxtLogWriter.WriteMessage(logfile, "ItemID=" + s7item.Id + " End At" + DateTime.Now.ToString("hh:mm:ss fff"));
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
                        throw new Exception("read data error,ItemId=" + s7item.Id + " ErrorCode=" + i.ToString() + " !");
                    }
                    ///
                    //object t = ReadDeviceData(s7item);

                    switch (s7item.ValueType)
                    {
                        case S7DataType.BIT:
                            ret = S7ByteConvert.ToBit(buffer[0], s7item.Address.DataLen, 0);
                            break;
                        case S7DataType.BYTE:
                            ret = buffer[0];
                            break;
                        case S7DataType.INT:
                            ret = S7ByteConvert.ToInt(buffer, 0);
                            break;
                        case S7DataType.INT16:
                            ret = S7ByteConvert.ToInt16(buffer, 0);
                            break;
                        case S7DataType.REAL:
                            ret = S7ByteConvert.ToFloat(buffer, 0);
                            break;
                        case S7DataType.UINT16:
                            ret = S7ByteConvert.ToUInt16(buffer, 0);
                            break;
                        case S7DataType.UINT32:
                            ret = S7ByteConvert.ToUInt32(buffer, 0);
                            break;
                        case S7DataType.TEXT:
                            ret = Encoding.Default.GetString(buffer, 2, s7item.Address.DataLen - 2);
                            break;
                        default:
                            ret = BitConverter.ToString(buffer);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile,"S7SharpDriver ReadData Error:" + m_host.IPAddress + ";" + ItemId.ToString() + ":" + ex.Message);
            }
            return ret;
        }

        /// <summary>
        /// 读取硬件数据，不负责数据转换
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public override object ReadDeviceData(object t)
        {
            object ret=null;
            int i = 0;
            try
            {
                //i = m_instance.ReadArea(
                //                           m_memeryType[item.BlockType.ToString()],
                //                           item.Block,
                //                           item.StartAddress,
                //                           item.ValueType.ToString() == "BIT" ? 1 : (item.Length / S7.DataSizeByte(m_valueType[item.ValueType.ToString()])),
                //                           m_valueType[item.ValueType.ToString()],
                //                           buffer);
                S7SharpReadItem item = (S7SharpReadItem)t;
                int amount = item.ValueType == S7DataType.BIT ? 1 : item.Address.DataLen / S7.DataSizeByte(item.Address.WordLen);
                byte[] buffer = new byte[12];
                
                i = m_client.ReadArea(
                       item.Address.BlockArea,
                       item.Address.BlockNo,
                       item.Address.Start,
                       amount,
                       item.Address.WordLen,
                       buffer
                        );

                if (i != 0)
                {
                    if (ErrorCount.ContainsKey(i)){
                        ErrorCount[i] += 1;
                    }
                    else
                    {
                        ErrorCount.Add(i, 1);
                    }
                    throw new Exception("read data error,ItemId="+item.Id+" ErrorCode=" + i.ToString() + " !");
                }
                ret = buffer;
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile,this.ToString() + ":ReadDeviceData error:" + ex.Message);
            }
            return ret;
            //return base.ReadDeviceData(ItemId);
        }

        /// <summary>
        /// 尝试连接到硬件
        /// </summary>
        /// <returns>0-成功</returns>
        public override int TryConnectToDevice()
        {
            int ret = -1;
            try {
                //根据设置创建临时对象并赋设置值 
                S7Client tmp = new S7Client();
                ret=tmp.ConnectTo(m_host.IPAddress, m_host.RackNo, m_host.SlotNo);
                if (!tmp.Connected)
                {
                    throw new Exception("连接主机失败:" + ret.ToString());
                }                
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage("S7SharpDriver.TryConnectoToDevice("+m_host.ToString()+"):" + ex.Message);
            }
            return ret;
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
                for(int i = 0; i < settings.Length; i++)
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
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, "ParsingHost(" + host + ")发生错误:" + ex.Message);
                ret = null;
            }
            return ret;
        }
        /// <summary>
        /// 根据地址字符串解析地址
        /// </summary>
        /// <param name="address"></param>
        /// <returns>null-不成功</returns>
        public override object ParsingAddress(string address)
        {
            S7Address ret = new S7Address();
            string[] rows = address.Split(';');
            for(int i = 0; i < rows.Length; i++)
            {
                string[] kvp = rows[i].Split('=');
                if (!string.IsNullOrEmpty(kvp[1]))
                {
                    switch (kvp[0])
                    {
                        case "BlockType":
                            ret.BlockType = (S7BlockType)int.Parse(kvp[1]);
                            break;
                        case "Start":
                            ret.Start = int.Parse(kvp[1]);
                            break;
                        case "BlockNo":
                            ret.BlockNo = int.Parse(kvp[1]);
                            break;
                        case "WordLen":
                            ret.WordLen = int.Parse(kvp[1]);
                            break;
                        case "DataLen":
                            ret.DataLen = int.Parse(kvp[1]);
                            break;
                    }
                }
            }
            if (ret == null)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, "ParsingAddress("+address+"):解释地址出错！");
            }
            return ret;
        }
    }
}
