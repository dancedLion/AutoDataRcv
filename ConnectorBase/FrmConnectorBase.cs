using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Reflection;
using CHQ.RD.DataContract;
using CHQ.RD.ConnDriverBase;
namespace CHQ.RD.ConnectorBase
{
    public partial class FrmConnectorBase : Form
    {
        public FrmConnectorBase()
        {
            InitializeComponent();
            initForm();
        }
        #region variables and properties
        protected List<AssemblyFile> m_driverclasses;
        protected ConnectorBase m_connector;
        protected List<ConnDriverSetting> m_conndrivers;
        protected Type currentView;

        int m_rcId = -1;
        public int RunningConnectorId
        {
            get { return m_rcId; }
            set { m_rcId = value; }
        }
        #endregion


        void initForm()
        {
            initTree();
            //上述方法会加载驱动类型和驱动连接器
            //变量在使用时添加
        }

        void initTree()
        {
            briefView.Nodes.Clear();
            briefView.Nodes.Add(new TreeNode
            {
                Text = "连接管理器",
                Tag = "ConnDrivers",
                ImageIndex = 0
            });
            briefView.Nodes.Add(new TreeNode
            {
                Text = "驱动类型",
                Tag = "DriverClass",
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
                Tag = "ServerSetting",
                ImageIndex = 3
            });
            //加载接管理器
            TreeNode node = briefView.Nodes[0];
            m_conndrivers = ConnDriverBase.Ops.getConnDriverSettingList();
            foreach (ConnDriverSetting m in m_conndrivers)
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
            m_driverclasses =ConnDriverBase.Ops.getDriverClassList();
            foreach (AssemblyFile file in m_driverclasses)
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
            node = briefView.Nodes[3];
            node.Nodes.Add(new TreeNode
            {
                Text = "发送设置",
                Tag = "DataSendingSetting",
                ImageIndex = 31
            });
            node.Nodes.Add(new TreeNode
            {
                Text = "本地服务设置",
                Tag = "HostDataServer",
                ImageIndex = 31
            });
            node.Nodes.Add(new TreeNode
            {
                Text = "本地存储设置",
                Tag = "LocalDataBaseSetting",
                ImageIndex = 31
            });
            node.Nodes.Add(new TreeNode
            {
                Text = "报警设置",
                Tag = "DataAlertSetting",
                ImageIndex = 31
            });
        }
        #region 界面方法

