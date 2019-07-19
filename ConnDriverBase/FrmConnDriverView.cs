using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CHQ.RD.DataContract;
namespace CHQ.RD.ConnDriverBase
{
    public partial class FrmConnDriverView : Form
    {
        public FrmConnDriverView()
        {
            InitializeComponent();
            initForm();
        }

        private int m_result = -1;
        private ConnDriverSetting m_returnedvalue = null;
        private bool m_selectmode=false;
        private List<ConnDriverSetting> m_cd;

        public ConnDriverSetting ReturnedValue
        {
            get { return m_returnedvalue; }
        }

        public void ViewConnDrivers()
        {
            this.Show();
            tbxOk.Visible = false;
        }
        public int SelectConnDriver()
        {
            m_selectmode = true;
            tbxOk.Visible = m_selectmode;
            this.ShowDialog();
            return m_result;
        }

        /// <summary>
        /// 初始化Form
        /// </summary>
        void initForm()
        {
            tvDriverClass.Nodes.Clear();
            vwConnDriver.Items.Clear();
            //初始化类型
            List<AssemblyFile> files = Ops.getDriverClassList();
            foreach(AssemblyFile file in files)
            {
                TreeNode node = new TreeNode
                {
                    Text = file.DriverName,
                    Tag = file
                };
                tvDriverClass.Nodes.Add(node);
            }
            //初始化驱动连接器
            m_cd = Ops.getConnDriverSettingList();
            foreach(ConnDriverSetting cd in m_cd)
            {
                toAddnewConnDriver(cd);
            }
        }
        #region 表单事件方法
        /// <summary>
        /// 添加新驱动连接器
        /// </summary>
        void toAddnew()
        {
            FrmConnDriverEdit edit = new FrmConnDriverEdit();
            if (tvDriverClass.SelectedNode != null)
            {
                if (edit.AddNewConnDriver((AssemblyFile)tvDriverClass.SelectedNode.Tag) == 0)
                {
                    m_cd.Add(edit.ReturnedValue);
                    toAddnewConnDriver(edit.ReturnedValue);
                }
            }
            else
            {
                if (edit.AddNewConnDriver() == 0)
                {

                    m_cd.Add(edit.ReturnedValue);
                    toAddnewConnDriver(edit.ReturnedValue);

                }
            }
        }
        /// <summary>
        /// 向列表添加驱动连接器
        /// </summary>
        /// <param name="cd">驱动连接器设置</param>
        void toAddnewConnDriver(ConnDriverSetting cd)
        {
            ListViewItem item = new ListViewItem(new string[]
                {
                    cd.Id.ToString(),
                    cd.Name.ToString(),
                    "ReadMode="+cd.ReadMode.ToString()+";ReadInterval="+cd.ReadInterval.ToString()+";TransMode="+cd.TransMode.ToString(),
                    cd.ClassFile.DriverName.ToString(),
                    "Host="+cd.DriverSet.Host+";ReadMode="+cd.DriverSet.ReadMode.ToString()+
                            ";ReadInterval="+cd.DriverSet.ReadInterval.ToString()+
                            ";TransMode="+cd.DriverSet.TransMode.ToString()
                });
            item.Tag = cd;
            vwConnDriver.Items.Add(item);
        }
        void SettingToView(ListViewItem item,ConnDriverSetting cd)
        {
            item.SubItems[1].Text = cd.Name.ToString();
            item.SubItems[2].Text = "ReadMode=" + cd.ReadMode.ToString() + ";ReadInterval=" + cd.ReadInterval.ToString() + ";TransMode=" + cd.TransMode.ToString();
            item.SubItems[3].Text = cd.ClassFile.DriverName.ToString();
            item.SubItems[4].Text = "Host=" + cd.DriverSet.Host + ";ReadMode=" + cd.DriverSet.ReadMode.ToString() +
                            ";ReadInterval=" + cd.DriverSet.ReadInterval.ToString() +
                            ";TransMode=" + cd.DriverSet.TransMode.ToString();
            item.Tag = cd;
        }
        /// <summary>
        /// 编辑选中的驱动连接器
        /// </summary>
        void toEdit()
        {
            if (vwConnDriver.SelectedItems != null && vwConnDriver.SelectedItems.Count > 0)
            {
                FrmConnDriverEdit edit = new FrmConnDriverEdit();
                if (edit.EditConnDriver((ConnDriverSetting)vwConnDriver.SelectedItems[0].Tag) == 0)
                {
                    SettingToView(vwConnDriver.SelectedItems[0],edit.ReturnedValue);
                }
            }
        }
        /// <summary>
        /// 删除选中的驱动连接器
        /// </summary>
        void toDelete()
        {
            if(vwConnDriver.SelectedItems!=null&& vwConnDriver.SelectedItems.Count > 0)
            {
                if (MyMessageBox.ShowSelectionMessage("是否确认要删除选中的驱动连接器？") == DialogResult.Yes)
                {
                    if (Ops.removeConnDriverSetting(((ConnDriverSetting)vwConnDriver.SelectedItems[0].Tag).Id) != 0)
                    {
                        MyMessageBox.ShowErrorMessage("删除设置时发生错误，请查看日志！");
                    }
                }
            }
        }
        /// <summary>
        /// 测试驱动连接器
        /// </summary>
        void toTest()
        {

        }
        /// <summary>
        /// 查看在线数据
        /// </summary>
        void toRunData()
        {

        }

        void toSelectOK()
        {
            if (vwConnDriver.SelectedItems != null && vwConnDriver.SelectedItems.Count > 0)
            {
                if (m_selectmode)
                {
                    m_returnedvalue = (ConnDriverSetting)vwConnDriver.SelectedItems[0].Tag;
                    m_result = 0;
                    this.DialogResult = DialogResult.OK;
                }
            }
        }

        void toRefresh()
        {
            initForm();
        }
        #endregion
        #region 表单事件方法触发
        private void tbxOk_Click(object sender, EventArgs e)
        {
            toSelectOK();
        }



        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            toAddnew();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            toEdit();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            toDelete();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            toTest();
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            toRunData();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            toRefresh();
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
