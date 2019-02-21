using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContract
{
    public class Class1
    {
    }

    public class S7DataAddress
    {
        public string BlockType;
        public int BlockAddress;
        public int ByteAddress;
        public int DataLength;  //如果是bit类型，则该数据填写bit位
        public string DataType;
    }
}
