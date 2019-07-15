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
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;    //创建驱动实例用
using GeneralOPs;
using System.Data;
namespace CHQ.RD.ConnectorBase
{
    public class ConnDriverBase:IConnDriverBase
    {
        public ConnDriverBase(int id,ConnectorBase host)
        {
            m_id = id;
            m_host = host;
            m_dataitems = new List<ConnDriverDataItem>();
            GetSettings();
 
        }

        static ManualResetEvent mre = new ManualResetEvent(false);
        #region 局部变量及相关属性设置
        //驱动数据读取时的错误标记
        public const string ErrorString = "ERROR";
        //驱动连接器设置变量
        ConnDriverSetting m_conndriverset;
        //数据项列表
        List<ConnDriverDataItem> m_dataitems;

        //连接管理器宿主
        ConnectorBase m_host;
        //数据发生变化事件
        DataChangeEventHandler m_datachangehandler;
        //数据处理线程
        Timer m_datareader;
        Timer m_errortransact;
        //驱动接口，用以实现发送设置，读取变量
        IDriverBase m_driver;
        //驱动类型，用于初始化驱动
        Type m_driverclass;
        /// <summary>
        /// 驱动连接器状态变量
        /// </summary>
        ConnDriverStatus m_status = ConnDriverStatus.None;

        /// <summary>
        /// 错误日志文件路径
        /// </summary>
        string errorfile = AppDomain.CurrentDomain.BaseDirectory + "logs\\ConnDriverError.log";
        /// <summary>
        /// 驱动连接器运行日志
        /// 用于记录用于调试的信息
        /// </summary>
        string logfile = AppDomain.CurrentDomain.BaseDirectory + "logs\\ConnDriver.log";
        //读取间隔
        int m_readinterval = -1;
        //检测异常时间间隔
        int m_errortransactinterval = 10000;
        //驱动连接器ID
        int m_id = -1;
        int m_readmode = -1;
        int m_transmode = -1;
        /// <summary>
        /// 侦听模式下的线程
        ///
        /// </summary>
        protected Thread m_thread;
        public ConnDriverSetting ConnDriverSet
        {
            get { return m_conndriverset; }
            set { m_conndriverset = value; }
           
        }

        /// <summary>
        /// 驱动连接器ID，初始化时赋值
        /// </summary>
        public int ID
        {
            get { return m_id; }
            set { m_id = value; }
        }
        /// <summary>
        /// 驱动器
        /// </summary>
        public IDriverBase Driver
        {
            get { return m_driver; }
        }
        /// <summary>
        /// 驱动类型
        /// </summary>
        public Type DriverClass
        {
            get { return m_driverclass; }
            set { m_driverclass = value; }
        }
        /// <summary>
        /// 驱动连接器状态
        /// </summary>
        public ConnDriverStatus Status
        {
            get { return m_status; }
            set { SetStatus(value); }
        }
        public List<ConnDriverDataItem> DataItems
        {
            get { return m_dataitems; }
            set { m_dataitems = value; }
        }
        /// <summary>
        /// 读取数据的间隔
        /// </summary>
        public int ReadInterval
        {
            get { return m_readinterval; }
            set { m_readinterval = value; }
        }
        /// <summary>
        /// 很怪异的写法，实现扫口的事件句柄
        /// </summary>
        //public event DataChangeEventHandler DataChange;
        event DataChangeEventHandler IConnDriverBase.DataChange{
            add { m_datachangehandler += value; }
            remove { m_datachangehandler -= value; }
        }

        #endregion

