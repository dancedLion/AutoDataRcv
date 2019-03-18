using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CHQ.RD.DataContract;
namespace GeneralOPs
{
    public class Class1
    {
    }
    public class GeneralOps
    {
        static string errorfile = AppDomain.CurrentDomain.BaseDirectory + "\\logs\\GeneralOpsError.log";
        static string logfile = AppDomain.CurrentDomain.BaseDirectory + "\\logs\\GeneraOps.log";
        public static object ParsingS7TCPHost(string host)
        {
            S7TCPHost ret = null;
            try
            {

            }
            catch(Exception ex)
            {

            }
            return ret;
        }

        public static object ParsingS7Address(string address)
        {
            S7Address ret = null;
            return ret;
        }
    }
}
