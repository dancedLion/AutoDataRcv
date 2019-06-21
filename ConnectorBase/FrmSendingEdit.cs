using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CHQ.RD.DataContract;
using GeneralOPs;
namespace CHQ.RD.ConnectorBase
{
    public partial class FrmSendingEdit : Form
    {
        public FrmSendingEdit()
        {
            InitializeComponent();

        }
        DataSendingSet m_set;
        int m_connectorId = -1;
        int m_result = -1;
        public DataSendingSet ReturnedValue
        {
            get { return m_set; }
        }
        public int EditDataSendingSet(int connectorId)
        {
            return EditDataSendingSet(connectorId, new DataSendingSet
            {
                Id = -1
            });

        }
        public int EditDataSendingSet(int connectorId,DataSendingSet dss)
        {
            m_set = dss;
            m_connectorId = connectorId;
            initForm();
            this.ShowDialog();
            return m_result;
        }
        //初始化
        void initForm()
        {
            tbxid.Text = m_set.Id.ToString();
            tbxid.ReadOnly = m_set.Id == -1 ? false : true;
            tbxName.Text = m_set.Name;
            tbxHost.Text = m_set.Host;
            tbxHostPort.Text = m_set.HostPort.ToString();
            tbxConnDrivers.Text = m_set.ConnDrivers;
            tbxMemo.Text = m_set.Memo;
            tbxSendInterval.Text = m_set.SendInterval.ToString();
            cbxVia.SelectedIndex = m_set.Via;
        }
        //选择驱动连接器
        void selectConnDrivers()
        {
            //TODO:选择驱动连接器，要求是清除后添加还是直接添加
        }
        /// <summary>
        /// 将表单元素写回到设置
        /// </summary>
        /// <returns></returns>
        bool formToDataSendingSet()
        {
            bool ret = false;
            try
            {
                int.TryParse(tbxid.Text,out m_set.Id);
                int.TryParse(tbxSendInterval.Text, out m_set.SendInterval);
                int.TryParse(tbxHostPort.Text, out m_set.HostPort);
                if (string.IsNullOrEmpty(tbxName.Text.Trim())) throw new Exception("未设定名称！");
                if (string.IsNullOrEmpty(tbxHost.Text.Trim())) throw new Exception("未设定接收主机！");
                //TODO:接收主机基于网络，需要解析IP
                if (cbxVia.SelectedIndex == -1) throw new Exception("未设定传输所用的协议（方式)");
                m_set.Name = tbxName.Text;
                m_set.Host = tbxHost.Text;
                m_set.Via = cbxVia.SelectedIndex;
                m_set.Memo = tbxMemo.Text;
                m_set.ConnDrivers = tbxConnDrivers.Text;
                ret = true;
            }
            catch(Exception ex)
            {
                MyMessageBox.ShowErrorMessage(ex.Message);
            }
            return ret;
        }
        /// <summary>
        /// 保存结果 
        /// </summary>
        void saveResult()
        {
            if (formToDataSendingSet())
            {
                try
                {
                    if (Ops.saveDataSending(m_connectorId, m_set) >= 0)
                    {
                        m_result = 1;
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        throw new Exception("保存失败，请查看日志！");
                    }
                }
                catch(Exception ex)
                {
                    MyMessageBox.ShowErrorMessage(ex.Message);
                }
            }
        }
        /// <summary>
        /// 放弃结果 
        /// </summary>
        void cancelResult()
        {
            if (MyMessageBox.ShowSelectionMessage("是否确认不保存结果？") == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            saveResult();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            cancelResult();
        }
    }
}
