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
        public string Host;
        public int ReadInterval;
        public int ReadMode;
        public int TransMode;
    }
    public class ConnDriverSetting
    {
        public int Id;      //唯一ID
        public string Name;
        public int ReadInterval;    //
        public int ReadMode;    //
        public int TransMode;   //传送模式
        public string DriverType;   //驱动类型
        public string AssemblyFile;
        public List<ConnectorDataItem> DataItems;
        public DriverSetting DriverSet; //驱动设置
    }
    public class ConnectorSetting
    {
        public int Id;
        public string Hosts;
        public List<ConnDriverSetting> ConnDriverSet;
        public List<ConnectorDataItem> DataItems;
    }
    public class ConnDriverDataItem
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string ValueType { get; set; }
    }
    public class ConnectorDataItem
    {
        public int ConnId;
        public string TransSig;
        public string Name;
        public int Id;
        public string Address;
        public string ValueType;
    }
}
