using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHQ.RD.DriverBase
{
    public interface IDriverBase
    {

    }

    public interface IAddressSetting
    {
        int Id { get; set; }
        string ValueType { get; set; }
        object Address { get; set; }
    }
}
