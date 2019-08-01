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
    public partial class FrmUpdateSettings : Form
    {
        public FrmUpdateSettings()
        {
            InitializeComponent();
            initForm();
        }

        void initForm()
        {
            //初始化tree
            List<Dictionary<string, string>> settings = xmloper.getAUSSetingsList();
            foreach(Dictionary<string,string> set in settings)
            {
                TreeNode node = new TreeNode
                {
                    Text = set["ServerId"] + ";" + set["Type"],
                    Tag = set["ServerId"]
                };
                treeView1.Nodes.Add(node);
            }
            //获取LocalSetting
            int currentServerId = int.Parse(xmloper.getCurrentServerSetting()["ServerId"]);
            foreach(TreeNode nd in treeView1.Nodes)
            {
                if (nd.Tag.ToString() == currentServerId.ToString())
                {
                    treeView1.SelectedNode = nd;
                    break;
                }
            }
            string updType = xmloper.getCurrentUpdateType();
            comboBox1.SelectedValue = updType;
        }

        void toSave()
        {
            try
            {
                if (treeView1.SelectedNode == null)
                {
                    throw new Exception("未选择当前使用的服务器");
                }
                if (comboBox1.SelectedIndex == -1)
                {
                    throw new Exception("未选择更新方式！");
                }
                Dictionary<string, string> curr = new Dictionary<string, string>();
                curr.Add("Id", treeView1.SelectedNode.Tag.ToString());
                curr.Add("UpdateType", comboBox1.SelectedValue.ToString());
                if (xmloper.saveCurrentSetting(curr) > -1)
                {
                    this.Close();
                }
                else
                {
                    throw new Exception("保存设备错误，请查看日志！");
                }
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void toExit()
        {
            this.Close();
        }

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
