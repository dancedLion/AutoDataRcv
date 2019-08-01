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
            refForm();
            refList();
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
            refForm();
            this.ShowDialog();
            return m_result;
        }

        #region local methods
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
            item.SubItems.Add(server["Type"]);
            item.SubItems.Add("");
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
            FrmServerSettingEdit e = new FrmServerSettingEdit();
            if (e.AddnewSetting() > -1)
            {
                toAddItem(e.ReturnedValue);
            }
        }
        void toEditSetting()
        {
            if (listView1.SelectedItems == null || listView1.SelectedItems.Count == 0)
            {
                return;
            }
            FrmServerSettingEdit e = new FrmServerSettingEdit();
            if (e.EditSetting((Dictionary<string, string>)listView1.SelectedItems[0].Tag) > -1)
            {
                ListViewItem item = listView1.SelectedItems[0];
                item.SubItems[1].Text = e.ReturnedValue["Type"];
                item.SubItems[2].Text = "";
                foreach(KeyValuePair<string,string> kv in e.ReturnedValue)
                {
                    if (kv.Key == "ServerId" || kv.Key == "Type")
                    {
                        continue;
                    }
                    else
                    {
                        item.SubItems[2].Text += kv.Key + ";" + kv.Value + ";";
                    }
                }
                item.Tag = e.ReturnedValue;
            }
        }
        void toDeleteSetting()
        {
            if (listView1.SelectedItems == null || listView1.SelectedItems.Count == 0)
            {
                return;
            }
            if (xmloper.delAUSSetting(int.Parse(listView1.SelectedItems[0].Text)) > -1)
            {
                listView1.SelectedItems[0].Remove();
            }
            else
            {
                MessageBox.Show("删除失败，请查看日志！");
            }
        }
        void toSelectSetting()
        {
            if (m_isselect)
            {
                if (listView1.SelectedItems != null && listView1.SelectedItems.Count > 0)
                {
                    m_returnvalue = int.Parse(listView1.SelectedItems[0].Text);
                    m_result = 0;
                    this.DialogResult = DialogResult.OK;
                }
            }
        }
        void toExit()
        {
            if (m_isselect)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else
            {
                this.Close();
            }
        }

        #endregion

        #region 人机交互

        private void buttonOk_Click(object sender, EventArgs e)
        {
            toSelectSetting();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            toAddnewSetting();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            toEditSetting();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            toDeleteSetting();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            refList();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            toExit();
        }
        #endregion
    }
}
