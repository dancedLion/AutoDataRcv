using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CHQ.RD.DataContract
{
    class SerialDriver
    {
    }
    public class SerialHost
    {
        public string ComPort;
        public int BaudRate;
    }

    public class SerialAddress
    {
        //起始位、停止位、有效数据、校验位
        public int BagLength;
        public int StartBits;
        public int StopBits;
        public int DataStartByte;
        public int DataStopByte;
    }

    public class SerialItem
    {
        public int ID;
        public SerialAddress Address;
        public SerialDataType ValueType;
    }
}
