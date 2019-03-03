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

    public enum ConnDriverStatus
    {
        None,
        Inited,
        Running,
        Pausing,
        Stoped,
        Closed
    }

    public interface IConnectorDataItem
    {
        int Id { get; set; }
        object Address { get; set; }
        string ValueType { get; set; }
    }
}
