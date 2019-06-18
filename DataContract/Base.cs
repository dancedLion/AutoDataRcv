/**************自我保护*********************
 * 驱动和驱动连接器的数据读取模式
 * 驱动：
 * ReadMode->0:按照interval读取主机（硬件）数据,1-主动读取数，忽略interval
 * TransMode->0:由连接器来读,1-主动发送给连接器，需要连接器配合
 * 驱动连接器：
 * ReadMode->0:读取驱动中的数据，1->由指定的程序来接收驱动发送的数据
 * TransMode->0:由驱动管理器来读，1->更新驱动管理器的数据
 * 连接管理器：
 * 当ReadMode为0时读取数据（驱动连接器的模式设置为0）并更新，当为1时，啥也不用做
 * **连接管理器才能往外发送数据
 * **连接管理器负责报警事件
 * **连接管理器负责附加事件
 */ 
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

    public interface IHostDataAddress
    {
        void Parsing(string addressString);
    }



    public enum ConnDriverStatus
    {
        None,
        Inited,
        Running,
        Pausing,
        Stoped,
        Closed,
        Error
    }

    public enum DriverStatus
    {
        None,
        Inited,
        Running,
        Stoped,
        Closed,
        Error
    }
    public class DriverSetting
    {
        public string Host;
        public int ReadInterval;
        public int ReadMode;
        public int TransMode;
        public int ErrorTransactInterval;
    }
    public class ConnDriverSetting
    {
        public int Id;      //唯一ID
        public string Name;
        public int ReadMode;    //
        public int ReadInterval;    //
        public int TransMode;   //传送模式
        public AssemblyFile ClassFile;    //驱动编辑及类型
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
        public int Id;
        public string Name;
        public int ConnId;
        public string TransSig;
        public string Address;
        public string ValueType;
    }

    public class AssemblyFile
    {
        public int Id;
        public string DriverName;
        public string ClassName;
        public string AssemblyInfo;
        public string FileName;
    }
}
