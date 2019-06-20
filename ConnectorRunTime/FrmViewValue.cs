using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CHQ.RD.DataContract;
using CHQ.RD.ConnectorBase;
namespace CHQ.RD.ConnectorRunTime
{
    public partial class FrmViewValue : Form
    {
        public FrmViewValue()
        {
            InitializeComponent();
        }
        ConnectorBase.ConnectorBase m_host;
        DataTable varList;
        public void ViewData(ConnectorBase.ConnectorBase host, List<ConnDriverBase> conndrivers)
        {
            if (conndrivers == null || conndrivers.Count == 0)
            {
                MyMessageBox.ShowErrorMessage("未传入需要查看的驱动连接器！");
                return;
            }
            varList = new DataTable("varlist");
            varList.Columns.Add("Id", typeof(int));
            //varList.Columns.Add("Name", typeof(string));
            varList.Columns.Add("Value", typeof(object));
            varList.Columns.Add("ValueType", typeof(string));
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.DataSource = varList;
            m_host = host;
            //写入到表中
            foreach (ConnDriverBase cd in conndrivers)
            {
                foreach (ConnDriverDataItem item in cd.DataItems)
                {
                    DataRow dr = varList.NewRow();
                    dr["Id"] = item.Id;
                    dr["ValueType"] = item.ValueType;
                    varList.Rows.Add(dr);
                }
            }
            //启动Timer
            timer1.Start();
            //启动FORM
            this.ShowDialog();
        }
        void readValue()
        {
            foreach(DataRow dr in varList.Rows)
            {
                dr["Value"] = m_host.ValueList[(int)dr["Id"]];
            }
        }
        private void FrmViewValue_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            readValue();
        }
    }
}
