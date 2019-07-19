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
    public partial class FrmDriverClassView : Form
    {
        List<AssemblyFile> m_files = new List<AssemblyFile>();
        bool m_selectmode = false;
        int m_result = -1;
        AssemblyFile m_returnedvalue;
        List<AssemblyFile> m_classes;// = new List<AssemblyFile>();
        
        public FrmDriverClassView()
        {
            InitializeComponent();
        }

        public AssemblyFile ReturnedValue
        {
            get { return m_returnedvalue; }
        }
        public int SelectDriver()
        {
            m_selectmode = true;
            initVIEW();
            this.ShowDialog();
            return m_result;
        }
        public void ViewDrivers()
        {
            m_selectmode = false;
            initVIEW();
            this.Show();
        }
        void initVIEW()
        {
            viewFiles.Items.Clear();
            m_classes = Ops.getDriverClassList();
            foreach(AssemblyFile file in m_classes)
            {
                ListViewItem item = new ListViewItem
                    (new string[]{
                        file.Id.ToString(),
                        file.DriverName,
                        file.ClassName,
                        file.AssemblyInfo,
                        file.FileName
                    }
                    );
                item.Tag = file;
                viewFiles.Items.Add(item);
            }
        }

        void addNewItem()
        {
            FrmDriverClassEdit edit = new FrmDriverClassEdit();
            if (edit.AddNewDriverClass() == 0)
            {
                //保存设置
                if (Ops.saveDriverClass(edit.ReturnedValue) == 0)
                {
                    AssemblyFile file=edit.ReturnedValue;
                    ListViewItem item = new ListViewItem
                    (new string[]{
                        file.Id.ToString(),
                        file.DriverName,
                        file.ClassName,
                        file.AssemblyInfo,
                        file.FileName
                    }
                    );
                    item.Tag = file;
                    viewFiles.Items.Add(item);
                }
                else
                {
                    MyMessageBox.ShowErrorMessage("操作失败，请查看日志！");
                }
            }
        }
        void removeItem()
        {
            if (viewFiles.SelectedItems != null && viewFiles.SelectedItems.Count > 0)
            {
                if (MyMessageBox.ShowSelectionMessage("是否确认要删除所选的第一项？") == DialogResult.Yes)
                {
                    AssemblyFile file = (AssemblyFile)viewFiles.SelectedItems[0].Tag;
                    if (Ops.removeDriverClass(file) == 0)
                    {
                        viewFiles.SelectedItems[0].Remove();
                    }
                    else
                    {
                        MyMessageBox.ShowErrorMessage("操作失败，请查看错误日志！");
                    }
                }
            }
            else
            {
                MyMessageBox.ShowTipMessage("未选中任何项！");
            }
        }
        void updateItem()
        {
            if(viewFiles.SelectedItems!=null && viewFiles.SelectedItems.Count > 0)
            {
                AssemblyFile oldfile = (AssemblyFile)viewFiles.SelectedItems[0].Tag;
                FrmDriverClassEdit edit = new FrmDriverClassEdit();
                if (edit.EditDriverClass(oldfile) == 0)
                {
                    AssemblyFile newfile = edit.ReturnedValue;
                    if (Ops.updateDriverClass(oldfile, newfile) == 0)
                    {
                        viewFiles.SelectedItems[0].Text = newfile.Id.ToString();
                        viewFiles.SelectedItems[0].SubItems[1].Text = newfile.DriverName;
                    }
                }
            }
        }
        void toSelectItem()
        {
            if (m_selectmode)
            {
                if (viewFiles.SelectedItems != null && viewFiles.SelectedItems.Count > 0)
                {
                    m_returnedvalue = (AssemblyFile)viewFiles.SelectedItems[0].Tag;
                    m_result = 0;
                    this.DialogResult = DialogResult.OK;
                }
            }
        }
        void toAddNew()
        {
            addNewItem();
        }
        void toUpdate()
        {
            if (viewFiles.SelectedItems != null && viewFiles.SelectedItems.Count > 0)
            {
                AssemblyFile af = (AssemblyFile)viewFiles.SelectedItems[0].Tag;
                FrmDriverClassEdit fe = new FrmDriverClassEdit();
                if (fe.EditDriverClass(af) == 0)
                {
                    if (Ops.updateDriverClass(af, fe.ReturnedValue) != 0)
                    {
                        MyMessageBox.ShowErrorMessage("保存设置时出错，数据未保存，请查看日志！");
                        return;
                    }
                    af = fe.ReturnedValue;
                    viewFiles.SelectedItems[0].Tag = af;
                    viewFiles.SelectedItems[0].Text = af.Id.ToString();
                    viewFiles.SelectedItems[0].SubItems[1].Text = af.DriverName;
                    viewFiles.SelectedItems[0].SubItems[2].Text = af.ClassName;
                    viewFiles.SelectedItems[0].SubItems[3].Text = af.AssemblyInfo;
                    viewFiles.SelectedItems[0].SubItems[4].Text = af.FileName;
                }
            }
        }
        void toRemove()
        {
            removeItem();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            toAddNew();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            toUpdate();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            toRemove();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            initVIEW();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            toSelectItem();
        }
    }
}
