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
    public partial class FrmMain : Form
    {
        public FrmMain(int runningConnectorId)
        {
            InitializeComponent();
            m_rcId = runningConnectorId;
        }

        ConnectorRunTime m_runtime;
        public ConnectorRunTime RunTimeHost
        {
            get { return m_runtime; }
            set { m_runtime = value; }
        }
        ConnectorBase.ConnectorBase m_connector;
        int m_rcId = -1;
        public int Init()
        {
            int ret = 0;
            m_connector = m_runtime.ConnectorBaseRunning;
            ret = m_connector.Init();
            //在列表中加载
            foreach(ConnDriverBase cd in m_connector.ConnDrivers)
            {
                ListViewItem item = new ListViewItem(
                    new string[]
                    {
                        cd.ID.ToString(),
                        cd.ConnDriverSet.Name,
                        cd.ConnDriverSet.DriverSet.Host
                        ,cd.Status.ToString()
                    }
                );
                item.Tag = cd;
                listView1.Items.Add(item);
            }
            return ret;
        }

        int startSingleConnDriver()
        {
            int ret = -1;
            if (listView1.SelectedItems != null && listView1.SelectedItems.Count > 0)
            {
                ConnDriverBase cd = (ConnDriverBase)listView1.SelectedItems[0].Tag;
                if (cd.Status != ConnDriverStatus.Running)
                {
                    m_connector.RunConnDriver(cd);
                    listView1.SelectedItems[0].SubItems[3].Text = cd.Status.ToString();
                }
            }
            else
            {
                MyMessageBox.ShowTipMessage("请先选择一行！");
            }
            ret = 1;
            return ret;
        }
        int closeSingleConnDriver()
        {
            int ret = -1;

            return ret;
        }

        void viewData()
        {
            if (listView1.SelectedItems != null && listView1.SelectedItems.Count > 0)
            {
                List<ConnDriverBase> cdblist = new List<ConnDriverBase>();
                foreach(ListViewItem item in listView1.SelectedItems)
                {
                    cdblist.Add((ConnDriverBase)item.Tag);
                }
                FrmViewValue frm = new FrmViewValue();
                frm.ViewData(m_connector, cdblist);
            }

        }



        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            startSingleConnDriver();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            viewData();
        }
    }
}
