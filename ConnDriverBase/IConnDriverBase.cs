using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CHQ.RD.DataContract;
using CHQ.RD.DriverBase;
namespace CHQ.RD.ConnDriverBase
{
    public interface IConnDriverBase : IDisposable
    {
        ConnDriverStatus Status { get; set; }
        List<ConnDriverDataItem> DataItems { get; set; }
        ConnDriverSetting ConnDriverSet { get; set; }
        IDriverBase Driver { get; }
        int ID { get; set; }
        Queue<ListKeyValue> ValuesToRead { get; set; }
        int Init();
        int Start();
        int Stop();
        int Close();
        event DataChangeEventHandler DataChange;
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
