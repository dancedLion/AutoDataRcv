using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CHQ.RD.S7Sharp7Driver
{
    public class S7ByteConvert
    {
        /// <summary>
        /// 根据字节数组将值转化为相应的varType指定的值，然后转化为float类型
        /// </summary>
        /// <param name="source">输入字节数组</param>
        /// <param name="direction">是否正向0-反向，1-正向</param>
        /// <returns>返回浮点数</returns>
        public static float ToFloat(byte[] source, byte direction)
        {
            float rvalue = 0.0f;
            byte[] temp = new byte[4];
            for (int i = 0; i != temp.Length; ++i)
            {
                temp[i] = source[direction == 1 ? i : (4 - 1 - i)];
            }
            rvalue = BitConverter.ToSingle(temp, 0);
            return (rvalue);
        }
        public static int ToInt(byte[] source, byte direction)
        {
            int rvalue = 0;
            byte[] temp = new byte[4];
            for (int i = 0; i != temp.Length; ++i)
            {
                temp[i] = source[direction == 1 ? i : (4 - 1 - i)];
            }
            rvalue = BitConverter.ToInt32(temp, 0);
            return (rvalue);
        }
        public static Int16 ToInt16(byte[] source, byte direction)
        {
            Int16 rvalue = 0;
            byte[] temp = new byte[2];
            if (direction == 0)
            {
                temp[0] = source[1];
                temp[1] = source[0];
            }
            else
            {
                temp[0] = source[0];
                temp[1] = source[1];
            }
            rvalue = BitConverter.ToInt16(temp, 0);
            return (rvalue);
        }
        public static byte ToBit(byte source, int bit, byte direction)
        {
            int rvalue = source;
            if (direction == 0)
                bit = 7 - bit;
            rvalue = (byte)rvalue << bit;
            rvalue = (byte)rvalue >> 7;
            return ((byte)rvalue);
        }
        public static string ToString(byte[] source, int length, byte direction)
        {
            byte[] temp = new byte[length];
            for (int i = 0; i < length; i++)
            {
                temp[i] = direction == 0 ? source[length - 1 - i] : source[i];
            }
            return Encoding.Default.GetString(temp, 0, length);
        }
        public static UInt16 ToUInt16(byte[] source, byte direction)
        {
            UInt16 rvalue = 0;
            byte[] temp = new byte[2];
            if (direction == 0)
            {
                temp[0] = source[1];
                temp[1] = source[0];
            }
            else
            {
                temp[0] = source[0];
                temp[1] = source[1];
            }
            rvalue = BitConverter.ToUInt16(temp, 0);
            return (rvalue);
        }
        public static UInt32 ToUInt32(byte[] source, byte direction)
        {
            uint rvalue = 0;
            byte[] temp = new byte[4];
            for (int i = 0; i != temp.Length; ++i)
            {
                temp[i] = source[direction == 1 ? i : (4 - 1 - i)];
            }
            rvalue = BitConverter.ToUInt32(temp, 0);
            return (rvalue);
        }
    }
}
