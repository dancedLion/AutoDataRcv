/*==============保护生命======================
 * 徐震 2019年3月
 * 此连接品是驱动和连接管理器的中间层，用以实现
 * 设置主机和变量
 * 设置驱动类型
 * 设置完成后测试连接
 * 如果连接可用，则开始取数
 * 连接器负责建立键-值表，以供连接管理器定时更新
 * 连接管理器向连接取数，
 * 应用向连接管理器取数（应用-连接管理器-连接-单个驱动）
 *  连接和驱动为一对 ，连接管理器管理多个连接，应用连接多个连接管理器
 *  连接和驱动为专门编程的性质，连接管理器负责从连接取数，此为标准应用，
 *  连接管理器可向某个标准应用发送数据
 *  如果是视频如何处理-2019-3-7？
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CHQ.RD.DriverBase;
using CHQ.RD.DataContract;
using System.Reflection;    //创建驱动实例用
namespace CHQ.RD.ConnectorBase
{
    public class ConnDriverBase
    {

        public ConnDriverBase(int id)
        {

        }
        int m_id = -1;
        public int ID
        {
            get { return m_id; }
        }
        IDriverBase m_driver;
        public IDriverBase Driver
        {
            get { return m_driver; }
        }
        Type m_driverclass;
        public Type DriverClass
        {
            get { return m_driverclass; }
            set { m_driverclass = value; }
        }
        ConnDriverStatus m_status = ConnDriverStatus.None;
        public ConnDriverStatus Status
        {
            get { return m_status; }
            set { SetStatus(value); }
        }
        Dictionary<int, int> m_errorCount;
        string errorfile = AppDomain.CurrentDomain.BaseDirectory + "\\logs\\ConnDriverError.log";
        string logfile = AppDomain.CurrentDomain.BaseDirectory + "\\logs\\ConnDriver.log";
        int readinterval = -1;
        /// <summary>
        /// 读取数据的间隔
        /// </summary>
        public int ReadInterval
        {
            get { return readinterval; }
            set { readinterval = value; }
        }


    

        public virtual int SetStatus(ConnDriverStatus status)
        {

            return -1;
        }
        /// <summary>
        /// 获取设置
        /// 根据初始的ID获取设置文件中的host及address设置
        /// 使用获取的设置以及驱动类型进行相应的设置解析
        /// 传入driver中的设置以此为基础
        /// </summary>
        /// <returns></returns>
        public int GetSettings()
        {
            return 1;
        }
        /// <summary>
        /// 建立变量列表
        /// 根据获取的设置来建立
        /// 添加第三方标识列
        /// </summary>
        /// <returns></returns>
        public virtual int EstableItemList()
        {
            return -1;
        }
        /// <summary>
        /// 连接到驱动
        /// </summary>
        /// <returns>1-成功，负数表示失败</returns>
        public virtual int ConnectToDriver()
        {
            return -1;
        }
        /// <summary>
        /// 读取数据，由于是dynamic类型，所以返回object，由调用程序负责根据valuetype进行转换
        /// </summary>
        /// <returns></returns>
        public virtual object ReadData()
        {
            return null;
        }
        public virtual object ReadData(int itemid)
        {
            return null;
        }
        /// <summary>
        /// 初始化
        /// 获取设置、传入驱动、测试驱动、建立连接
        /// </summary>
        /// <returns></returns>
        public virtual int Init()
        {
            //加载驱动
            m_driver = (IDriverBase)m_driverclass.Assembly.CreateInstance(m_driverclass.ToString()); 
            //m_driver.AcceptSetting()
            return -1;
        }
        /// <summary>
        /// 开始运行连接器，即开始向驱动索要数据
        /// </summary>
        /// <returns></returns>
        public virtual int Start()
        {

            return -1;
        }
        /// <summary>
        /// 暂停驱动读数
        /// </summary>
        /// <returns></returns>
        public virtual int Pause()
        {
            return -1;
        }
        /// <summary>
        /// 停止驱动取数，主要是为了变更变量列表
        /// </summary>
        /// <returns></returns>
        public virtual int Stop()
        {
            return -1;
        }
        /// <summary>
        /// 关闭驱动，所有的设置均可进行（主机、变量列表）
        /// </summary>
        /// <returns></returns>
        public virtual int Close()
        {
            return -1;
        }
        /// <summary>
        /// 这个功能待考虑，应该可以不用，也许就是不管状态，只管是否可以运行
        /// </summary>
        /// <returns></returns>
        public virtual int Run()
        {
            return -1;
        }
    }
}
