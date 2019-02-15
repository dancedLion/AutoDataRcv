using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionBase
{
    interface BaseInterface
    {
        List<string> LogList
        {
            get;
        }

    }
    public interface ConnectionDriver
    {
        string ConnectionString
        {
            get;set;
        }
        List<string> LogList
        {
            get;
        }
        Dictionary<int,object> ValueList
        {
            get;
        }
        int TryConnection();
    }
    public interface ConnectionBase
    {
        ConnectionDriver Driver { get; set; }
        int TryConnection();
        int Connect();
        int ReadData();
        int SendToServer();
        int AcceptRequest();
        int ResponseData();
    }
}
