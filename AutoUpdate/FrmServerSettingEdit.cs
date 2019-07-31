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
            InitializeComponent();
        }

        private int m_result = -1;
        private Dictionary<string, string> m_setting = new Dictionary<string, string>();
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
            this.ShowDialog();
            return m_result;
        }

        void settingToForm()
        {

        }

        bool formToSetting()
        {

        }
        void toSave()
        {

        }
        void toExit()
        {

        }
        void toSelectServerType()
        {

        }
    }
}
