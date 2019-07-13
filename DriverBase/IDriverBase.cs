/*驱动器标准接口
 * 不间断读取的方式，如：tcpclient，如果是tcpclient，那必须使用相应的机制在收到数据后及时发送出去
 * 使用tcpclient和server是最简单的，
 * 
 * 
 */ 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHQ.RD.DataContract;
namespace CHQ.RD.DriverBase
{
    public interface IDriverBase:IDisposable
    {
        int TransMode { get; set; } //数据传输模式，0-等待读取 1-主动发送
        int ReadMode { get; set; }  //从设备读取数据的模式，0-由连接器请求 1-连接器请求后不间断读取
        int ReadInterval { get; set; }  //从设备读取数据的时间间隔
        DriverStatus Status { get; set; }
        Type HostType { get; }
        Type AddressType { get; }
        Dictionary<int,int> ErrorCount { get; set; }
        object ValueList { get; set; }
        int DebugMode { get; set; }
        /// <summary>
        /// 从设备读取数据
        /// </summary>
        /// <param name="ItemId">数据ID，目的是为了获取地址设置</param>
        /// <returns></returns>
        object ReadData(int ItemId);
        /// <summary>
        /// 获取设置
        /// </summary>
        /// <param name="host">主机设置</param>
        /// <param name="list">地址列表</param>
        /// <returns></returns>
        int AcceptSetting(object host, object list);

        /// <summary>
        /// 主动发送数据
        /// </summary>
        /// <param name="values">数据键值列表</param>
        /// <returns></returns>
        int SendData(object values);
        /// <summary>
        /// 尝试连接硬件设备
        /// </summary>
        /// <returns>0-1成功，其他-失败</returns>
        int TryConnectToDevice();

        object ParsingHost(string host);
        object ParsingAddress(string address);

        void StartListener();
        int Start();
        int Stop();
    }

}
