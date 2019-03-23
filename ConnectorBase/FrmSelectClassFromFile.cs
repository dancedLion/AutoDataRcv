using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using CHQ.RD.DriverBase;
using CHQ.RD.DataContract;
namespace CHQ.RD.ConnectorBase
{
    public partial class FrmSelectClassFromFile : Form
    {
        AssemblyFile m_returnedvalue = null;
        int m_result = -1;
        public FrmSelectClassFromFile()
        {
            InitializeComponent();
        }

        public AssemblyFile ReturnedValue
        {
            get { return m_returnedvalue; }
        }

        public int SelectDriverClass()
        {
            if (openDllFile.ShowDialog() == DialogResult.OK)
            {
                string filename = openDllFile.FileName;
                loadFile(filename);
                this.ShowDialog();
            }
            else
            {
                return -1;
            }
            return m_result;
        }
        void toLoadFile()
        {
            if (openDllFile.ShowDialog() == DialogResult.OK)
            {
                loadFile(openDllFile.FileName);
            }
        }
        void loadFile(string filename)
        {
            //相同文件不再加载
            Assembly asm = Assembly.LoadFile(filename);
            foreach(TreeNode node in treeClasses.Nodes)
            {
                if ((Assembly)node.Tag==asm)
                {
                    //已加载过
                    return;
                }
            }
            //加载命名空间
            TreeNode nd = new TreeNode();
            nd.ImageIndex = 0;
            nd.Text = asm.GetName().Name;
            nd.Tag = asm;
            //加载类
            Type[] tps = asm.GetTypes();
            for(int i = 0; i < tps.Length; i++)
            {
                if (typeof(IDriverBase).IsAssignableFrom(tps[i]))
                {
                    TreeNode node = new TreeNode();
                    node.Text = tps[i].FullName;
                    node.Tag = tps[i];
                    node.ImageIndex = 1;
                    nd.Nodes.Add(node);
                }
            }
            treeClasses.Nodes.Add(nd);
        }

        public void selectOK()
        {
            if (treeClasses.SelectedNode != null)
            {
                if (treeClasses.SelectedNode.ImageIndex == 1)
                {
                    Type tp = (Type)treeClasses.SelectedNode.Tag;
                    m_returnedvalue = new AssemblyFile
                    {
                        ClassName = tp.FullName,
                        AssemblyInfo = tp.Assembly.FullName,
                        FileName = tp.Assembly.Location
                    };
                    m_result = 0;
                    this.DialogResult = DialogResult.OK;
                }
            }
        }

        void toCancel()
        {
            m_returnedvalue = null;
            this.DialogResult = DialogResult.Cancel;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            selectOK();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            toLoadFile();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            toCancel();
        }
        void toShowDriverInfo(Type type)
        {
            txtDetails.Text = type.FullName;
            txtDetails.Text += "\r\n" + type.Assembly.FullName;
            txtDetails.Text += "\r\n" + type.Assembly.Location;
            txtDetails.Text += "\r\n" + "Properties:";
            PropertyInfo[] props = type.GetProperties();
            for(int i = 0; i < props.Length; i++)
            {
                txtDetails.Text += "\r\n" + props[i].Name + " " + props[i].PropertyType.ToString();
            }
            txtDetails.Text += "\r\nMethods";
            MethodInfo[] meths = type.GetMethods();
            for(int i = 0; i < meths.Length; i++)
            {
                txtDetails.Text += "\r\n" + meths[i].Name + " " + meths[i].ReturnType.ToString();
            }
            
        }

        private void treeClasses_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeClasses.SelectedNode != null)
            {
                if (treeClasses.SelectedNode.ImageIndex == 1)
                {
                    toShowDriverInfo((Type)treeClasses.SelectedNode.Tag);
                }
            }
            else
            {
                txtDetails.Text = "";
            }
        }
    }
}
