using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
namespace CHQ.RD.DataContract
{
    class SerialDriver
    {
    }
    public class SerialHost
    {
        public string comPort;
        public int baudRate;
        public int parity;
        public int dataBits;
        public string stopBits;
    }

    public class SerialAddress
    {
        //起始位、停止位、有效数据、校验位
        public int Start;
        public int Length;
        public char StartFlag;
        public char EndFlag;
    }

    public class SerialItem
    {
        public int ID;
        public SerialAddress Address;
        public S7DataType ValueType;
    }
}
