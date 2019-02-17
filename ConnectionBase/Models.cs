using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionBase
{
    public class Models
    {
    }
    public enum RequestType
    {
        EstablishConnection,
        SendVariantList,
        Testing,
        GetDatas,
        Reset
    }
    public class SimensS7Host
    {
        public string HostName;
        public int HostPort;
    }
    public class SimensS7VariantAddress
    {
        public string BlockType;
        public int BlockAddress;
        public int StartAddress;
        public int BitAddress;
        public int DataLength;
    }

    public class OPCHost
    {
        public int HostType;        //主机类型，是UA还是Classic
        public string HostName;
        public string HostProgId;   //UA-port,int, Classic,ProgId
    }
    public class OPCVariantAddress
    {
        public int ClientHandle;
        public string ItemId;
        public string DataType;
    }

    public enum DriverErrors
    {
        ParsingConnectionStringError,
        TryConnectToDeviceError,
        ReadDataError,
        Other
    }
}
