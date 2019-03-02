using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CHQ.RD.ConnectorBase
{
    public interface IConnectorBase
    {
        int AcceptValue(int ItemId);    //接受来自于驱动器的值
        int ReadValue(int ItemId);  //从驱动器中读取值

        int EstableTCPServer();

        int EstableValueServer();
        int SendDataViaUDP();
        int SendDataViaTCP();
        int RegisterDriver();
        int WriteValueToDB();

        int DoAdditionalEvent();
        int ConnectDriverInstance();
        int SetDriverInstanceItems();
        int DropDriverInstance();
    }
}
