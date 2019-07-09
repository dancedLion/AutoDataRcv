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
    //基础类型，上升为所有驱动所使用的类型
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
    public class S7Address:IHostDataAddress
    {
        public S7BlockType BlockType;
        public int BlockNo;
        public int Start;
        public int DataLen;
        public int WordLen;
        public override string ToString()
        {
            return "BlockType=" + BlockType.ToString() + ";BlockNo=" + BlockNo.ToString() + ";Start=" + Start.ToString() + ";DataLen=" + DataLen.ToString() + ";WordLen=" + WordLen.ToString();
        }
        //通过解析字符串获取地址值 
        public object Parsing(string addressString)
        {
            S7Address ret = new S7Address();
            string[] flds = addressString.Split(';');
            for(int i = 0; i < flds.Length; i++)
            {
                string[] keyvalue = flds[i].Split('=');
                switch (keyvalue[0])
                {
                    case "BlockType":
                        ret.BlockType = (S7BlockType)int.Parse(keyvalue[1]);
                        break;
                    case "BlockNo":
                        ret.BlockNo = int.Parse(keyvalue[1]);
                        break;
                    case "Start":
                        ret.Start = int.Parse(keyvalue[1]);
                        break;
                    case "DataLen":
                        ret.DataLen = int.Parse(keyvalue[1]);
                        break;
                    case "WordLen":
                        ret.WordLen = int.Parse(keyvalue[1]);
                        break;
                }
            }
            return ret;
        }
    }
    public class S7TCPHost
    {
        public string IPAddress;
        public int Port;
        public int RackNo;
        public int SlotNo;
        public override string ToString()
        {
            return "IPAddress=" + IPAddress + ";Port=" + Port.ToString() + ";RackNo=" + RackNo.ToString() + ";SlotNo=" + SlotNo.ToString();
        }
    }


    //主机地址接口，要求主机地址均可实现从字符串解析；

}
