using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CHQ.RD.ConnectorBase
{
    class ConnectorEvent
    {
    }
    public class DataChangeEventArgs : EventArgs
    {
        public DataChangeEventArgs(int itemid, object value)
        {
            m_itemid = itemid;
            m_value = value;
        }
        public int ItemId
        {
            get { return m_itemid; }
        }
        public object Value
        {
            get { return m_value; }
        }
        private int m_itemid;
        private object m_value;
    }
    public delegate void DataChangeEventHandler(object sender, DataChangeEventArgs e);
}
