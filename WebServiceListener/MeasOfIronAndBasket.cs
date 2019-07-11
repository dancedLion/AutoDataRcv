/*此驱动用于从计量数据库中读取以下数据：
 * 废钢车间料蓝称重
 * 轨道衡铁水数据
 * 数据传送给驱动连接器后，由驱动连接器负责写入到炼钢计量系统的指定数据库中
 * 作者：徐震
 * 开始编码日期：2019年7月11日
 * 为了减少客户端上的配置，从webservice取数,每次取数成功将更新上次取数日期
 * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CHQ.RD.DriverBase;
using CHQ.RD.DataContract;
namespace CHQ.RD.WebServiceListener
{
    public class MeasOfIronAndBasket:DriverBase.DriverBase
    {
        
        public MeasOfIronAndBasket() : base()
        {

        }

        public override int AcceptSetting(object host, object list)
        {
            return base.AcceptSetting(host, list);
        }
        public override int Start()
        {
            return base.Start();
        }
        public override int Stop()
        {
            return base.Stop();
        }
        
    }
}
