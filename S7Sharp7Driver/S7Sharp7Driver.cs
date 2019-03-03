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
        Dictionary<string, int> m_valuetype;
        Dictionary<string, int> m_dbtype;
        Dictionary<string, int> m_datalen;
        Dictionary<int, int> m_errorcount;

        S7SharpHostStatus m_status;
        public S7SharpHostStatus DriverStatus
        {
            get { return m_status; }
            set { SetStatus(value); }
        }

        public override int SetStatus(object status)
        {

            return base.SetStatus(status);
        }

        public S7Sharp7Driver()
        {
            //初始化列表
            m_itemlist = new List<S7SharpReadItem>();
            
            m_items = new List<IAddressSetting>();
            m_host = new S7TCPHost();
            m_client = new S7Client();
            m_dbtype = new Dictionary<string, int>();
            m_dbtype.Add("DB", 0x84);
            m_dbtype.Add("MB", 0x83);
            m_dbtype.Add("PE", 0x81);   //IB

            m_valuetype = new Dictionary<string, int>();
            m_valuetype.Add("BIT", 0x02);
            m_valuetype.Add("BYTE", 0x02);
            m_valuetype.Add("REAL", 0x08);
            m_valuetype.Add("TEXT", 0x02);
            m_valuetype.Add("INT16", 0x04);
            m_valuetype.Add("INT", 0x06);
            m_valuetype.Add("UINT16", 0x04);
            m_valuetype.Add("UINT32", 0x06);

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
        public void Dispose()
        {
            m_valuetype.Clear();
            m_dbtype.Clear();
            m_datalen.Clear();
            m_errorcount.Clear();
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
            m_errorcount = null;
        }
        public int AcceptSetting(object host,object list)
        {
            int ret = 1;
            
            try
            {
                m_itemlist.Clear();
                m_host = (S7TCPHost)host;
                IPAddress hostip = IPAddress.Parse(m_host.IPAddress);
                List<S7SharpItem> tmp_list = (List<S7SharpItem>)list;
                ret = m_client.ConnectTo(m_host.IPAddress, m_host.RackNo, m_host.SlotNo);
                if (ret != 0)
                {
                    throw new Exception("Connect to Host(" + m_host.IPAddress + ") error,error code=" + ret.ToString());
                }
                foreach(S7SharpItem ssi in tmp_list)
                {
                    S7SharpReadItem ssri = new S7SharpReadItem();
                    ssri.Id = ssi.Id;
                    ssri.ValueType =(S7DataType)Enum.Parse(typeof(S7DataType), ssi.ValueType);
                    ssri.Address.BlockArea = m_dbtype[((S7Address)ssi.Address).BlockType.ToString()];
                    ssri.Address.BlockNo = ((S7Address)ssi.Address).BlockNo;
                    ssri.Address.Start = ((S7Address)ssi.Address).Start;
                    ssri.Address.WordLen = m_valuetype[ssi.ValueType.ToString()];
                    ssri.Address.DataLen = ssri.ValueType == S7DataType.BIT ? 1 : (((S7Address)ssi.Address).DataLen / S7.DataSizeByte(m_valuetype[ssi.ValueType.ToString()]));
                    m_itemlist.Add(ssri);
                }
                //每个值的试读
                ret = SettingAddress();
                //地址设置的错误不影响整 体的使用
                if (ret != 1)
                {
                    ret = 1;
                }
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(this.ToString() + ":AcceptSetting(" + host.ToString() + "," + list.ToString() + "):" + ex.Message);
                ret = -1;
            }
            return ret;
        }
        //public override int SettingAddress()
        //{
        //    //return base.SettingAddress();
        //    //测试每个地址的设置是否OK，如果错误就返回错误值 
        //    int result = 1;
        //    try
        //    {
        //        foreach (S7SharpItem add in m_itemlist)
        //        {
        //            S7Address address = (S7Address)add.Address;
        //            S7Client.S7DataItem item = new S7Client.S7DataItem();
        //            item.Area = m_dbtype[address.BlockType.ToString()];
        //            item.DBNumber = address.BlockAddress;
        //            item.Start = address.ByteAddress;
        //            item.Amount = address.DataType.ToString() == "BIT" ? 1 : (address.DataLength / S7.DataSizeByte(m_valuetype[address.DataType.ToString()]));
        //            item.WordLen = m_datalen[address.DataType.ToString()];
        //            S7DataItemSetting s7item = new S7DataItemSetting();
        //            s7item.Id = add.Id; s7item.DataItem = item;
        //            m_dataitem.Add(s7item);
        //        }
        //        foreach(S7DataItemSetting item in m_dataitem)
        //        {
        //            object t = ReadDeviceData(item);
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        TxtLogWriter.WriteErrorMessage(this.ToString() + ":SettingAddress ERROR:" + ex.Message);
        //        result = -1;
        //    }
        //    return result;
        //}

        public override object ReadData(int ItemId)
        {
            object ret = null;
            try
            {
                S7SharpReadItem s7item = m_itemlist.Find((S7SharpReadItem x) => x.Id == ItemId);
                if (s7item == null)
                {
                    throw new Exception("指定ID的变量不存在("+ItemId.ToString()+")");
                }
                byte[] buffer = (byte[])ReadDeviceData(s7item.Address);
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
            catch (Exception ex)
            {
                TxtLogWriter.WriteErrorMessage("S7SharpDriver read DataError:" + m_host.IPAddress + ";" + ItemId.ToString() + ":" + ex.Message);
            }
            return ret;
        }


        public override object ReadDeviceData(object t)
        {
            object ret=null;
            int i = 0;
            try
            {
                //S7Client client = new S7Client();
                //ret=client.ConnectTo(m_host.IpAddress, m_host.RackNo, m_host.SlotNo);
                //S7SharpReadItem s7item = m_itemlist.Find((S7SharpReadItem x) => x.Id == ItemId);
                //if (s7item == null) throw new Exception("指定的ItemID不在列表中！");
                //S7SharpReadAddress item = s7item.Address;
                S7SharpReadAddress item = (S7SharpReadAddress)t;
                byte[] buffer = new byte[12];
                i = m_client.ReadArea(
                       item.BlockArea,item.BlockNo,item.Start,item.DataLen,item.WordLen,buffer
                        );
                if (i != 0)
                {
                    if (m_errorcount.ContainsKey(i)){
                        m_errorcount[i] += 1;
                    }
                    else
                    {
                        m_errorcount.Add(i, 1);
                    }
                    throw new Exception("read data error, item id=" + t.ToString() + "!");
                }
                //else
                //{
                //    switch (item.valuetype)
                //    {
                //        case "BIT":
                //            ret = S7ByteConvert.ToBit(buffer[0], address.DataLength, 0);
                //            break;
                //        case "BYTE":
                //            ret = buffer[0];
                //            break;
                //        case "INT":
                //            ret = S7ByteConvert.ToInt(buffer, 0);
                //            break;
                //        case "INT16":
                //            ret = S7ByteConvert.ToInt16(buffer, 0);
                //            break;
                //        case "REAL":
                //            ret = S7ByteConvert.ToFloat(buffer, 0);
                //            break;
                //        case "UINT16":
                //            ret = S7ByteConvert.ToUInt16(buffer, 0);
                //            break;
                //        case "UINT32":
                //            ret = S7ByteConvert.ToUInt32(buffer, 0);
                //            break;
                //        case "TEXT":
                //            ret = Encoding.Default.GetString(buffer, 2, address.DataLength - 2);
                //            break;
                //        default:
                //            ret = BitConverter.ToString(buffer);
                //            break;

                //    }
                //}
                ret = buffer;
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(this.ToString() + ":ReadDeviceData error:" + ex.Message);
            }
            return ret;
            //return base.ReadDeviceData(ItemId);
        }
    }
}
