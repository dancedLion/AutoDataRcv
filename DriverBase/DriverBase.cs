using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Threading;
using CHQ.RD.DataContract;
using GeneralOPs;
namespace CHQ.RD.DriverBase
{
    public class DriverBase:IDriverBase
    {
        #region 变量和属性
        protected Thread m_thread;

        protected object m_host;
        protected List<ConnDriverDataItem> m_datalist;

        int m_transmode=0;
        int m_readmode=0;
        int m_readinterval=2000;
        int m_debugmode = -1;

        string errorfile = AppDomain.CurrentDomain.BaseDirectory + "logs\\DriverBaseError.log";
        string logfile = AppDomain.CurrentDomain.BaseDirectory + "logs\\DriverBase.log";
        public string ErrorFile
        {
            get { return errorfile; }
        }
        public string LogFile
        {
            get { return logfile; }
        }
        Type m_hosttype = typeof(S7TCPHost);
        Type m_addresstype = typeof(S7Address);
        Dictionary<int, int> m_errorCount;
        protected Timer m_datareader;
        protected Timer m_errortransact;
        DriverStatus m_status = DriverStatus.None;
        

        public virtual object ValueList
        {
            get;set;
        }

        public int DebugMode
        {
            get { return m_debugmode; }
            set { m_debugmode = value; }
        }
        public int TransMode
        {
            get { return m_transmode; }
            set { m_transmode = value; }
        }
        public int ReadMode
        {
            get { return m_readmode; }
            set { m_readmode = value; }
        }
        public int ReadInterval
        {
            get { return m_readinterval; }
            set { m_readinterval = value; }
        }
        public DriverStatus Status
        {
            get { return m_status; }
            set { m_status = value; }
        }
        public Type HostType
        {
            get { return m_hosttype; }
            set { m_hosttype = value; }
        }
        public Type AddressType
        {
            get { return m_addresstype; }
            set { m_addresstype = value; }
        }
        public Dictionary<int,int> ErrorCount
        {
            get { return m_errorCount; }
            set { m_errorCount = value; }
        }
        #endregion
        public DriverBase()
        {
            m_errorCount = new Dictionary<int, int>();
        }


