using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CHQ.RD.DataContract;
using CHQ.RD.DriverBase;
using System.Reflection;
namespace CHQ.RD.ConnectorBase
{
    public partial class FrmConnDriverEdit : Form
    {
        public FrmConnDriverEdit()
        {
            InitializeComponent();
        }

        AssemblyFile m_file=null;
        IDriverBase m_driver;
        int m_result = -1;
        DataTable m_dt;
        ConnDriverSetting m_returnedvalue;

        public int AddNewConnDriver()
        {
            return EditConnDriver(new ConnDriverSetting
            {
                Id = -1
            });
        }
        public int EditConnDriver(ConnDriverSetting conndriver)
        {
            loadConnDriver(conndriver);
            if (conndriver.Id != -1)
            {
                loadDataItems(conndriver);
            }
            this.ShowDialog();
            return m_result;
        }

        void initTable()
        {
            m_dt = new DataTable("dataitems");
            m_dt.Columns.Add("Id", typeof(int));
            m_dt.Columns.Add("Name", typeof(string));
            m_dt.Columns.Add("ConnId", typeof(int));
            m_dt.Columns.Add("TransSig", typeof(string));
            m_dt.Columns.Add("ValueType", typeof(string));
            m_dt.Columns.Add("Address", typeof(string));
        }
        void loadDataItems(ConnDriverSetting conndriver)
        {

        }
        /// <summary>
        /// 当变更驱动类型时需要变更变量表中的地址列
        /// </summary>
        /// <param name="newdriver"></param>
        void changeTable(IDriverBase newdriver)
        {
            //删除当前driver的地址列
            //如果类型一致，无需变更
            if (newdriver.AddressType == m_driver.AddressType) return;
            FieldInfo[] flds;
            //如果原类型不存在或者是地址类型不存在就不需要处理
            if ( m_driver!=null&&m_driver.AddressType!=null)
            {
                flds = m_driver.AddressType.GetFields();
                List<DataColumn> dcs = new List<DataColumn>();
                for(int i = 0; i < flds.Length; i++)
                {
                    foreach(DataColumn dc in m_dt.Columns)
                    {
                        if (dc.ColumnName == flds[i].Name)
                        {
                            dcs.Add(dc);
                        }
                    }
                }
                foreach(DataColumn dc in dcs)
                {
                    m_dt.Columns.Remove(dc);
                    vwDataItem.Columns.Remove(
                        vwDataItem.Columns[dc.ColumnName]
                        );
                }
            }
            //添加新的driver的地址列
            flds = newdriver.AddressType.GetFields();
            for (int i = 0; i < flds.Length; i++)
            {
                m_dt.Columns.Add(flds[i].Name, flds[i].FieldType);
                vwDataItem.Columns.Add(new DataGridViewColumn
                {
                    DataPropertyName = flds[i].Name,
                    Name=flds[i].Name
                });
            }
        }
        
        void initForm()
        {

        }

        public ConnDriverSetting ReturnedValue
        {
            get { return m_returnedvalue; }
        }
        /// <summary>
        /// 选择类型
        /// </summary>
        void toSelectDriver()
        {
            FrmDriverClassView v = new FrmDriverClassView();
            if (v.SelectDriver() == 1)
            {
                AssemblyFile file = v.ReturnedValue;
                Assembly asm = Assembly.LoadFile(file.FileName);
                Type type = asm.GetType(file.ClassName);
                if (m_file == null)
                {
                    onDriverTypeChanged(type);
                }
                else
                {
                    if (file.ClassName != m_file.ClassName)
                    {
                        if (MyMessageBox.ShowSelectionMessage("选择的驱动类型与原类型不同，是否继续？") == DialogResult.Yes)
                        {
                            onDriverTypeChanged(type);
                            tbxdriverclass.Text = file.ClassName;
                            tbxdriverhost.Text = "";
                        }
                    }
                }               
            }
        }

        void onDriverTypeChanged(Type newDriver)
        {

        }
        


        void loadConnDriver(ConnDriverSetting conndriver)
        {

        }

        void toAddNewRow()
        {

        }
        void toRemoveRow()
        {

        }
        void toSave()
        {

        }

    }
}
