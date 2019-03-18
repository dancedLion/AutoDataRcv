using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CHQ.RD.DataContract
{
    class S7SharpDriver
    {
    }

    public enum S7SharpHostType
    {
        TCP,
        MPI
    }
    public class S7SharpReadAddress
    {
        public int BlockArea;
        public int BlockNo;
        public int Start;
        public int DataLen;
        public int WordLen;
    }

    public class S7SharpItem:ConnDriverDataItem
    {
        //S7Address m_address;
        //int m_id;
        //S7DataType m_valuetype;
        //public int Id
        //{
        //    get { return m_id; }
        //    set { m_id = value; }
        //}
        //public object Address
        //{
        //    get { return m_address; }
        //    set { m_address =(S7Address)value; }
        //}
        //public string ValueType
        //{
        //    get { return m_valuetype.ToString(); }
        //    set { m_valuetype =(S7DataType)Enum.Parse(typeof(S7DataType),value); }
        //}
    }

    public class S7SharpReadItem
    {
        public int Id;
        public S7SharpReadAddress Address;
        public S7DataType ValueType;
    }
}
