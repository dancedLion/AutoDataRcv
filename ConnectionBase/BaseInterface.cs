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
    public interface IConnectionDriver
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
        List<object> ItemList
        {
            get;
            set;
        }
        int ConnectToDevice();
        int TryConnection();
        int EstablishDataList();
        int ReadData(int ItemId);
        int ReadData();
        int AcceptRequest(RequestType requesttype,object requestdata);
        int SendData(int ItemId);
        int SendData();
        int TransactRequest();
        int ParsingConnectionString();
    }
    public interface ConnectionBase
    {
        IConnectionDriver Driver { get; set; }
        int TryConnection();
        int Connect();
        int ReadData();
        int SendToServer();
        int AcceptRequest();
        int ResponseData();
    }
}
