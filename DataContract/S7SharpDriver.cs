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
    public enum S7SharpHostStatus
    {
        NOTCONNECT,
        CONNECTED,
        RUNNING,
        PAUSING,
        RUNNINGWITHERROR,
        REQUIREINIT
    }

    public class S7SharpReadAddress
    {
        public int BlockArea;
        public int BlockNo;
        public int Start;
        public int DataLen;
        public int WordLen;
    }

    public class S7SharpItem
    {
        public int Id;
        public S7Address Address;
        public S7DataType ValueType;
    }

    public class S7SharpReadItem
    {
        public int Id;
        public S7SharpReadAddress Address;
        public S7DataType ValueType;
    }
}
