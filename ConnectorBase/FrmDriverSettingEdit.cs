using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
namespace CHQ.RD.ConnectorBase
{
    public partial class FrmDriverSettingEdit : Form
    {
        public FrmDriverSettingEdit()
        {
            InitializeComponent();
            initTable();
        }
        DataTable dt;
        int m_result = -1;
        string m_returnedvalue = "";
        Type m_type = null;
        public string ReturnedValue
        {
            get { return m_returnedvalue; }
        }
        void initTable()
        {
            dt = new DataTable("dt");
            dt.Columns.Add("propName", typeof(string));
            dt.Columns.Add("propValue", typeof(string));
            dt.PrimaryKey =new DataColumn[] { dt.Columns[0] };
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = dt;
        }
        public int EditDriverSetting(Type type)
        {
            return EditDriverSetting(type, "");
        }
        public int EditDriverSetting(Type type,string settings)
        {
            m_type = type;
            parsingHost(settings);
            this.ShowDialog();
            return m_result;
        }

        void parsingHost(string settings)
        {
            //根据host类型来获取属性或者field值
            if (m_type == null)
            {
                MyMessageBox.ShowErrorMessage("类型设置不正确！");
                return;
            }
            FieldInfo[] flds = m_type.GetFields();
            for(int i = 0; i < flds.Length; i++)
            {
                DataRow dr = dt.NewRow();
                dr["propName"] = flds[i].Name;
                dt.Rows.Add(dr);
            }
            //根据相应的列表来获取setting中的相关设置
            if (string.IsNullOrEmpty(settings)) { return; }
            //ip=value1;port=value2;格式
            string[] valus = settings.Split(';');
            for(int i = 0; i < valus.Length; i++)
            {
                string[] dict = valus[i].Split('=');
                if (dict.Length > 1)
                {
                    dt.Rows.Find(dict[0])["propValue"] = dict[1];
                }
            }
        }

        void toOK()
        {
            try
            {
                foreach(DataRow dr in dt.Rows)
                {
                    if (string.IsNullOrEmpty(dr[1].ToString())){
                        if (MyMessageBox.ShowSelectionMessage("属性有空值 ，是否继续？ ") == DialogResult.No) return;
                    }
                }
                m_returnedvalue = "";
                foreach(DataRow dr in dt.Rows)
                {
                    if (!string.IsNullOrEmpty(m_returnedvalue)) m_returnedvalue += ";";
                    m_returnedvalue += dr[0].ToString() + "=" + dr[1].ToString();
                }
                m_result = 0;
                this.DialogResult = DialogResult.OK;
            }
            catch(Exception ex)
            {
                MyMessageBox.ShowErrorMessage(ex.Message);
            }
        }
        void toCancel()
        {
            this.DialogResult = DialogResult.Cancel;
        }

        void toViewHostType()
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            toOK();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            toCancel();
        }
    }
}
