using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CHQ.RD.ConnDriverBase;
namespace CHQ.RD.ConnectorBase
{
    public partial class FrmInputBox : Form
    {
        public FrmInputBox()
        {
            InitializeComponent();
        }
        int m_result = -1;
        string m_returnedvalue;
        public string ReturnedValue
        {
            get { return m_returnedvalue; }
        }

        public int GetInput(string tips)
        {
            return GetInput(tips, "");
        }
        public int GetInput(string tips,string defaulttext)
        {
            label1.Text = "请输入" + tips + "：";
            textBox1.Text = defaulttext;
            this.ShowDialog();
            return m_result;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text.Trim()=="")
            {
                MyMessageBox.ShowErrorMessage("未输入值！");
                return;
            }
            m_returnedvalue = textBox1.Text;
            m_result = 0;
            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
