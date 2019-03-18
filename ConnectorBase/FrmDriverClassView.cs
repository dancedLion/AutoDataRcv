using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CHQ.RD.ConnectorBase
{
    public partial class FrmDriverClassView : Form
    {
        List<AssemblyFile> m_files = new List<AssemblyFile>();
        bool m_selectmode = false;
        int m_returnvalue = -1;
        List<AssemblyFile> m_classes;// = new List<AssemblyFile>();

        public FrmDriverClassView()
        {
            InitializeComponent();
        }

        void initVIEW()
        {
            viewFiles.Items.Clear();
            m_classes = Ops.getDriverClassList();
            foreach(AssemblyFile file in m_classes)
            {
                ListViewItem item = new ListViewItem
                {
                    Text = file.Id.ToString(),
                    Tag=file
                };
                item.SubItems[1].Text = file.DriverName;
                item.SubItems[2].Text = file.ClassName;
                item.SubItems[3].Text = file.AssemblyInfo;
                item.SubItems[4].Text = file.FileName;
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
                    {
                        Text = file.Id.ToString(),
                        Tag = file
                    };
                    item.SubItems[1].Text = file.DriverName;
                    item.SubItems[2].Text = file.ClassName;
                    item.SubItems[3].Text = file.AssemblyInfo;
                    item.SubItems[4].Text = file.FileName;
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

        }
        void toAddNew()
        {
            addNewItem();
        }
        void toUpdate()
        {

        }
        void toRemove()
        {
            removeItem();
        }
    }
}
