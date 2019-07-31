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
    public partial class FrmServerSettingsView : Form
    {
        public FrmServerSettingsView()
        {
            InitializeComponent();
        }

        private bool m_isselect = false;
        private int m_returnvalue = -1;
        private int m_result = -1;
        public int ReturnedValue
        {
            get { return m_returnvalue; }
        }

        public int SelectServerSetting()
        {
            m_isselect = true;
            this.ShowDialog();
            return m_result;
        }

        #region
        void refForm()
        {
            buttonOk.Visible = m_isselect;
            splitOK.Visible = m_isselect;
        }
        /// <summary>
        /// 刷新列表
        /// </summary>
        void refList()
        {
            listView1.Items.Clear();
            List<Dictionary<string, string>> serverList = xmloper.getAUSSetingsList();
            foreach(Dictionary<string,string> server in serverList)
            {
                toAddItem(server);
            }
        }
        /// <summary>
        /// 根据某项服务器设置写入到列表中
        /// </summary>
        /// <param name="server"></param>
        void toAddItem(Dictionary<string,string> server)
        {
            ListViewItem item = new ListViewItem(server["ServerId"]);
            item.SubItems[1].Text = server["Type"];
            foreach (KeyValuePair<string, string> kv in server)
            {
                if (kv.Key == "ServerId" || kv.Key == "Type")
                {
                    //DoNothing
                }
                else
                {
                    item.SubItems[2].Text += kv.Key + ":" + kv.Value + ";";
                }
            }
            item.Tag = server;
            listView1.Items.Add(item);
        }

        void toAddnewSetting()
        {

        }
        void toEditSetting()
        {

        }
        void toDeleteSetting()
        {

        }
        void toSelectSetting()
        {

        }
        void toExit()
        {

        }
        #endregion
    }
}
