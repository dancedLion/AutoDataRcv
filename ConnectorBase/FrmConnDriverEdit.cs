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
        Type m_driver=null;
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
        public int AddNewConnDriver(DriverSetting driver)
        {
            return EditConnDriver(new ConnDriverSetting
            {
                Id = -1,
                DriverSet = driver
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
        /// <summary>
        /// 加载数据表
        /// </summary>
        /// <param name="conndriver"></param>
        void loadDataItems(ConnDriverSetting conndriver)
        {
            try
            {
                m_dt.Rows.Clear();
                DataTable items = Ops.getConnDriverDataItems(conndriver.Id);
                Type type = null;
                if (conndriver.ClassFile != null)
                {
                    AssemblyFile file = Ops.getDriverClass(conndriver.ClassFile.Id);
                    Assembly asm = Assembly.LoadFile(file.FileName);
                    type = asm.GetType(file.ClassName).GetField("AddressType").FieldType;
                }
                foreach (DataRow r in items.Rows)
                {

                    DataRow dr = m_dt.NewRow();
                    if (type != null)
                    {
                        object o = Ops.ParsingAddress(type, dr["Address"].ToString());
                        FieldInfo[] flds = type.GetFields();
                        for (int i = 0; i < flds.Length; i++)
                        {
                            //TODO: 不知道如何取值 
                            dr[flds[i].Name] = flds[i].GetValue(Convert.ChangeType(o, type));
                        }
                    }
                    dr["Id"] = r["Id"];
                    dr["Name"] = r["Name"];
                    dr["ConnId"] = r["ConnId"];
                    dr["TransSig"] = r["TransSig"];
                    dr["ValueType"] = r["ValueType"];
                    dr["Address"] = r["Address"];
                    m_dt.Rows.Add(dr);
                }
                m_dt.AcceptChanges();
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowErrorMessage(ex.Message);
            }
        }
        /// <summary>
        /// 当变更驱动类型时需要变更变量表中的地址列
        /// </summary>
        /// <param name="newdriver"></param>
        void changeTable(Type type)
        {
            try
            {
                //删除当前driver的地址列
                //如果类型一致，无需变更
                if (m_driver != null && type == m_driver.GetField("AddressType").FieldType) return;
                FieldInfo[] flds;
                //如果原类型不存在或者是地址类型不存在就不需要处理
                if (m_driver != null && m_driver.GetField("AddressType").FieldType != null)
                {
                    flds = m_driver.GetField("AddressType").FieldType.GetFields();
                    List<DataColumn> dcs = new List<DataColumn>();
                    for (int i = 0; i < flds.Length; i++)
                    {
                        foreach (DataColumn dc in m_dt.Columns)
                        {
                            if (dc.ColumnName == flds[i].Name)
                            {
                                dcs.Add(dc);
                            }
                        }
                    }
                    foreach (DataColumn dc in dcs)
                    {
                        m_dt.Columns.Remove(dc);
                        vwDataItem.Columns.Remove(
                            vwDataItem.Columns[dc.ColumnName]
                            );
                    }
                }
                //添加新的driver的地址列
                flds = type.GetFields();
                for (int i = 0; i < flds.Length; i++)
                {
                    m_dt.Columns.Add(flds[i].Name, flds[i].FieldType);
                    vwDataItem.Columns.Add(new DataGridViewColumn
                    {
                        DataPropertyName = flds[i].Name,
                        Name = flds[i].Name
                    });
                }
                foreach (DataRow dr in m_dt.Rows)
                {
                    dr["Address"] = "";
                }
            }
            catch(Exception ex)
            {
                MyMessageBox.ShowErrorMessage(ex.Message);
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
                        }
                    }
                }               
            }
        }
/// <summary>
/// 选择不同的驱动后，要判断 是否更新HOST，是否更新数据项
/// </summary>
/// <param name="newDriver"></param>
        void onDriverTypeChanged(Type newDriver)
        {
            //当类型更改时将
            //如果HOST不同，将空白HOST
            //如果地址设置不同，将空白ADDRESS
            try
            {
                if (m_driver == null)
                {
                    changeTable(newDriver.GetField("AddressType").FieldType);
                }
                else
                {
                    if (m_driver.GetField("HostType").FieldType != newDriver.GetField("HostType").FieldType)
                    {
                        tbxdriverhost.Text = "";
                    }
                    if (m_driver.GetField("AddressType").FieldType != newDriver.GetField("AddressType").FieldType)
                    {
                        changeTable(newDriver.GetField("AddressType").FieldType);
                    }
                }
                //更新driver
                m_driver = newDriver;
            }
            catch(Exception ex)
            {
                MyMessageBox.ShowErrorMessage(ex.Message);
            }
        }
        


        void loadConnDriver(ConnDriverSetting conndriver)
        {
            try
            {
                if (conndriver.ClassFile!=null)
                {
                    Assembly asm = Assembly.LoadFile(conndriver.ClassFile.FileName);
                    //需要设置变更
                    onDriverTypeChanged(asm.GetType(conndriver.ClassFile.ClassName));
                    tbxdriverclass.Text = conndriver.ClassFile.DriverName.ToString();
                }
                tbxid.Text = conndriver.Id.ToString();
                tbxname.Text = conndriver.Name;
                tbxconndriverreadinterval.Text = conndriver.ReadInterval.ToString();
                cbxconndriversendmode.SelectedIndex = conndriver.TransMode;
                cboconndriverreadmode.SelectedIndex = conndriver.ReadMode;
                if (conndriver.DriverSet != null)
                {
                    tbxconndriverreadinterval.Text = conndriver.DriverSet.ReadInterval.ToString();
                    cbxdriverreadmode.SelectedIndex = conndriver.DriverSet.ReadMode;
                    cbxdriversendmode.SelectedIndex = conndriver.DriverSet.TransMode;
                }
                //加载变量设置
                loadDataItems(conndriver);

            }
            catch(Exception ex)
            {
                MyMessageBox.ShowErrorMessage(ex.Message);
            }
        }

        void toAddNewRow()
        {
            DataRow dr = m_dt.NewRow();
            dr["Id"] =(int)m_dt.Compute("max(Id)", "true")+1;
            m_dt.Rows.Add(dr);
        }
        void toRemoveRow()
        {
            if (vwDataItem.SelectedCells != null&&vwDataItem.SelectedCells.Count>0)
            {
                DataRowView drv = (DataRowView)vwDataItem.Rows[vwDataItem.SelectedCells[0].RowIndex].DataBoundItem;
            }
        }
        void toSave()
        {
            //保存需要多项处理
            //类的获取
            //表格行的字符化处理

        }

    }
}