        public ConnDriverBase()
        {

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
        public virtual int GetSettings()
        {
            int ret = 0;
            m_conndriverset = Ops.getConnDriverSetting(m_id);
            if (m_conndriverset == null)
            {
                ret = -1;
                TxtLogWriter.WriteErrorMessage("ConnDriver.GetSettings(" + m_id.ToString() + "):" + "获取设置失败！");
            }
            else
            {
                m_readmode = m_conndriverset.ReadMode;
                m_readinterval = m_conndriverset.ReadInterval;
                m_transmode = m_conndriverset.TransMode;

                //数据行的获取

                foreach(ConnectorDataItem cditem in m_conndriverset.DataItems)
                {
                    m_dataitems.Add(
                        new ConnDriverDataItem
                        {
                            Id = cditem.Id,
                            ValueType = cditem.ValueType,
                            Address = cditem.Address
                        }
                    );
                }
                Assembly asm = Assembly.LoadFile(m_conndriverset.ClassFile.FileName);
                m_driverclass = asm.GetType(m_conndriverset.ClassFile.ClassName);
            }
            return ret;
        }
        /// <summary>
        /// 连接到驱动
        /// </summary>
        /// <returns>0-成功，负数表示失败</returns>
        public virtual int TryDriver()
        {
            int ret = -1;
            using (IDriverBase tmp = (IDriverBase)m_driverclass.Assembly.CreateInstance(m_driverclass.FullName))
            {
                try
                {
                    if (m_driverclass != null)
                    {
                        ret = tmp.AcceptSetting(m_conndriverset.DriverSet.Host, m_dataitems);
                        if (ret != 0)
                        {
                            throw new Exception("尝试连接时错误！");
                        }

                        //20190624 如果不是侦听模式，就读个值试试
                        //if (m_readmode == 0)
                        //{
                        //    object value = tmp.ReadData(m_dataitems[0].Id);
                        //    if (value.ToString() == ErrorString)
                        //    {
                        //        throw new Exception("试读数据时出错！");
                        //    }
                        //}
                    }
                    else
                    {
                        throw new Exception("创建驱动错误！");
                    }
                }
                catch (Exception ex)
                {
                    TxtLogWriter.WriteErrorMessage(errorfile,"ConnDriverBase.TryDriver(" + m_id.ToString() + "):" + ex.Message);
                    ret = -1;
                }
            }
            return ret;
        }
        /// <summary>
        /// 读取数据，由于是dynamic类型，所以返回object，由调用程序负责根据valuetype进行转换
        /// 如果值更新，则发生值 改变事件
        /// </summary>
        /// <returns></returns>
        public virtual void ReadData(object state)
        {
            //侦听模式，不需要读取
            if (m_readmode == 1) return;
            try
            {
                foreach (ConnDriverDataItem item in m_dataitems)
                {
                    object value = m_driver.ReadData(item.Id);

                    object curvalue = m_host.ValueList[item.Id];
                    if (value == null)
                    {
                        if (curvalue != null)
                        {
                            //引起值变化

                            //new Task(runDataChange<new DataChangeEventArgs(item.Id, value)>).Start();
                            //runDataChange(new DataChangeEventArgs(item.Id, value));
                            onDataChanged(this, new DataChangeEventArgs(item.Id, value));
                        }
                    }
                    else
                    {
                        //if (value.ToString() == ErrorString)
                        //{
                        //    continue;
                        //}
                        if (curvalue == null || !object.Equals( curvalue,value))
                        {
                            //引起值变化
                            //runDataChange(new DataChangeEventArgs(item.Id, value));
                            onDataChanged(this, new DataChangeEventArgs(item.Id, value));
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, "ConnDriverBase.ReadData(" + m_id.ToString() + "):" + ex.Message);
            }
        }
        public virtual object ReadData(int itemid)
        {
            object ret = null;
            try
            {
                object value = m_driver.ReadData(itemid);
                if (value.ToString() == ErrorString)
                {

                    throw new Exception("读取数据时发生错误！");
                }
                return value;
            }
            catch(Exception ex)
            {
                ret = null;
            }
            return ret;
        }
        /// <summary>
        /// 初始化
        /// 获取设置、传入驱动、测试驱动、建立连接
        /// </summary>
        /// <returns></returns>
        public virtual int Init()
        {
           
            //加载驱动
            int ret = -1;
            m_driver = (IDriverBase)m_driverclass.Assembly.CreateInstance(m_driverclass.FullName);
            ret=m_driver.AcceptSetting(m_conndriverset.DriverSet.Host, m_dataitems);
            //如果驱动是主动读取且有需求则传
            m_driver.ReadMode = m_conndriverset.DriverSet.ReadMode;
            m_driver.ReadInterval = m_conndriverset.DriverSet.ReadInterval;
            m_driver.TransMode = m_conndriverset.DriverSet.TransMode;
            
            //设置状态
            if (ret != 0)
            {
                TxtLogWriter.WriteErrorMessage("ConnDriverBase.Init(" + m_id.ToString() + "):初始化失败");
                m_status = ConnDriverStatus.Error;
            }
            else
            {
                m_status = ConnDriverStatus.Inited;
            }
            if (m_readmode == 1)
            {

            }
            //m_driver.AcceptSetting()
            return ret;

        }
        /// <summary>
        /// 开始运行连接器，即开始向驱动索要数据
        /// </summary>
        /// <returns></returns>
        public virtual int Start()
        {
            int ret = 0;
            try {
                if (m_status == ConnDriverStatus.Running)
                {
                    return 0;
                }
                if (m_status == ConnDriverStatus.Closed || m_status == ConnDriverStatus.None || m_status == ConnDriverStatus.Error)
                {
                    Init();
                    if (m_status != ConnDriverStatus.Inited)
                    {
                        throw new Exception("尝试初始化失败！");
                    }
                }
                if (m_status!=ConnDriverStatus.Inited&&m_status!=ConnDriverStatus.Stoped)
                {
                    Init();
                    if (m_status != ConnDriverStatus.Inited)
                    {
                        throw new Exception("尝试初始化失败！");
                    }
                }
                if (m_status == ConnDriverStatus.Stoped || m_status == ConnDriverStatus.Inited)
                {
                    if (m_readmode == 0)
                    {
                        //如果驱动的读取模式为主动，则驱动开始读取模式
                        ret=m_driver.Start();
                        m_datareader = new Timer(ReadData, null, m_readinterval, m_readinterval);
                        m_errortransact = new Timer(ErrorTransact, null, m_errortransactinterval, m_errortransactinterval);
                    }
                    else
                    {
                        //驱动开启侦听
                        m_driver.Start();
                        //驱动连接器启动
                        m_thread = new Thread(AcceptValue);
                        m_thread.Start();
                        mre.Set();
                    }
                    m_status = ConnDriverStatus.Running;
                }
            }
            catch(Exception ex)
            {
                ret = -1;
                TxtLogWriter.WriteErrorMessage(this.GetType().ToString() + ":Start Error:" + ex.Message);
                m_status = ConnDriverStatus.Error;
            }
            return ret; 
        }
        /// <summary>
        /// 停止驱动取数，主要是为了变更变量列表
        /// </summary>
        /// <returns></returns>
        public virtual int Stop()
        {
            int ret = 0;
            try
            {
                if (m_status != ConnDriverStatus.Running)
                {
                    throw new Exception("当前状态为："+m_status.ToString()+",无法执行停止操作！");
                }
                if (m_readmode == 0)
                {
                    //如果驱动的读取模式为主动，则驱动停止读取模式
                    m_driver.Stop();
                    m_datareader.Dispose();
                }
                else
                {
                    m_driver.Stop();
                    mre.Reset();
                }
                m_status = ConnDriverStatus.Stoped;
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(this.GetType().ToString() + ":Stop Error:" + ex.Message);
                ret = -1;
            }
            return ret;
        }
        /// <summary>
        /// 关闭驱动，所有的设置均可进行（主机、变量列表）
        /// </summary>
        /// <returns></returns>
        public virtual int Close()
        {
            int ret = 0;
            try
            {
                if (m_status == ConnDriverStatus.Running)
                {
                    if (Stop() < 0)
                    {
                        throw new Exception("关闭时出错！");
                    }
                }
                if (m_status == ConnDriverStatus.Error)
                {
                    throw new Exception("当前状态为Error，无需执行操作");
                }
                if (m_readmode == 0)
                {
                    m_driver.Dispose();
                    m_datareader = null;
                    //m_driverset = null;
                    //m_driverclass = null;
                }
                else
                {
                    mre.Reset();
                    m_thread.Abort();
                }
                m_status = ConnDriverStatus.Closed;
            }
            catch(Exception ex)
            {
                ret = -1;
                TxtLogWriter.WriteErrorMessage(this.GetType().ToString() + ":Close Error:" + ex.Message);
            }
            return ret;
        }
        /// <summary>
        /// 重新初始化并启动
        /// stop close init start
        /// </summary>
        /// <returns>0-成功</returns>
        public virtual int Restart()
        {
            int ret = 0;
            try {
                if (m_status == ConnDriverStatus.Running)
                {
                    Stop();
                }
                if (m_status == ConnDriverStatus.Stoped)
                {
                    Close();
                }
                if (Init() < 0)
                {
                    throw new Exception("初始化失败");
                }
                if (Start() < 0)
                {
                    throw new Exception("启动失败！");
                }
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(this.GetType().ToString() + ":Restart Error:" + ex.Message);
                ret = -1;
            }
            return ret;
        }

        /// <summary>
        /// 错误处理
        /// 当某个错误号达到10次以上时，将进行重连
        /// </summary>
        /// <param name="state"></param>
        public virtual void ErrorTransact(object state)
        {
            try
            {
                foreach (KeyValuePair<int, int> m in Driver.ErrorCount)
                {
                    if (m.Value >= 10)
                    {
                        //置状态
                        m_status = ConnDriverStatus.AutoErrorTransacting;
                        //
                        if (m_readmode == 0)
                        {
                            //如果驱动在运行中，则需要停止
                            m_driver.Stop();
                            if (m_datareader != null)
                            {
                                m_datareader.Dispose();
                                m_datareader = null;
                            }
                            //准备重新启动并初始化驱动
                            //TODO:停止errortransact，在timer启动的进程中杀掉timer，不知道会不会可行
                            m_errortransact.Dispose();
                            m_errortransact = null;
                            bool m_issuccess = false;
                            while (!m_issuccess)
                            {
                                if (TryDriver() == 0)
                                {
                                    if (Restart() == 0)
                                    {
                                        Driver.ErrorCount[m.Key] = 0;
                                        m_issuccess = true;
                                    }

                                }
                                Thread.Sleep(m_errortransactinterval);
                            }
                            //重新启动
                            m_errortransact = new Timer(ErrorTransact, null, m_errortransactinterval, m_errortransactinterval);
                        }
                        else
                        {
                            //TODO:主动侦听模式下，就没有写ERRORCOUNT，所以暂时无须处理
                        }
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, "ConnDriverBase.ErrorTransact(" + m_id.ToString() + "):"+ex.Message);
            }
        }

        #region Listener Mode code
        /// <summary>
        /// 当驱动为主动模式下，需要使用该进程来接收传入的数据并处理
        /// 读取序列中的所有变量，并写入到Connector的值列表中
        /// 读取并写入一个则需要清除一个
        /// </summary>
        /// <returns></returns>
        public virtual  void AcceptValue()
        {
            while (true)
            {
                mre.WaitOne();
                if (((Queue<ListKeyValue>)m_driver.ValueList).Count > 0)
                {
                    lock (m_driver.ValueList)
                    {
                        ListKeyValue t = ((Queue<ListKeyValue>)m_driver.ValueList).Dequeue();
                        object curvalue = m_host.ValueList[t.Id];
                        //侦听模式下，收到数据就要体现
                        onDataChanged(this, new DataChangeEventArgs(t.Id, t.Value));
                    }
                    //if(curvalue==null&&t.Value!=null || t.Value == null && curvalue != null)
                    //{
                    //    onDataChanged(this, new DataChangeEventArgs(t.Id, t.Value));
                    //}
                    //else
                    //{
                    //    if (!object.Equals(curvalue, t.Value))
                    //    {
                    //        onDataChanged(this, new DataChangeEventArgs(t.Id, t.Value));
                    //    }
                    //}
                }
            }
        }
        #endregion

        public void Dispose()
        {
            if (m_status == ConnDriverStatus.Running)
            {
                Stop();
            }
            if (m_status == ConnDriverStatus.Stoped||m_status==ConnDriverStatus.Inited)
            {
                Close();
            }
            m_thread = null;
            m_dataitems = null;
            m_driverclass = null;
            m_driver.Dispose();
            m_conndriverset = null;
        }


        public virtual void WriteErrorMessage(string message)
        {
            TxtLogWriter.WriteErrorMessage(errorfile, this.GetType().FullName + message);
        }


        #region 内部事件和方法
        public virtual void onDataChanged(object sender,DataChangeEventArgs e)
        {
            //写值
            m_host.ValueList[e.ItemId] = e.Value;
            m_host.SendData(m_conndriverset.DataItems.Find(x => x.Id == e.ItemId), e.Value);
            //触发handler
            if(m_datachangehandler!=null)
            {
                m_datachangehandler(sender, e);
            }
        }
        //public async Task runDataChange(DataChangeEventArgs e)
        //{
        //    (new Task(() => onDataChanged(this, e))).Start();
        //    //onDataChanged(this, e));
        //}
        #endregion

    }
}