        #region Operations
        public virtual int SetStatus(DriverStatus status)
        {
            int ret = -1;
            /*状态设置规则 
             * 驱动的状态应该只有几种
             * 未初始-None
             * 已初始-Inited
             * 错误中-Error
             * 主动读取的驱动--Running,Stoped
             * 
             */
            try
            {
                switch (Status)
                {
                    //未初始状态下，只能置于错误
                    case DriverStatus.None:
                        Status = status;
                        break;
                    case DriverStatus.Inited:
                        if (status == DriverStatus.Running || status == DriverStatus.Stoped)
                        {
                            if (ReadMode != 1)
                            {
                                throw new Exception("非主动读取模式中，不能设置运行或停止！");
                            }
                            else
                            {
                                //如果是运行，则设置开始运行，否则设置为stoped
                                m_status = status;
                            }
                        }
                        else
                        {
                            m_status = status;
                        }
                        break;
                    case DriverStatus.Error:
                        //根据状态设置其运行
                        break;
                    case DriverStatus.Running:
                        //错误、停止
                        break;
                    case DriverStatus.Stoped:
                        //启动、错误
                        break;
                }
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, "DriverBase.SetStatus(" + status.ToString() + "):" + ex.Message);
            }
            return ret;
        }
        public virtual int Init()
        {
            int ret = -1;
            //m_datalist = new List<ConnDriverDataItem>();
            try
            {
                ret = AcceptSetting(m_host, m_datalist);
                if (ret != 0)
                {
                    throw new Exception("加载设置应用失败！");
                }
                m_status = DriverStatus.Inited;
                if (m_readmode == 1)
                {
                    //TODO:如果是主动读取，则初始化reader
                    
                }
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, "DriverBase.Init(" + m_host.ToString() + "):" + ex.Message);
                SetStatus(DriverStatus.Error);
            }
            return ret;
        }
        public virtual int Start()
        {
            int ret = -1;
            try
            {
                if (m_readmode != 1)
                {
                    throw new Exception("非主动读取模式不能启动读取！");
                }
                else
                {
                    if (m_status == DriverStatus.Stoped || m_status == DriverStatus.Inited)
                    {
                        //TODO:启动自动读取

                    }
                    else
                    {
                        throw new Exception(m_status.ToString()+"=>"+DriverStatus.Running.ToString()+" 错误，不支持的状态");
                    }
                }
                m_status = DriverStatus.Running;
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, "DriverBase.Start(" + m_host.ToString() + "):" + ex.Message);
            }
            return ret;
        }
        public virtual int Stop()
        {
            int ret = -1;
            try
            {
                if (m_readmode != 1)
                {
                    throw new Exception("非主动读取模式下不需要停止读取！");
                }
                else
                {
                    if (m_status != DriverStatus.Running)
                    {
                        throw new Exception(m_status.ToString() + "=>" + DriverStatus.Running.ToString() + " 错误，不支持的状态");
                    }
                    else
                    {
                        //TODO:停止读取
                    }
                }
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, "DriverBase.Stop(" + m_host.ToString() + "):" + ex.Message);
            }
            return ret;
        }
        public virtual int Restart()
        {
            int ret = -1;
            return ret;
        }
        #endregion

        public virtual void Dispose()
        {

        }
        public virtual int AcceptSetting(object host,object list)
        {
            int ret = 0;
            return ret;
        }

        public virtual int TryConnectToDevice()
        {
            int ret = 0;
            return ret;
        }
        public virtual object ReadData(int ItemId)
        {
            object ret = 0;
            return ret;
        }

        public virtual object ReadDeviceData(object Item)
        {
            object ret = 0;
            return ret;
        }

        public virtual void StartListener()
        {

        }

        public virtual int SendData(object value)
        {
            int ret = 0;
            return ret;
        }
        public virtual object ParsingHost(string host)
        {
            object ret = null;
            try
            {
                if (m_hosttype != null)
                {
                    ret = m_hosttype.Assembly.CreateInstance(m_hosttype.FullName);
                    FieldInfo[] flds = m_hosttype.GetFields();
                    string[] rows = host.Split(';');
                    for (int i = 0; i < rows.Length; i++)
                    {
                        string[] kvp = rows[i].Split('=');
                        for (int j = 0; j < flds.Length; j++)
                        {
                            if (kvp[0].ToUpper() == flds[j].Name.ToUpper())
                            {
                                if (flds[j].FieldType.IsEnum)
                                {
                                    flds[j].SetValue(ret, Convert.ChangeType(int.Parse(kvp[1]), flds[j].FieldType));
                                }
                                else
                                {
                                    flds[j].SetValue(ret, Convert.ChangeType(kvp[1], flds[j].FieldType));
                                }
                                break;  //退出当前并继续下一个循环
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, this.GetType().ToString() + ".ParsingAddress(" + host + ") Error:" + ex.Message);
                ret = null;
            }
            return ret;
        }
        public virtual object ParsingAddress(string address)
        {
            object ret = null;
            try
            {
                if (m_addresstype == null)
                {
                    return null;
                }
                ret = m_addresstype.Assembly.CreateInstance(m_addresstype.FullName);
                FieldInfo[] flds = m_addresstype.GetFields();
                string[] rows = address.Split(';');
                for(int i = 0; i < rows.Length; i++)
                {
                    string[] kvp = rows[i].Split('=');
                    for(int j = 0; j < flds.Length; j++)
                    {
                        if (flds[j].FieldType.IsEnum)
                        {
                            flds[j].SetValue(ret, Convert.ChangeType(int.Parse(kvp[1]), flds[j].FieldType));
                        }
                        else
                        {
                            flds[j].SetValue(ret, Convert.ChangeType(kvp[1], flds[j].FieldType));
                        }
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, this.GetType().ToString() + ".ParsingAddress(" + address + ") Error:" + ex.Message);
                ret = null;
            }
            return ret;
        }
    }
}
