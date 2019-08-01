using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace AutoUpdate
{
    public partial class FrmUpload : Form
    {
        public FrmUpload(IAutoUpdateClient client)
        {
            m_client = client;
            InitializeComponent();
        }
        IAutoUpdateClient m_client;
        void toAddnewFile()
        {
            FrmAddUpdateFile frm = new FrmAddUpdateFile();
            if (frm.AddnewFile() == 0)
            {
                ListViewItem item = new ListViewItem
                (
                    new string[]
                    {
                        frm.SelectedFileInfo.FileId.ToString(),
                        frm.SelectedFile,
                        frm.SelectedFileInfo.FileVersion,
                        ""
                    }
                );
                item.Tag = frm.SelectedFileInfo;
                listView1.Items.Add(item);
            }
        }
        void toEditFile()
        {
            if (listView1.SelectedItems == null || listView1.SelectedItems.Count == 0) return;
            FrmAddUpdateFile frm = new FrmAddUpdateFile();
            if (frm.EditFile(listView1.SelectedItems[0].SubItems[1].Text, (AUFileInfo)listView1.SelectedItems[0].Tag) == 0)
            {
                listView1.SelectedItems[0].Tag = frm.SelectedFileInfo;
                listView1.SelectedItems[0].Text = frm.SelectedFileInfo.FileId.ToString();
                listView1.SelectedItems[0].SubItems[2].Text = frm.SelectedFileInfo.FileVersion;
                listView1.SelectedItems[0].SubItems[1].Text = frm.SelectedFile;
            }
        }
        void toRemoveFile()
        {
            if (listView1.SelectedItems == null || listView1.SelectedItems.Count == 0) return;
            listView1.SelectedItems[0].Remove();
        }

        void toUpload()
        {
            foreach(ListViewItem item in listView1.Items)
            {
                try
                {
                    FileStream fs = new FileStream(item.SubItems[1].Text, FileMode.Open, FileAccess.Read);
                    int numsToRead = 0;
                    int numsReading =(int)fs.Length;
                    byte[] buff = new byte[fs.Length];
                    item.SubItems[3].Text = "Prepare";
                    while (true)
                    {
                        int n = fs.Read(buff, numsToRead, numsReading);
                        if (n == 0)
                        {
                            break;
                        }
                        numsToRead += n;
                        numsReading -= n;
                    }
                    fs.Close();
                    int i=m_client.AUServer.SetServerFile((AUFileInfo)item.Tag, buff);
                    if (i == 0)
                    {
                        item.SubItems[3].Text = "OK";
                    }
                }
                catch(Exception ex)
                {
                    item.SubItems[3].Text = ex.Message;
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            toUpload();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            toAddnewFile();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            toEditFile();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            toRemoveFile();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
