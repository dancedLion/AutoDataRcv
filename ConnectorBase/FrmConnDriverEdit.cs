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
            initTable();
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
        public int AddNewConnDriver(AssemblyFile af)
        {
            return EditConnDriver(new ConnDriverSetting
            {
                Id = -1,
                ClassFile = af
            });
        }
        public int EditConnDriver(ConnDriverSetting conndriver)
        {
            loadConnDriver(conndriver);
            if (conndriver.Id != -1)
            {
                loadDataItems(conndriver);
            }
            else
            {
                tbxid.Text = "-1";
                
            }
            m_returnedvalue = conndriver;
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
            //值类型来自于某个枚举
            m_dt.Columns.Add("ValueType", typeof(string));
            m_dt.Columns.Add("Address", typeof(string));
            vwDataItem.DataSource = m_dt;
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
                Type addressType = null;
                if (conndriver.ClassFile != null)
                {
                    AssemblyFile file = Ops.getDriverClass(conndriver.ClassFile.Id);
                    Assembly asm = Assembly.LoadFile(file.FileName);
                    addressType = (Type)asm.GetType(file.ClassName).GetProperty("AddressType").GetValue(asm.CreateInstance(file.ClassName),null);
                }
                foreach (DataRow r in items.Rows)
                {

                    DataRow dr = m_dt.NewRow();
                    if (addressType != null)
                    {
                        object o = Ops.ParsingAddress(addressType, r["Address"].ToString());
                        FieldInfo[] flds = addressType.GetFields();
                        for (int i = 0; i < flds.Length; i++)
                        {
                            dr[flds[i].Name] = flds[i].GetValue(Convert.ChangeType(o, addressType));
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
                PropertyInfo addProps=null;
                if (m_driver != null) { addProps = m_driver.GetProperty("AddressType"); }
                if (m_driver != null && type == (Type)addProps.GetValue(m_driver.Assembly.CreateInstance(m_driver.FullName),null)) return;
                FieldInfo[] flds;
                //如果原类型不存在或者是地址类型不存在就不需要处理
                if (m_driver != null && (Type)addProps.GetValue(m_driver.Assembly.CreateInstance(m_driver.FullName), null)!= null)
                {
                    flds = ((Type)addProps.GetValue(m_driver.Assembly.CreateInstance(m_driver.FullName), null)).GetFields();
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
                        //vwDataItem.Columns.Remove(
                        //    vwDataItem.Columns[dc.ColumnName]
                        //    );
                    }
                }
                //添加新的driver的地址列
                flds = type.GetFields();
                for (int i = 0; i < flds.Length; i++)
                {
                    m_dt.Columns.Add(flds[i].Name, flds[i].FieldType);
                    //vwDataItem.Columns.Add(new DataGridViewColumn
                    //{
                    //    DataPropertyName = flds[i].Name,
                    //    Name = flds[i].Name
                    //});
                }
                //尝试解析当前ADDRESS对应的列的值 
                foreach (DataRow dr in m_dt.Rows)
                {
                    string adr = dr["Address"].ToString();
                    if (!string.IsNullOrEmpty(adr))
                    {
                        string[] padr = adr.Split(';');
                        for(int j = 0; j < padr.Length; j++)
                        {
                            string[] pair = padr[j].Split('=');
                            if (!string.IsNullOrEmpty(pair[1]))
                            {
                                for(int k = 0; k < flds.Length; k++)
                                {
                                    if (pair[0].ToUpper() == flds[k].Name.ToUpper())
                                    {
                                        try
                                        {
                                            dr[flds[k].Name] = Convert.ChangeType(pair[1], flds[k].FieldType);
                                        }
                                        catch
                                        {

                                        }
                                    }
                                }
                            }
                        }
                    }
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
                if (m_file==null||file.ClassName != m_file.ClassName)
                {
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
                    m_file = file;
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
            //PropertyInfo[] props=newDriver.GetProperties();
            PropertyInfo hostProps = newDriver.GetProperty("HostType");
            PropertyInfo addressProps = newDriver.GetProperty("AddressType");
            object o = newDriver.Assembly.CreateInstance(newDriver.FullName);
            try
            {
                if (m_driver == null)
                {
                    Type tp =(Type)addressProps.GetValue(o,null);
                    changeTable(tp);
                }
                else
                {
                    if (hostProps.GetValue(m_driver.Assembly.CreateInstance(m_driver.FullName),null) != 
                        hostProps.GetValue(o,null))    
         
                    {
                        tbxdriverhost.Text = "";
                    }
                    if (addressProps.GetValue(m_driver.Assembly.CreateInstance(m_driver.FullName), null) !=
                       addressProps.GetValue(o, null))
                    {
                        changeTable((Type)addressProps.GetValue(o, null));
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

        void toEditDriverHost()
        {
            if (m_driver != null)
            {
                PropertyInfo prop = m_driver.GetProperty("HostType");
                FrmDriverSettingEdit edit = new FrmDriverSettingEdit();
                if (edit.EditDriverSetting((Type)prop.GetValue(m_driver.Assembly.CreateInstance(m_driver.FullName),null),tbxdriverhost.Text) == 0)
                {
                    tbxdriverhost.Text = edit.ReturnedValue;
                }
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
                    m_file = conndriver.ClassFile;
                }
                tbxid.Text = conndriver.Id.ToString();
                tbxname.Text = conndriver.Name;
                tbxconndriverreadinterval.Text = conndriver.ReadInterval.ToString();
                cbxconndriversendmode.SelectedIndex = conndriver.TransMode;
                cboconndriverreadmode.SelectedIndex = conndriver.ReadMode;
                if (conndriver.DriverSet != null)
                {
                    tbxdriverreadinterval.Text = conndriver.DriverSet.ReadInterval.ToString();
                    cbxdriverreadmode.SelectedIndex = conndriver.DriverSet.ReadMode;
                    cbxdriversendmode.SelectedIndex = conndriver.DriverSet.TransMode;
                    //设置主机值 
                    tbxdriverhost.Text = conndriver.DriverSet.Host;
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
            int i = 0;
            try
            {
                //避免无行计算出错了
                i = (int)m_dt.Compute("max(Id)", "true");
            }
            catch
            {

            }
            dr["Id"] =i+1;
            dr["ConnId"] = int.Parse(tbxid.Text);
            m_dt.Rows.Add(dr);
        }
        void toRemoveRow()
        {
            if (vwDataItem.SelectedCells != null&&vwDataItem.SelectedCells.Count>0)
            {
                DataRowView drv = (DataRowView)vwDataItem.Rows[vwDataItem.SelectedCells[0].RowIndex].DataBoundItem;
            }
        }

        int formToConnDriver()
        {
            int ret = -1;
            try
            {
                if (string.IsNullOrEmpty(tbxid.Text) || tbxid.Text.Trim() == "-1")
                {
                    throw new Exception("未指定ID");
                }
                if (string.IsNullOrEmpty(tbxname.Text)) throw new Exception("未指定名称！");
                if (string.IsNullOrEmpty(tbxdriverclass.Text) || m_file == null)
                {
                    throw new Exception("未指定驱动！");
                }
                if (string.IsNullOrEmpty(tbxdriverhost.Text)) throw new Exception("设备未指定！");

                ConnDriverSetting m = new ConnDriverSetting();
                m.Id = int.Parse(tbxid.Text);
                m.Name = tbxname.Text;
                m.ClassFile = m_file;
                m.ReadInterval = int.Parse(tbxconndriverreadinterval.Text);
                m.ReadMode = cboconndriverreadmode.SelectedIndex;
                m.TransMode = cbxconndriversendmode.SelectedIndex;
                DriverSetting md = new DriverSetting
                {
                    Host = tbxdriverhost.Text,
                    ReadInterval = int.Parse(tbxdriverreadinterval.Text),
                    ReadMode = cbxdriverreadmode.SelectedIndex,
                    TransMode = cbxdriversendmode.SelectedIndex
                };
                m.DriverSet = md;
                //数据行的处理
                //m_driver的addresstype属性
                Type addressType=null;
                if (m_driver != null)
                {
                    PropertyInfo addressProp = m_driver.GetProperty("AddressType");
                    addressType = (Type)addressProp.GetValue(m_driver.Assembly.CreateInstance(m_driver.FullName),null);
                }
                if (addressType != null) {
                    foreach (DataRow dr in m_dt.Rows)
                    {
                        //根据类的数据表述来生成address字符串
                        if (dr.RowState == DataRowState.Added || dr.RowState == DataRowState.Modified)
                        {
                            m_result = 2;
                            //IHostDataAddress add = (IHostDataAddress)addressType.Assembly.CreateInstance(addressType.FullName);
                            FieldInfo[] addflds = addressType.GetFields();
                            //TODO:如何赋默认值
                            string s = "";
                            for(int i = 0; i < addflds.Length; i++)
                            {
                                s += addflds[i].Name + "="+dr[addflds[i].Name].ToString();
                                s += ((i == addflds.Length - 1) ? "" : ";");
                            }
                            dr["Address"] = s;
                        }
                    }
                }
                //检测多项对比，有值不同提示不同的保存值 
                FieldInfo[] flds = typeof(ConnDriverSetting).GetFields();
                for(int i = 0; i < flds.Length; i++)
                {
                    if (m_result == 1 || m_result == 3) break;
                    if (flds[i].FieldType == typeof(AssemblyFile)||flds[i].FieldType==typeof(DriverSetting))
                    {
                        FieldInfo[] subflds = typeof(AssemblyFile).GetFields();
                        for (int j = 0; j < subflds.Length; j++)
                        {
                            if (subflds[j].GetValue(m_returnedvalue) != subflds[j].GetValue(m))
                            {
                                if (m_result == 2||m_result==3)
                                {
                                    m_result = 3;
                                }
                                else
                                {
                                    m_result = 1;
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (flds[i].GetValue(m_returnedvalue) != flds[i].GetValue(m))
                        {
                            if (m_result == 2 || m_result == 3)
                            {
                                m_result = 3;
                            }
                            else
                            {
                                m_result = 1;
                            }
                            break;
                        }
                    }
                }
                m_returnedvalue = m;
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
            //预处理不成功则返回
            if (formToConnDriver() != 0) return;
            //保存需要多项处理
            //类的获取
            //表格行的字符化处理
            //保存表格2 保存前面的1 最多为3
            if (m_result == 1 || m_result == 3)
            {
                //保存主要设置
                if (Ops.writeConnDriverSetting(m_returnedvalue) != 0)
                {
                    MyMessageBox.ShowErrorMessage("保存主要设置失败，请查看日志！");
                }
                if (Ops.writeDriverSetting(m_returnedvalue.Id, m_returnedvalue.DriverSet) != 0)
                {
                    MyMessageBox.ShowErrorMessage("保存驱动设置失败！");
                }
            }
            if (m_result == 2 || m_result == 3)
            {
                //保存数据表设置
                if (Ops.saveConnectorDataItems(m_returnedvalue.Id, m_dt) != 0)
                {
                    MyMessageBox.ShowErrorMessage("保存变量失败，请查看日志！");
                }
            }
        }

        private void btnselectdriverclass_Click(object sender, EventArgs e)
        {
            toSelectDriver();
        }

        private void btneditdriversetting_Click(object sender, EventArgs e)
        {
            toEditDriverHost();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            toSave();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            toAddNewRow();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            toRemoveRow();
        }
    }
}
