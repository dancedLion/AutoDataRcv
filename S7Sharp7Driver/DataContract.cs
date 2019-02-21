using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataContract;
using DriverBase;
using GeneralOPs;
using System.Net;
namespace S7Sharp7Driver
{
    class DataContract
    {
    }
    public class S7Sharp7Host
    {
        public string IpAddress;
        public int Port;
        public int RackNo;
        public int SlotNo;
    }
    public class S7DataItem
    {
        public int Id;
        public S7Client.S7DataItem DataItem;
    }
    public class S7Sharp7AddressSetting : IAddressSetting
    {
        S7DataAddress m_itemaddress;
        int m_id;
        string m_valuetype;
        public int Id
        {
            get { return m_id; }
            set { m_id = value; }
        }
        public string ValueType
        {
            get { return m_valuetype; }
            set { m_valuetype = value; }
        }
        public object Address
        {
            get
            {
                return m_itemaddress;
            }
            set
            {
                parsingaddress(value);
            }
        }
        void parsingaddress(object address)
        {
            try
            {
                m_itemaddress = (S7DataAddress)address;
            }
            catch(Exception ex)
            {
                m_itemaddress = null;
                TxtLogWriter.WriteErrorMessage(this.ToString() + ";parsingaddress(" + address.ToString() + "):" + ex.Message);
            }
        }
    }


}
