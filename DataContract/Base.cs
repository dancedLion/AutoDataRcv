using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CHQ.RD.DataContract
{
    public class Base
    {
    }

    public enum ConnDriverStatus
    {
        None,
        Inited,
        Running,
        Pausing,
        Stoped,
        Closed
    }


    public class DriverSetting
    {
        public int ReadInterval;
        public int ReadMode;
        public int TransMode;
    }
    public class ConnDriverSetting
    {
        public int Id;
        public int ReadInterval;
        public int ReadMode;
        public int TransMode;
        public string Host;
        public List<ConnectorDataItem> DataItems;
        public DriverSetting DriverSet;
    }
    public class ConnectorSetting
    {
        public int Id;
        public string Hosts;
        public List<ConnDriverSetting> ConnDriverSet;
        public List<ConnectorDataItem> DataItems;
    }
    public class ConnDriverSetting
    {
        public int Id;
        public string Host;
        public 
    }
    public class ConnDriverDataItem
    {
        public int Id { get; set; }
        public object Address { get; set; }
        public string ValueType { get; set; }
    }
    public class ConnectorDataItem
    {
        public int ConnDriverId { get; set; }
        public string TransSig { get; set; }
        public ConnDriverDataItem DataItemSetting { get; set; }
    }
}
