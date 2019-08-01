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
    public partial class FrmServerSettingEdit : Form
    {
        public FrmServerSettingEdit()
        {
            m_dt.Columns.Add("KeyCol", typeof(string));
            m_dt.Columns.Add("ValueCol", typeof(string));
            InitializeComponent();
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = m_dt;

        }

        private int m_result = -1;
        private Dictionary<string, string> m_setting = new Dictionary<string, string>();
        private DataTable m_dt = new DataTable("dt");
        public Dictionary<string,string> ReturnedValue
        {
            get { return m_setting; }
        }

        public int AddnewSetting()
        {
            //获取最大ID
            List<Dictionary<string, string>> t = xmloper.getAUSSetingsList();
            m_setting.Add("ServerId", "1");
            if (t.Count == 0)
            {
                
            }
            else
            {
                foreach(Dictionary<string,string> l in t)
                {
                    if (int.Parse(l["ServerId"]) > int.Parse(m_setting["ServerId"]))
                    {
                        m_setting["ServerId"] = (int.Parse(l["ServerId"]) + 1).ToString();
                    }
                }
            }
            textBox1.ReadOnly = false;
            return EditSetting(m_setting);
        }
        public int EditSetting(Dictionary<string,string> server)
        {
            m_setting = server;
            settingToForm();
            this.ShowDialog();
            return m_result;
        }
        #region local method
        void settingToForm()
        {
            m_dt.Rows.Clear();
            textBox1.Text = m_setting["ServerId"];
            textBox2.Text = m_setting["Type"];
            foreach(KeyValuePair<string,string> kv in m_setting)
            {
                if (kv.Key == "ServerId" || kv.Key == "Type")
                {
                    continue;
                }
                else
                {
                    DataRow dr = m_dt.NewRow();
                    dr[0] = kv.Key;
                    dr[1] = kv.Value;
                    m_dt.Rows.Add(dr);
                }
            }
            m_dt.AcceptChanges();
        }

        bool formToSetting()
        {
            bool ret = false;
            try
            {
                int.Parse(textBox1.Text);
                textBox2.Text= Type.GetType(textBox2.Text).FullName;
                //先清除，后添加
                m_setting.Clear();
                m_setting.Add("ServerId", textBox1.Text);
                m_setting.Add("Type", textBox2.Text);
                m_dt.AcceptChanges();
                foreach(DataRow dr in m_dt.Rows)
                {
                    m_setting.Add(dr[0].ToString(), dr[1].ToString());
                }
                ret = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return ret;
        }
        void toSave()
        {
            if (formToSetting())
            {
                try
                {
                    if (xmloper.saveAUSSetting(m_setting) > -1)
                    {
                        m_result = 0;
                        this.DialogResult = DialogResult.OK;
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        void toExit()
        {
            this.DialogResult = DialogResult.Cancel;
        }
        void toSelectServerType()
        {

        }
        #endregion

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            toSave();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            toExit();
        }
    }
}
