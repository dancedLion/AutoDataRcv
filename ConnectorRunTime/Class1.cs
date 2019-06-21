using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CHQ.RD.DataContract;
using CHQ.RD.ConnectorBase;
using GeneralOPs;
namespace CHQ.RD.ConnectorRunTime
{
    public class ConnectorRunTime
    {
        public ConnectorRunTime()
        {
            init();
            showForm();
        }


        #region variables and properties
        int m_rcId = -1;
        public int RunningConnector
        {
            get { return m_rcId; }
            set { m_rcId = value; }
        }
        FrmMain m_frm;
        /// <summary>
        /// 运行中的连接管理器
        /// </summary>
        ConnectorBase.ConnectorBase m_cb;
        public ConnectorBase.ConnectorBase ConnectorBaseRunning
        {
            get { return m_cb; }
            set { m_cb = value; }
        }
        /// <summary>
        /// 接收发送数据的主机
        /// </summary>
        List<DataSendingSet> m_sendings;
        public List<DataSendingSet> DataSendingHosts
        {
            get { return m_sendings; }
        }
        #endregion

        void init()
        {
            m_rcId = Ops.getCurrentConnector();
            m_sendings = new List<DataSendingSet>();
            m_cb = new ConnectorBase.ConnectorBase(m_rcId);
            //加载connector设置
            //加载Host 设置
            //加载发送数据设置
            m_sendings = Ops.getDataSendingList(m_rcId);
            
        }

        void showForm()
        {
            FrmMain frm = new FrmMain(m_rcId);
            frm.RunTimeHost = this;
            frm.Init();
            frm.ShowDialog();
        }

    }
}
