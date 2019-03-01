using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CHQ.RD.DataContract
{
    class S7
    {
    }
    public enum S7BlockType
    {
        DB,
        MB,
        PE
    }
    public enum S7DataType
    {
        BIT,
        BYTE,
        REAL,
        TEXT,
        INT,
        INT16,
        UINT16,
        UINT32
    }
    public enum S7Status
    {
        STOP,
        RUN,
        PAUSE
    }
    public class S7Address
    {
        public S7BlockType BlockType;
        public int BlockNo;
        public int Start;
        public int DataLen;
        public int WordLen;
    }
    public class S7TCPHost
    {
        public string IPAddress;
        public int Port;
        public int RackNo;
        public int SlotNo;
    }

}
