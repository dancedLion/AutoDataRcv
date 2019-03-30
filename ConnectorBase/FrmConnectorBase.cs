using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using CHQ.RD.DataContract;
namespace CHQ.RD.ConnectorBase
{
    public partial class FrmConnectorBase : Form
    {
        public FrmConnectorBase()
        {
            InitializeComponent();
            initForm();
        }
        protected List<AssemblyFile> m_driverclasses;
        protected ConnectorBase m_connector;
        protected List<ConnDriverSetting> m_conndrivers;
        protected Type currentView;
        void initForm()
        {
            initTree();
            //上述方法会加载驱动类型和驱动连接器
            //变量在使用时添加
        }

        #region 界面方法
        void onNodeSelect(TreeNode node)
        {
            if (node == null) return;
            TreeNode parent = node;
            //获取顶级，知道其所在位置
            while (parent.Parent != null)
            {
                parent = parent.Parent;
            }
            switch (parent.Tag.ToString())
            {
                case "ConnDrivers":
                    //如果子节点，显示变量列表
                    //否则显示所有的驱动边器
                    if (node != parent)
                    {
                        //非根节点
                        showConnDriverDataItem((ConnDriverSetting)node.Tag);
                    }
                    else
                    {
                        showConnDrivers();
                    }
                    break;
                case "DriverClass":
                    //如果父节点，显示所有的列表
                    //否则显示其下的驱动连接器
                    if (node != parent)
                    {
                        List<ConnDriverSetting> templist = new List<ConnDriverSetting>();
                        foreach(ConnDriverSetting cds in m_conndrivers)
                        {
                            if (cds.ClassFile.ClassName == ((AssemblyFile)node.Tag).ClassName)
                            {
                                templist.Add(cds);
                            }
                        }
                        showConnDrivers(templist);
                    }
                    else
                    {
                        showDriverClasses();
                    }
                    break;
                case "CustomView":
                    //显示其下的所有节点及驱动连接器
                    break;
                case "ServerSetting":
                    //显示设置或者设置清单
                    break;
            }
        }
        /// <summary>
        /// 显示指定驱动连接器下的数据变量
        /// </summary>
        /// <param name="conndriver">ConnDriverSetting</param>
        void showConnDriverDataItem(ConnDriverSetting conndriver)
        {
            detailView.Items.Clear();
            defineColumns(typeof(ConnectorDataItem));
            DataTable items = Ops.getConnDriverDataItems(conndriver.Id);
            foreach(DataRow dr in items.Rows)
            {
                ListViewItem item = new ListViewItem(new string[]
                {
                    dr["Id"].ToString(),
                    dr["Name"].ToString(),
                    dr["ConnId"].ToString(),
                    dr["TransSig"].ToString(),
                    dr["Address"].ToString(),
                    dr["ValueType"].ToString()
                });
                item.Tag = dr["Id"];
            }
        }
        void showConnDrivers()
        {
            showConnDrivers(m_conndrivers);
        }
        /// <summary>
        /// 显示指定列表中的驱动连接器
        /// </summary>
        /// <param name="conndrivers"></param>
        void showConnDrivers(List<ConnDriverSetting> conndrivers)
        {
            detailView.Items.Clear();
            defineColumns(typeof(ConnDriverSetting));
            foreach(ConnDriverSetting ds in conndrivers)
            {
                ListViewItem item = new ListViewItem(new string[]
                {
                    ds.Id.ToString(),
                    ds.Name,
                    ds.ReadMode.ToString(),
                    ds.ReadInterval.ToString(),
                    ds.TransMode.ToString(),
                    ds.ClassFile.DriverName,
                    "",
                    ds.DriverSet.ToString()
                });
                item.Tag = ds.Id;
                detailView.Items.Add(item);
            }
        }
        void showItems(Type type,DataTable dt)
        {

        }
        /// <summary>
        /// 根据类型来定制视图列
        /// </summary>
        /// <param name="type"></param>
        void defineColumns(Type type)
        {
            //当前视图与将要显示的视图一致
            if (currentView != null && currentView == type) return;
            //更新视图列
            currentView = type;
            detailView.Columns.Clear();
            FieldInfo[] flds = type.GetFields();
            for(int i =0; i < flds.Length; i++)
            {
                detailView.Columns.Add(new ColumnHeader
                {
                    //Text=getFieldDescription
                    Text=flds[i].Name,
                    Width=100
                });
            }
        }
        /// <summary>
        /// 显示所有的驱动类型
        /// </summary>
        void showDriverClasses()
        {
            detailView.Items.Clear();
            defineColumns(typeof(AssemblyFile));
            foreach(AssemblyFile af in m_driverclasses)
            {
                ListViewItem item = new ListViewItem
                (new string[] {
                    af.Id.ToString(),af.DriverName,af.ClassName,af.AssemblyInfo,af.FileName
                });
                item.Tag = af;
                detailView.Items.Add(item);
            }
        }
        /// <summary>
        /// 添加自定义设置文件夹
        /// </summary>
        void addNewCustomFolder()
        {
            FrmInputBox inputbox = new FrmInputBox();
            if (inputbox.GetInput("自定值") == 0)
            {
                //添加树节点
                briefView.Nodes.Add(new TreeNode
                {
                    Text = inputbox.ReturnedValue,
                    Tag = "Folder"
                });
                //TODO:保存
            }
        }
        /// <summary>
        /// 修改已选择的节点的文本
        /// </summary>
        /// <param name="node">已选择的节点</param>
        void editCustomFolder(TreeNode node)
        {
            FrmInputBox inputBox = new FrmInputBox();
            if (inputBox.GetInput("自定值", node.Text) == 0)
            {
                //TODO:保存设置
                //修改选中节点的文本
                node.Text = inputBox.ReturnedValue;
            }
        }
        void initTree()
        {
            briefView.Nodes.Clear();
            briefView.Nodes.Add(new TreeNode
            {
                Text = "连接管理器",
                Tag="ConnDrivers",
                ImageIndex = 0
            });
            briefView.Nodes.Add(new TreeNode
            {
                Text = "驱动类型",
                Tag="DriverClass",
                ImageIndex = 1
            });
            briefView.Nodes.Add(new TreeNode
            {
                Text = "自定义组织",
                Tag = "CustomView",
                ImageIndex = 2
            });
            briefView.Nodes.Add(new TreeNode
            {
                Text = "连接管理器设置",
                Tag="ServerSetting",
                ImageIndex = 3
            });
            //加载接管理器
            TreeNode node = briefView.Nodes[0];
            m_conndrivers = Ops.getConnDriverSettingList();
            foreach(ConnDriverSetting m in m_conndrivers)
            {
                TreeNode subnode = new TreeNode
                {
                    Text = m.Name,
                    Tag = m,
                    ImageIndex = 4
                };
                node.Nodes.Add(subnode);
            }
            //加载驱动设置
            node = briefView.Nodes[1];
            m_driverclasses = Ops.getDriverClassList();
            foreach(AssemblyFile file in m_driverclasses)
            {
                TreeNode subnode = new TreeNode
                {
                    Text = file.DriverName,
                    Tag = file,
                    ImageIndex = 4
                };
                node.Nodes.Add(subnode);
            }
            //加载可设置项
        }
        #endregion
        #region 驱动连接器操作
        void startConnDriver(ConnDriverSetting conndriver)
        {

        }
        void stopConnDriver(ConnDriverSetting conndriver)
        {

        }
        #endregion

        #region 服务器操作
        #endregion
        private void 编辑驱动类型ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDriverClassView v = new FrmDriverClassView();
            v.ViewDrivers();
        }

        private void 驱动连接器管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmConnDriverView v = new FrmConnDriverView();
            v.ViewConnDrivers();
        }

        private void briefView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (briefView.SelectedNode != null)
            {
                onNodeSelect(briefView.SelectedNode);
            }
        }
    }
    class AllView
    {
        public string Id;
        public string Type;
        public string Name;
        public string Memo;
    }

    class ConnDriverInfo
    {
        public string Id;
        public string Name;
        public string ConnDriverStatus;
        public string DriverName;
        public string DriverStatus;
    }
    class DataItemValue
    {
        public string Id;
        public string ConnDriverName;
        public string VariableName;
        public string Address;
        public string Type;
        public string Value;
    }
}
