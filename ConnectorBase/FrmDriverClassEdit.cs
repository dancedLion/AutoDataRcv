using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CHQ.RD.DataContract;
namespace CHQ.RD.ConnectorBase
{
    public partial class FrmDriverClassEdit : Form
    {
        AssemblyFile m_returnedvalue = null;
        int m_result = -1;
        public FrmDriverClassEdit()
        {
            InitializeComponent();
        }

        public AssemblyFile ReturnedValue
        {
            get { return m_returnedvalue; }
        }

        public int AddNewDriverClass()
        {
            //m_returnedvalue = new AssemblyFile { Id = -1 };
            return EditDriverClass(new AssemblyFile { Id = -1 });
        }
        public int EditDriverClass(AssemblyFile driverclass){
            m_returnedvalue = driverclass;
            AssemblyFileToForm();
            this.ShowDialog();
            return m_result;
        }

        void toSelectFromFile()
        {
            FrmSelectClassFromFile selform = new FrmSelectClassFromFile();
            if (selform.SelectDriverClass() == 0)
            {
                tbxclassname.Text = selform.ReturnedValue.ClassName;
                tbxassemblyinfo.Text = selform.ReturnedValue.AssemblyInfo;
                tbxfile.Text = selform.ReturnedValue.FileName;
            }
        }
        void AssemblyFileToForm()
        {
            tbxid.Text = m_returnedvalue.Id.ToString();
            tbxname.Text = m_returnedvalue.DriverName;
            tbxclassname.Text = m_returnedvalue.ClassName;
            tbxassemblyinfo.Text = m_returnedvalue.AssemblyInfo;
            tbxfile.Text = m_returnedvalue.FileName;
        }
        int FormToAssemblyFile()
        {
            int ret = -1;
            try
            {
                m_returnedvalue.Id = int.Parse(tbxid.Text);
                if (m_returnedvalue.Id == -1) throw new Exception("未指定注册的驱动类型的ID");
                if (tbxname.Text.Trim() == "") throw new Exception("未指定名称");
                if (tbxclassname.Text == "") throw new Exception("类名称不能为空");
                m_returnedvalue.DriverName = tbxname.Text;
                m_returnedvalue.ClassName = tbxclassname.Text;
                m_returnedvalue.AssemblyInfo = tbxassemblyinfo.Text;
                m_returnedvalue.FileName = tbxfile.Text;
                ret = 0;
            }
            catch(Exception ex)
            {
                MyMessageBox.ShowErrorMessage(ex.Message);
            }
            return ret;
        }
        void toSave()
        {
            if (FormToAssemblyFile() == 0)
            {
                m_result = 0;
                this.DialogResult = DialogResult.OK;
            }
        }
        void toCancel()
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