        #region 显示玩意儿
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
                    if(node.Tag.ToString()== "DataSendingSetting")
                    {
                        showSendingSettings();
                    }
                    if(node.Tag.ToString()== "HostDataServer")
                    {
                        showDataServerSetting();
                    }
                    if (node.Tag.ToString() == "LocalDataBaseSetting")
                    {
                        showLocalDataSetting();
                    }
                    break;
                //case "DataSendingSetting":
                //    showSendingSettings();
                //    break;
            }
        }
        void showDataServerSetting()
        {
            detailView.Items.Clear();
            defineColumns(typeof(ConnectorHost));
            
        }
        void showLocalDataSetting()
        {
            detailView.Items.Clear();
            defineColumns(typeof(ConnectorLocalData));
            List<ConnectorLocalData> l = ConnectorOps.getConnectorLocalDataList();
            foreach(ConnectorLocalData cld in l)
            {
                ListViewItem item = new ListViewItem(new string[]
                {
                    cld.Id.ToString(),cld.Desc,cld.RDType.ToString(),cld.DBDriverType.ToString(),cld.ConnectString
                });
                item.Tag = cld;
                detailView.Items.Add(item);
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
                detailView.Items.Add(item);
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
                    ds.ConnDriverClass==null?"":ds.ConnDriverClass.FullName,
                    ds.ReadMode.ToString(),
                    ds.ReadInterval.ToString(),
                    ds.TransMode.ToString(),
                    ds.ClassFile==null?"":ds.ClassFile.DriverName,
                    "",
                    ds.DriverSet.ToString()
                });
                item.Tag =ds;
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
        /// 显示当前管理器下的发送设置
        /// </summary>
        void showSendingSettings()
        {
            detailView.Items.Clear();
            defineColumns(typeof(DataSendingSet));
            List<DataSendingSet> list = ConnectorOps.getDataSendingList(m_rcId);
            foreach (DataSendingSet l in list)
            {
                ListViewItem item = new ListViewItem(new string[]
                {
                    l.Id.ToString(),l.Name,
                    l.Host,l.HostPort.ToString(),
                    l.SendInterval.ToString(),l.Memo,
                    l.ConnDrivers,l.Via.ToString()
                });
                item.Tag = l;
                detailView.Items.Add(item);
            }
        }
        /// <summary>
        /// 新增或者修改后，同步m_conndrivers列表及detailview列表
        /// </summary>
        /// <param name="cds"></param>
        /// <param name="item"></param>
        void ConnDriverToList(ConnDriverSetting cds,ListViewItem item)
        {
            if (item == null)
            {
                item = new ListViewItem(new string[]
                {
                    cds.Id.ToString(),
                    cds.Name,
                    cds.ConnDriverClass.FullName,
                    cds.ReadMode.ToString(),
                    cds.ReadInterval.ToString(),
                    cds.TransMode.ToString(),
                    cds.ClassFile==null?"":cds.ClassFile.DriverName,
                    "",
                    cds.DriverSet.ToString()
                });
                item.Tag = cds;
                detailView.Items.Add(item);
                m_conndrivers.Add(cds);
            }
            else
            {
                //修改
                foreach(ConnDriverSetting cd in m_conndrivers)
                {
                    if (cd.Id == cds.Id)
                    {
                        cd.Name = cds.Name;
                        cd.ReadInterval = cds.ReadInterval;
                        cd.TransMode = cds.TransMode;
                        cd.ReadMode = cds.ReadMode;
                        cd.ClassFile = cds.ClassFile;
                        cd.ConnDriverClass = cds.ConnDriverClass;
                        cd.DataItems = cds.DataItems;
                        cd.DriverSet = cds.DriverSet;
                        break;
                    }
                }
                item.SubItems[1].Text = cds.Name;
                item.SubItems[2].Text = cds.ConnDriverClass.FullName;
                item.SubItems[3].Text = cds.ReadMode.ToString();
                item.SubItems[4].Text = cds.ReadInterval.ToString();
                item.SubItems[5].Text = cds.TransMode.ToString();
                item.SubItems[6].Text = cds.ClassFile == null ? "" : cds.ClassFile.DriverName;
                item.SubItems[8].Text = cds.DriverSet == null ? "" : cds.DriverSet.ToString();
                item.Tag = cds;     
            }
        }
        #endregion

        
        #region 增加新玩意
        void AddNew()
        {
            if (briefView.SelectedNode != null)
            {
                TreeNode node = getTopLevelNode(briefView.SelectedNode);
                switch (node.Tag.ToString())
                {
                    case "ServerSetting:":
                        switch (briefView.SelectedNode.Tag.ToString())
                        {
                            case "DataSendingSetting":
                                toAddDataSendingSet();
                                break;
                            case "LocalDataBaseSetting":
                                toAddLocalDataSetting();
                                break;
                        }
                        break;
                    case "ConnDrivers":
                        if (briefView.SelectedNode.Equals(node))
                        {
                            toAddNewConnDriver();
                        }
                        else
                        {
                            //不单独添加数据项
                        }
                        break;
                    case "DriverClass":
                        if (briefView.SelectedNode.Equals(node))
                        {
                            toAddNewDriverClass();
                        }
                        else
                        {
                            toAddNewConnDriver();
                        }
                        break;
                }
            }
        }

        void toAddNewConnDriver()
        {

            if (briefView.SelectedNode != null)
            {
                FrmConnDriverEdit edit = new FrmConnDriverEdit();
                TreeNode node = getTopLevelNode(briefView.SelectedNode);
                if (node.Tag.ToString() == "ConnDrivers")
                {
                    if (edit.AddNewConnDriver() > -1)
                    {
                        ConnDriverToList(edit.ReturnedValue, null);
                    }
                }
                else
                {
                    if (node.Tag.ToString() == "DriverClass")
                    {
                        if (object.Equals(node, briefView.SelectedNode))
                        {
                            //addnew方法，没得事了
                        }
                        else
                        {
                            if (edit.AddNewConnDriver((AssemblyFile)briefView.SelectedNode.Tag) > 0)
                            {
                                ConnDriverToList(edit.ReturnedValue, null);
                            }
                        }
                    }
                }
                if (edit.DialogResult==DialogResult.OK)
                {
                    
                }
            }
        }

        void toAddNewDriverClass()
        {
            FrmDriverClassEdit edit = new FrmDriverClassEdit();
            if (edit.AddNewDriverClass() > -1)
            {
                AssemblyFile af = edit.ReturnedValue;
                briefView.SelectedNode.Nodes.Add(new TreeNode
                {
                    Tag = af,
                    Text = af.DriverName,
                    SelectedImageIndex = 22
                });
                m_driverclasses.Add(af);
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
        /// 添加新的发送设置
        /// </summary>
        void toAddDataSendingSet()
        {
            FrmSendingEdit frm = new FrmSendingEdit();
            if (frm.EditDataSendingSet(m_rcId) >= 0)
            {
                DataSendingSet dss = frm.ReturnedValue;
                ListViewItem item = new ListViewItem(new string[]
                {
                    dss.Id.ToString(),dss.Name,
                    dss.Host,dss.HostPort.ToString(),
                    dss.SendInterval.ToString(),dss.Memo,
                    dss.ConnDrivers,dss.Via.ToString()
                });
                item.Tag = dss;
                detailView.Items.Add(item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void toAddLocalDataSetting()
        {
            FrmDataStorage frm = new FrmDataStorage();
            if (frm.EditConnectorLocalData() >= 0)
            {
                ConnectorLocalData cld = frm.ReturnedValue;
                ListViewItem item = new ListViewItem(new string[]
                {
                    cld.Id.ToString(),cld.Desc,cld.RDType.ToString(),cld.DBDriverType.ToString(),cld.ConnectString
                });
                item.Tag = cld;
                detailView.Items.Add(item);
            }
        }
        #endregion
        #region 修改玩意儿
        void Edit()
        {
            if (detailView.SelectedItems == null || detailView.SelectedItems.Count == 0)
            {
                return;
            }
            if (briefView.SelectedNode != null)
            {
                
                switch (briefView.SelectedNode.Tag.ToString())
                {
                    case "DataSendingSetting":
                        toEditDataSendingSet();
                        break;
                    case "LocalDataBaseSetting":
                        toEditLocalDataSetting();
                        break;
                    case "ConnDrivers":
                    case "":
                        toEditConnDriver();
                        break;
                }
            }
        }

        void toEditConnDriver()
        {
            FrmConnDriverEdit edit = new FrmConnDriverEdit();
            if (edit.EditConnDriver((ConnDriverSetting)detailView.SelectedItems[0].Tag) > -1)
            {
                ConnDriverSetting ds = edit.ReturnedValue;
                ListViewItem item = detailView.SelectedItems[0];
                ConnDriverToList(ds, item);
            }
        }
        void toEditDataSendingSet()
        {
            FrmSendingEdit frm = new FrmSendingEdit();
            if (frm.EditDataSendingSet(m_rcId, (DataSendingSet)detailView.SelectedItems[0].Tag) > -1)
            {
                //更新
                detailView.SelectedItems[0].Remove();
                ListViewItem item = new ListViewItem(new string[]
                {
                    frm.ReturnedValue.Id.ToString(),frm.ReturnedValue.Name,
                    frm.ReturnedValue.Host,frm.ReturnedValue.HostPort.ToString(),
                    frm.ReturnedValue.SendInterval.ToString(),frm.ReturnedValue.Memo,
                    frm.ReturnedValue.ConnDrivers,frm.ReturnedValue.Via.ToString()
                });
                item.Tag = frm.ReturnedValue;
                detailView.Items.Add(item);
            }
        }
        void toEditLocalDataSetting()
        {

            FrmDataStorage fds = new FrmDataStorage();
            if (fds.EditConnectorLocalData((ConnectorLocalData)detailView.SelectedItems[0].Tag) > -1)
            {
                detailView.SelectedItems[0].Remove();
                ConnectorLocalData cld = fds.ReturnedValue;
                ListViewItem item = new ListViewItem(new string[]
                {
                    cld.Id.ToString(),cld.Desc,cld.RDType.ToString(),cld.DBDriverType.ToString(),cld.ConnectString
                });
                item.Tag = cld;
                detailView.Items.Add(item);
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
        #endregion
       
        #endregion
        #region 驱动连接器操作
        void startConnDriver(ConnDriverSetting conndriver)
        {

        }
        void stopConnDriver(ConnDriverSetting conndriver)
        {

        }
        /// <summary>
        /// 测试驱动连接器设置
        /// 1、传设置
        /// 2、尝试连接
        /// 3、尝试读取所有变量
        /// </summary>
        /// <param name="conndriver"></param>
        void testConnDriver(ConnDriverSetting conndriver)
        {
            //创建connDriver实例
            if (conndriver.ClassFile ==null)
            {
                addMessage("类文件未设置无法启动测试");
                return;
            }
            addMessage("开始初始化驱动连接器实例！");
            ConnectorBase tmpbase = new ConnectorBase(-1);
            //创建Driver实例
            tmpbase.TestConnDriver(new ConnDriverBase.ConnDriverBase(conndriver.Id));
            //传入设置
            Thread.Sleep(conndriver.ReadInterval);
            foreach(KeyValuePair<int,object> pr in tmpbase.ValueList)
            {
                addMessage(pr.Key.ToString() + ":" + pr.Value.ToString());
            }
            //准备读取值 
            //测试完成
            tmpbase = null;
        }
        

        #endregion
        #region 公用操作
        void addMessage(string msg)
        {
            messageList.Items.Insert(0, msg);
        }
        TreeNode getTopLevelNode(TreeNode node)
        {
            TreeNode ret = node;
            if (node.Parent != null) { return getTopLevelNode(node.Parent); }
            else
            {
                //nothing todo
            }
            return ret;
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

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            if (detailView.SelectedItems != null && detailView.SelectedItems.Count > 0)
            {
                try
                {
                    ConnDriverSetting conn = (ConnDriverSetting)detailView.SelectedItems[0].Tag;
                    testConnDriver(conn);
                }
                catch
                {
                    MyMessageBox.ShowErrorMessage("请在详细信息中选择驱动连接器");
                }
            }
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            AddNew();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            Edit();
        }
    }
}
