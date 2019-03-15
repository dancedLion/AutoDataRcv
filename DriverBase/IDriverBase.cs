using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHQ.RD.DriverBase
{
    public interface IDriverBase:IDisposable
    {
        int TransMode { get; set; } //数据传输模式，0-等待读取 1-主动发送
        int ReadMode { get; set; }  //从设备读取数据的模式，0-由连接器请求 1-连接器请求后不间断读取
        int ReadInterval { get; set; }  //从设备读取数据的时间间隔
        object ReadData(int ItemId);
        /// <summary>
        /// 获取设置
        /// </summary>
        /// <param name="host">主机设置</param>
        /// <param name="list">地址列表</param>
        /// <returns></returns>
        int AcceptSetting(object host, object list);
    }
}
