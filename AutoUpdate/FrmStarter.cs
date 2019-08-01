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
    public partial class FrmStarter : Form
    {
        public FrmStarter(IAutoUpdateClient client)
        {
            m_client = client;
            InitializeComponent();
        }
        IAutoUpdateClient m_client;

        void toEditServer()
        {
            FrmServerSettingsView frm = new FrmServerSettingsView();
            frm.ShowDialog();
        }
        void toEditClientSetting()
        {
            FrmUpdateSettings frm = new FrmUpdateSettings();
            frm.ShowDialog();
        }
        void toUploadFiles()
        {
            FrmUpload frm = new FrmUpload(m_client);
            frm.ShowDialog();
        }
        void toDownloadFile()
        {
            FrmUpdate frm = new FrmUpdate(m_client);
            frm.ShowDialog();
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            toEditServer();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            toEditClientSetting();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            toUploadFiles();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            toDownloadFile();
        }
    }
}
