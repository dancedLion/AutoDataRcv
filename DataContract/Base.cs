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
        None,   //只是定义
        Inited, //初始化
        Running,    //运行中
        Pausing,    //暂停中，已经弃用
        Stoped,     //已停止，
        Closed,     //已关闭，比停止多出来释放变量
        AutoErrorTransacting,   //如果驱动错误计数在10次及以上，将停止并且开始重连
        Error       //错误，手工操作时才会有，运行中的只会有上一选项
    }

    public enum DriverStatus
    {
        None,       //只定义不初始化
        Inited,     //初始化
        Running,    //主动读取模式下，运行中
        Stoped,     //主动读取模式下，停止中
        Closed,     //已关闭,不起作用
        Error       //错误状态中
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
        public int Port;
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

    public class DataSendingSet
    {
        public int Id;
        public string Name;
        public string Host;
        public int HostPort;
        public int SendInterval;    //如果为零，则表示有变化就发送；
        public string Memo;
        public string ConnDrivers;  //预留，为了不同驱动连接器的数据发送给不同的主机
        public int Via;     //发送方式 0-Socket,1-TCP 2-UDP
    }

    public class ListKeyValue
    {
        public int Id;
        public object Value;
    }

    public class NetworkListenerHostType
    {
        public int Via; //方式，0-socket 1-udp 2-tcp
        public int HostPort;    //监听端口
    }
    public class NetworkListenerAddressType
    {
        public int VarId;
    }
}
