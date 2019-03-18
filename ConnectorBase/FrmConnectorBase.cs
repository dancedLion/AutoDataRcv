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
    public partial class FrmConnectorBase : Form
    {
        public FrmConnectorBase()
        {
            InitializeComponent();
        }
        protected List<AssemblyFile> m_driverclasses;
        protected ConnectorBase m_connector;



        #region 方法
        void showViewColumns(Type type)
        {
            detailView.Columns.Clear();
            FieldInfo[] flds = type.GetFields();
            for(int i = 0; i < flds.Length; i++)
            {
                ColumnHeader header = new ColumnHeader();
                header.Width = 100;
                header.Text = flds[i].Name;
                detailView.Columns.Add(header);
            }
            detailView.Tag = type;
        }
        void showItems(Type type,DataTable dt)
        {

        }
        void defineColumns(Type type)
        {

        }

        void loadColumns(Type type)
        {

        }

        void initTree()
        {
            briefView.Nodes.Clear();
            briefView.Nodes.Add(new TreeNode
            {
                Text = "连接管理器",
                ImageIndex = 0
            });
            briefView.Nodes.Add(new TreeNode
            {
                Text = "驱动类型",
                ImageIndex = 1
            });
            briefView.Nodes.Add(new TreeNode
            {
                Text = "连接管理器设置",
                ImageIndex = 2
            });
            //加载加接管理器
            //加载驱动设置
            //加载可设置项
        }
        #endregion
    }
    class AllView
    {
        public string Id;
        public string Type;
        public string Name;
        public string Memo;
    }
    public class AssemblyFile
    {
        public int Id;
        public string DriverName;
        public string ClassName;
        public string AssemblyInfo;
        public string FileName;
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
