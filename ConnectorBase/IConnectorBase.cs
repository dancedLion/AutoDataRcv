using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CHQ.RD.DataContract;
namespace CHQ.RD.ConnectorBase
{
    public interface IConnectorBase
    {
        //int AcceptValue(int ItemId);    //接受来自于驱动器的值
        //int ReadValue(int ItemId);  //从驱动器中读取值

        //int EstablishTCPServer();

        //int EstablishValueServer();
        //int SendDataViaUDP();
        //int SendDataViaTCP();
        //int RegisterDriver();
        //int WriteValueToDB();

        //int DoAdditionalEvent();
        //int ConnectDriverInstance();
        //int SetDriverInstanceItems();
        //int DropDriverInstance();
    }

    public interface IConnDriverBase
    {
        ConnDriverStatus Status{ get; set; }
        int Init();
        int Start();
        int Stop();
        int Close();
        /// <summary>
        /// 重新初始化并启动
        /// </summary>
        /// <returns>0-成功</returns>
        int Restart();
        void AcceptValue();
        //object ConnectDriver();
        void ReadData(object state);
    }
}
