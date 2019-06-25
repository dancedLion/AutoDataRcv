using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.OracleClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeneralOPs;
using CHQ.RD.DataContract;
namespace CHQ.RD.ConnectorBase
{
    public partial class FrmDataStorage : Form
    {
        public FrmDataStorage()
        {
            InitializeComponent();
        }

        ConnectorLocalData m_cld = null;
        int m_result = -1;

        public ConnectorLocalData ReturnedValue
        {
            get { return m_cld; }
        }

        public int EditConnectorLocalData()
        {
            return (EditConnectorLocalData(new ConnectorLocalData
            {
                Id = -1
            }));
        }
        public int EditConnectorLocalData(ConnectorLocalData cld)
        {
            m_cld = cld;
            initForm();
            this.ShowDialog();
            return m_result;
        }

        void initForm()
        {
            tbxId.ReadOnly = m_cld.Id == -1 ? false : true;
            tbxId.Text = m_cld.Id.ToString();
            tbxDesc.Text = m_cld.Desc;
            comboBox1.SelectedIndex = m_cld.RDType;
            comboBox2.SelectedIndex = m_cld.DBDriverType;
            tbxConnectString.Text = m_cld.ConnectString;
        }

        bool formToData()
        {
            bool ret = false;
            try
            {
                int.TryParse(tbxId.Text,out m_cld.Id);
                if (string.IsNullOrEmpty(tbxDesc.Text.Trim()))
                    throw new Exception("未设置描述！");
                if (comboBox1.SelectedIndex < 0)
                {
                    throw new Exception("未设置存储类型");
                }
                if (string.IsNullOrEmpty(tbxConnectString.Text.Trim()))
                    throw new Exception("未设置本地系统数据连接！");
                if (comboBox2.SelectedIndex < 0) throw new Exception("未选择数据库驱动类型");
                if (m_cld.Id < 0) throw new Exception("ID不能使用负数，请更改ID");
                m_cld.Desc = tbxDesc.Text;
                m_cld.RDType = comboBox1.SelectedIndex;
                m_cld.DBDriverType = comboBox2.SelectedIndex;
                m_cld.ConnectString = tbxConnectString.Text;
                ret = testConnectString();
            }
            catch(Exception ex)
            {
                MyMessageBox.ShowErrorMessage(ex.Message);
            }
            return ret;
        }
        /// <summary>
        /// 数据连接测试
        /// </summary>
        /// <returns></returns>
        bool testConnectString()
        {
            bool ret = false ;
            try
            {
                switch (comboBox2.SelectedIndex)
                {
                    case 0:
                        SqlConnection sqlcn = new SqlConnection(tbxConnectString.Text);
                        sqlcn.Open();
                        sqlcn.Close();
                        break;
                    case 1:
                        OracleConnection oracn = new OracleConnection(tbxConnectString.Text);
                        oracn.Open();
                        oracn.Close();
                        break;
                    case 2:
                        OleDbConnection olecn = new OleDbConnection(tbxConnectString.Text);
                        olecn.Open();
                        olecn.Close();
                        break;
                    case 3:
                        OdbcConnection odbccn = new OdbcConnection(tbxConnectString.Text);
                        odbccn.Open();
                        odbccn.Close();
                        break;
                }
                ret = true;
            }
            catch(Exception ex)
            {
                MyMessageBox.ShowErrorMessage(ex.Message);
            }
            return ret;
        }
        void saveResult()
        {
            if (formToData())
            {
                if (Ops.saveConnectorLocalData(m_cld) < 0)
                {
                    MyMessageBox.ShowErrorMessage("保存时出现错误，请查看日志!");
                }
                else
                {
                    m_result = 1;
                    this.DialogResult = DialogResult.OK;
                }
            }
        }

        void cancelResult()
        {
            if (MyMessageBox.ShowSelectionMessage("是否确认放弃所做工作？") == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.OK;
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
