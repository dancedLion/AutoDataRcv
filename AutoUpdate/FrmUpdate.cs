using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AutoUpdate
{
    public partial class FrmUpdate : Form
    {
        public FrmUpdate(IAutoUpdateClient client)
        {
            m_client = client;
            InitializeComponent();
            refItems();
        }
        IAutoUpdateClient m_client;

        void refItems()
        {
            listView1.Items.Clear();
            List<AUFileInfo> t = m_client.GetDownloadList();
            foreach(AUFileInfo f in t)
            {
                ListViewItem item = new ListViewItem(
                    new string[]
                    {
                        f.FileId,f.FileName,f.FileVersion,f.FileSize.ToString(),""
                    });
                item.Tag = f;
                listView1.Items.Add(item);
            }
        }
        void downSelectedLoadFile()
        {
            if (listView1.SelectedItems == null || listView1.SelectedItems.Count == 0) return;
            foreach(ListViewItem item in listView1.SelectedItems)
            {
                downLoadFile(item);
            }
        }
        void downLoadAllFile()
        {
            foreach(ListViewItem item in listView1.Items)
            {
                downLoadFile(item);
            }
        }
        void downLoadFile(ListViewItem item)
        {
            if (m_client.DownloadFile((AUFileInfo)item.Tag) == 0)
            {
                item.SubItems[4].Text = "OK";
            }
            else
            {
                item.SubItems[4].Text = "Error";
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            downLoadAllFile();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            downSelectedLoadFile();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            refItems();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
