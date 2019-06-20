using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CHQ.RD.ConnectorBase;
using CHQ.RD.ConnectorRunTime;
namespace Console
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FrmConnectorBase frm = new FrmConnectorBase();
            frm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FrmMain frm = new FrmMain();
            frm.ShowDialog();
        }
    }
}
