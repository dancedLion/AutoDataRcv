/******保命的：用于操作XML设置文件及其他作用
 * 日期：2019-3-9
 * 编码：徐震
 * 设置的内容：
 * 1-注册的驱动类型
 * 2-添加的驱动连接器设置
 * 3-更改的驱动设置
 * 4-连接管理器的设置
 * 5-变量列表的设置
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Net;
using CHQ.RD.DataContract;
using GeneralOPs;
using System.Data;
using System.Windows.Forms;
namespace CHQ.RD.ConnectorBase
{
    public class Ops
    {
        static string xmlfile = AppDomain.CurrentDomain.BaseDirectory + "\\connectorsettings.xml";
        static string logfile = AppDomain.CurrentDomain.BaseDirectory + "\\logs\\ConnectorBasetracelog.log";
        static string errorfile = AppDomain.CurrentDomain.BaseDirectory + "\\logs\\ConnectorBaseerrorlog.log";

        #region 驱动及驱动连接器
        /// <summary>
        /// 获取驱动连接器设置列表
        /// </summary>
        /// <returns>驱动连接器设置列表</returns>
        public static List<ConnDriverSetting> getConnDriverSettingList()
        {
            List<ConnDriverSetting> ret = new List<ConnDriverSetting>();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlfile);
                XmlNodeList settings = doc.GetElementsByTagName("ConnDriver");
                foreach(XmlElement e in settings)
                {
                    ConnDriverSetting set = new ConnDriverSetting
                    {
                        Id = int.Parse(e.Attributes["Id"].Value),
                        Name = e.Attributes["Name"].Value,
                        ReadMode = int.Parse(e.Attributes["ReadMode"].Value),
                        ReadInterval = int.Parse(e.Attributes["ReadInterval"].Value),
                        TransMode = int.Parse(e.Attributes["TransMode"].Value)
                    };
                    DriverSetting driver = new DriverSetting
                    {
                        Host=e.ChildNodes[0].Attributes["Host"].Value,
                        ReadMode=int.Parse(e.ChildNodes[0].Attributes["ReadMode"].Value),
                        ReadInterval=int.Parse(e.ChildNodes[0].Attributes["ReadInterval"].Value),
                        TransMode=int.Parse(e.ChildNodes[0].Attributes["TransMode"].Value)
                    };
                    set.DriverSet = driver;
                    set.ClassFile = getDriverClass(int.Parse(e.Attributes["AssemblyId"].Value));
                    ret.Add(set);
                }
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, "GetConnDriverList():" + ex.Message);
            }
            return ret;
        }
        /// <summary>
        /// 保存驱动设置（用于修改驱动设置）
        /// </summary>
        /// <param name="connDriverId"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static int writeDriverSetting(int connDriverId, DriverSetting setting)
        {
            int ret = 0;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlfile);
                XmlNodeList nodes = doc.GetElementsByTagName("ConnDriver");
                XmlNode node = null;
                foreach (XmlNode n in nodes)
                {
                    if (n.Attributes["Id"].Value == connDriverId.ToString())
                    {
                        node = n;
                        break;
                    }
                }
                if (node == null)
                {
                    throw new Exception("指定的驱动连接ID不存在！");
                }
                else
                {
                    //更新节点
                    if (node.ChildNodes.Count == 1)
                    {
                        node.ChildNodes[0].Attributes["ReadInterval"].Value = setting.ReadInterval.ToString();
                        node.ChildNodes[0].Attributes["ReadMode"].Value = setting.ReadMode.ToString();
                        node.ChildNodes[0].Attributes["TransMode"].Value = setting.TransMode.ToString();
                        node.ChildNodes[0].Attributes["Host"].Value = setting.Host;
                    }
                    else
                    {
                        while (node.HasChildNodes)
                        {
                            node.RemoveChild(node.FirstChild);
                        }
                        XmlElement elem = doc.CreateElement("Driver");
                        elem.SetAttribute("ReadInterval", setting.ReadInterval.ToString());
                        elem.SetAttribute("TransMode", setting.ReadMode.ToString());
                        elem.SetAttribute("TransMode", setting.TransMode.ToString());
                        elem.SetAttribute("Host", setting.Host);
                        node.AppendChild(elem);
                    }
                }
                doc.Save(xmlfile);
            }
            catch (Exception ex)
            {
                TxtLogWriter.WriteErrorMessage("XMLwriteError(writeDriverSetting:" + connDriverId.ToString() + ":" + setting.ToString() + "):" + ex.Message);
                ret = -1;
            }
            return ret;
        }
        /// <summary>
        /// 保存驱动连接器设置（用于修改或新增连接器设置）
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static int writeConnDriverSetting(ConnDriverSetting setting)
        {
            int ret = 0;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlfile);
                //是否存在Conndrivers元素
                XmlNodeList cd = doc.GetElementsByTagName("ConnDrivers");
                if (cd == null)
                {
                    XmlElement cds = doc.CreateElement("ConnDrivers");
                    doc.DocumentElement.AppendChild(cds);
                    doc.Save(xmlfile);
                }
                cd = doc.GetElementsByTagName("ConnDrivers");
                //添加东西
                XmlElement node = null;
                foreach (XmlElement n in cd[0].ChildNodes)
                {
                    if (n.Attributes["Id"].Value == setting.Id.ToString())
                    {
                        node = n;
                        break;
                    }
                }
                if (node == null)
                {
                    //添加节点
                    XmlElement elem = doc.CreateElement("ConnDriver");
                    node = elem;
                    cd[0].AppendChild(node);
                }
                //更新节点信息
                node.SetAttribute("Id", setting.Id.ToString());
                //TODO: 类型如何处理
                //node.SetAttribute("", );
                node.SetAttribute("ReadInterval", setting.ReadInterval.ToString());
                node.SetAttribute("ReadMode", setting.ReadMode.ToString());
                node.SetAttribute("TransMode", setting.TransMode.ToString());
                node.SetAttribute("Name", setting.Name);
                doc.Save(xmlfile);
                ret = writeDriverSetting(int.Parse(node.Attributes["Id"].Value), setting.DriverSet);
            }
            catch (Exception ex)
            {
                ret = -1;
                TxtLogWriter.WriteErrorMessage("ConnectBase.Ops.writeConnDriverSetting(" + setting.ToString() + "):" + ex.Message);
            }
            return ret;
        }
        /// <summary>
        /// 获取驱动连接器设置
        /// </summary>
        /// <param name="connDriverId"></param>
        /// <returns></returns>
        public static ConnDriverSetting getConnDriverSetting(int connDriverId)
        {
            ConnDriverSetting ret = null;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlfile);
                XmlNodeList nodes = doc.GetElementsByTagName("ConnDriver");
                foreach (XmlElement e in nodes)
                {
                    if (e.Attributes["Id"].Value == connDriverId.ToString())
                    {
                        ret = new ConnDriverSetting();
                        ret.Id = connDriverId;
                        //TODO: 驱动连接器数据更新，类型如何处理
                        //ret.DriverType = e.Attributes["Type"].Value;
                        ret.Name = e.Attributes["Name"].Value;
                        ret.ReadInterval = int.Parse(e.Attributes["ReadInterval"].Value);
                        ret.ReadMode = int.Parse(e.Attributes["ReadMode"].Value);
                        ret.TransMode = int.Parse(e.Attributes["TransMode"].Value);
                        ret.DriverSet = getDriverSetting(connDriverId);
                    }
                }
            }
            catch (Exception ex)
            {
                TxtLogWriter.WriteErrorMessage("ConnectorBase.Ops.getConnDriverSetting(" + connDriverId.ToString() + "):" + ex.Message);
                ret = null;
            }
            return ret;
        }
        /// <summary>
        /// 获取驱动设置
        /// </summary>
        /// <param name="connDriverId"></param>
        /// <returns></returns>
        public static DriverSetting getDriverSetting(int connDriverId)
        {
            DriverSetting ret = null;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlfile);
                XmlNodeList nodes = doc.GetElementsByTagName("ConnDriver");
                foreach (XmlElement e in nodes)
                {
                    if (e.Attributes["Id"].ToString() == connDriverId.ToString())
                    {
                        foreach (XmlElement ec in e.ChildNodes)
                        {
                            if (ec.LocalName != "Driver")
                            {
                                continue;
                            }
                            ret = new DriverSetting();
                            ret.Host = ec.Attributes["Host"].Value;
                            ret.ReadInterval = int.Parse(ec.Attributes["ReadInterval"].Value);
                            ret.ReadMode = int.Parse(ec.Attributes["ReadMove"].Value);
                            ret.TransMode = int.Parse(ec.Attributes["TransMode"].Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TxtLogWriter.WriteErrorMessage("Ops.getDriverSetting(" + connDriverId.ToString() + "):" + ex.Message);
                ret = null;
            }
            return ret;
        }
        /// <summary>
        /// 获取所有注册的驱动类型的列表
        /// </summary>
        /// <returns></returns>
        public static List<AssemblyFile> getDriverClassList()
        {
            List<AssemblyFile> ret = new List<AssemblyFile>();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlfile);
                XmlNodeList nodes = doc.GetElementsByTagName("DriverAssemblies");
                if (nodes.Count > 0)
                {
                    foreach (XmlElement e in nodes[0].ChildNodes)
                    {
                        ret.Add(new AssemblyFile
                        {
                            Id = int.Parse(e.Attributes["Id"].Value),
                            DriverName = e.Attributes["DriverName"].Value,
                            ClassName = e.Attributes["ClassName"].Value,
                            AssemblyInfo = e.Attributes["AssemblyInfo"].Value,
                            FileName = e.Attributes["FileName"].Value
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, "GetDriverClassList:" + ex.Message);
            }
            return ret;
        }
        /// <summary>
        /// 根据ID获取注册的驱动类型信息
        /// 运用于驱动连接器获取类信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static AssemblyFile getDriverClass(int id)
        {
            AssemblyFile ret = null;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlfile);
                XmlNodeList nodes = doc.GetElementsByTagName("Assemblies");
                foreach(XmlElement e in nodes)
                {
                    if (e.Attributes["Id"].Value == id.ToString())
                    {
                        ret = new AssemblyFile
                        {
                            Id = id,
                            ClassName = e.Attributes["ClassName"].Value,
                            DriverName = e.Attributes["DriverName"].Value,
                            AssemblyInfo = e.Attributes["AssemblyInfo"].Value,
                            FileName = e.Attributes["FileName"].Value
                        };
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, "getDriverClass(" + id.ToString() + "):" + ex.Message);
            }
            return ret;
        }
        /// <summary>
        /// 根据注册的驱动类型获得及程序中的类型
        /// </summary>
        /// <param name="driverclass"></param>
        /// <returns></returns>
        public static Type getType(AssemblyFile driverclass)
        {
            Type ret = null;
            try
            {
                Assembly asm = Assembly.LoadFile(driverclass.FileName);
                ret = asm.GetType(driverclass.ClassName);
            }
            catch (Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, "getType(" + driverclass.ClassName + "):" + ex.Message);
            }
            return ret;
        }
        /// <summary>
        /// 保存驱动类型（新增)
        /// </summary>
        /// <param name="driverclass">AssemblyFile类型</param>
        /// <returns>0-成功</returns>
        public static int saveDriverClass(AssemblyFile driverclass)
        {
            int ret = -1;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlfile);
                XmlNodeList nodes = doc.GetElementsByTagName("DriverAssemblies");
                if (nodes.Count == 0)
                {
                    XmlElement ele = doc.CreateElement("DriverAssemblies");
                    doc.DocumentElement.AppendChild(ele);
                }
                nodes = doc.GetElementsByTagName("DriverAssemblies");
                foreach (XmlElement e in nodes[0].ChildNodes)
                {
                    if(e.Attributes["ClassName"].Value==driverclass.ClassName||
                        e.Attributes["Id"].Value == driverclass.Id.ToString()||
                        e.Attributes["DriverName"].Value==driverclass.DriverName)
                    {
                        throw new Exception("指定的类型或者名称或者ID已经存在！");
                    }
                }
                XmlElement newele = doc.CreateElement("Assemblie");
                newele.SetAttribute("Id", driverclass.Id.ToString());
                newele.SetAttribute("DriverName", driverclass.DriverName);
                newele.SetAttribute("ClassName", driverclass.ClassName);
                newele.SetAttribute("AssemblyInfo", driverclass.AssemblyInfo);
                newele.SetAttribute("FileName", driverclass.FileName);
                nodes[0].AppendChild(newele);
                doc.Save(xmlfile);
                ret = 0;
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, "SaveDriverClass(" + driverclass.ClassName + "):" + ex.Message);
            }
            return ret;
        }
        /// <summary>
        /// 删除驱动类型
        /// </summary>
        /// <param name="driverclass"></param>
        /// <returns></returns>
        public static int removeDriverClass(AssemblyFile driverclass)
        {
            int ret = -1;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlfile);
                XmlNodeList nodes = doc.GetElementsByTagName("DriverAssemblies");
                bool isfound = false;
                //TODO:先找有没有被引用

                //再找是否存在
                isfound = false;
                foreach(XmlElement e in nodes[0].ChildNodes)
                {
                    if(e.Attributes["Id"].Value==driverclass.Id.ToString()&&
                        e.Attributes["DriverName"].Value==driverclass.DriverName&&
                        e.Attributes["ClassName"].Value == driverclass.ClassName)
                    {
                        nodes[0].RemoveChild(e);
                        isfound = true;
                        break;
                    }
                }
                if (isfound)
                {
                    doc.Save(xmlfile);
                    ret = 0;
                }
                else
                {
                    throw new Exception("指定的类型未找到！");
                }
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, "RemoveDriverClass(" + driverclass.ClassName + "):" + ex.Message);
            }
            return ret;
        }
        /// <summary>
        /// 保存编辑的驳动类型，类型无法更改
        /// </summary>
        /// <param name="olddriver">旧的类型，用于获取其ID</param>
        /// <param name="newdriver">新的类型，用于保存ID和名称</param>
        /// <returns></returns>
        public static int updateDriverClass(AssemblyFile olddriver, AssemblyFile newdriver)
        {
            int ret = -1;
            //已引用的要变更为新的ID
            try
            {
                //TODO:加入更新代码
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, "UpdateDriverClass(" + newdriver.ClassName + "):" + ex.Message);
            }
            return ret;
        }
        #endregion

        #region 数据项操作
        /// <summary>
        /// 保存传入的驱动连接器设置的数据项
        /// </summary>
        /// <param name="connDriverId">驱动连接器ID，用于双验证，即ID和驱动连接器ID都必须与数据表相符</param>
        /// <param name="items">数据项表</param>
        /// <returns></returns>
        public static int saveConnectorDataItems(int connDriverId,DataTable items)
        {
            int ret = 0;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlfile);
                //是否有dataitems页签
                XmlNodeList dataitems = doc.GetElementsByTagName("DataItems");
                if (dataitems == null)
                {
                    XmlElement elem = doc.CreateElement("DataItems");
                    doc.DocumentElement.AppendChild(elem);
                    doc.Save(xmlfile);
                    dataitems = doc.GetElementsByTagName("DataItems");
                }
                //开始保存操作;
                //遍历数据表，新增的、修改的、删除的
                foreach(DataRow dr in items.Rows)
                {
                    XmlElement data;
                    switch (dr.RowState)
                    {
                        case DataRowState.Added:
                            //查检是否存在同一ID
                            if(dataitems.Item(0).SelectNodes("Item[@Id=" + dr["Id"].ToString() + "]") != null)
                            {
                                throw new Exception("指定ID的数据项已经存在，不能新增相同ID的数据项");
                            }
                            data = doc.CreateElement("Item");
                            dataitems[0].AppendChild(data);
                            data.SetAttribute("Id", dr["id"].ToString());
                            data.SetAttribute("Name", dr["name"].ToString());
                            data.SetAttribute("ConnId", dr["ConnId"].ToString());
                            data.SetAttribute("TransSig", dr["TransSig"].ToString());
                            data.SetAttribute("Address", dr["Address"].ToString());
                            data.SetAttribute("ValueType", dr["ValueType"].ToString());
                            break;
                        case DataRowState.Modified:
                            if(dataitems.Item(0).SelectNodes("Item[@Id="+dr["Id"].ToString()+" and @ConnId=" + connDriverId + "]") == null)
                            {
                                throw new Exception("该ID对应的驱动连接器ID并不存在，修改失败！");
                            }
                            else
                            {
                                data =(XmlElement)dataitems[0].SelectNodes("Item[@Id=" + dr["id"].ToString() + "]").Item(0).FirstChild;
                                data.SetAttribute("Name", dr["name"].ToString());
                                data.SetAttribute("TransSig", dr["TransSig"].ToString());
                                data.SetAttribute("Address", dr["Address"].ToString());
                                data.SetAttribute("ValueType", dr["ValueType"].ToString());
                            }
                            break;
                        case DataRowState.Deleted:
                            if (dataitems.Item(0).SelectNodes("Item[@Id=" + dr["Id"].ToString() + " and @ConnId=" + connDriverId + "]") == null)
                            {
                                throw new Exception("该ID对应的驱动连接器ID并不存在，删除失败！");
                            }
                            else
                            {
                                dataitems.Item(0).RemoveChild(
                                    dataitems.Item(0).SelectSingleNode("Item[@Id=" + dr["Id", DataRowVersion.Original].ToString() + "]")
                                    );
                            }
                            break;
                    }
                }
                doc.Save(xmlfile);
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage("Ops.saveConnectorDataItems():" + ex.Message);
                ret = -1;
            }
            return ret;
        }
        /// <summary>
        /// 返回连接管理器的所有数据变量列表
        /// </summary>
        /// <returns></returns>
        public static DataTable getConnectorDataItems()
        {
            DataTable ret = CreateClassDataTable(typeof(ConnectorDataItem));
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlfile);
                XmlNodeList nodes = doc.GetElementsByTagName("DataItems");
                if (nodes != null)
                {
                    foreach (XmlElement e in nodes[0].ChildNodes)
                    {

                        DataRow dr = DatanodeToRow(ret, e);
                        ret.Rows.Add(dr);
                    }
                    //接受修改，实际的数据为最终状态
                    ret.AcceptChanges();
                }
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage("Ops.getConnectorDataItems():" + ex.Message);
            }
            return ret;
        }
        /// <summary>
        /// 根据驱动连接器的ID返回所有该驱动连接器下的变量
        /// </summary>
        /// <param name="connDriverId"></param>
        /// <returns></returns>
        public static DataTable getConnDriverDataItems(int connDriverId)
        {
            DataTable ret = CreateClassDataTable(typeof(ConnectorDataItem));
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlfile);
                XmlNodeList nodes = doc.GetElementsByTagName("DataItems");
                if (nodes != null)
                {
                    foreach(XmlElement e in nodes[0].ChildNodes)
                    {
                        if (e.Attributes["ConnId"].Value == connDriverId.ToString())
                        {
                            DataRow dr = DatanodeToRow(ret, e);
                            ret.Rows.Add(dr);
                        }
                    }
                    ret.AcceptChanges();
                }
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage("Ops.getConnDriverDataItems(" + connDriverId.ToString() + "):" + ex.Message);
            }
            return ret;
        }
        /// <summary>
        /// 连接管理器数据变量的节点转化为数据行
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        static DataRow DatanodeToRow(DataTable dt, XmlElement e)
        {
            DataRow dr = dt.NewRow();
            dr["Id"] = int.Parse(e.Attributes["Id"].Value);
            dr["Name"] = e.Attributes["Name"].Value;
            dr["ConnId"] = int.Parse(e.Attributes["ConnId"].Value);
            dr["TransSig"] = e.Attributes["TransSig"].Value;
            dr["Address"] = e.Attributes["Address"].Value;
            dr["ValueType"] = e.Attributes["ValueType"].Value;
            return dr;
        }
        /// <summary>
        /// 将数据表行写入到数据节点中
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        static XmlElement RowToDataItemElement(DataRow dr,XmlElement e)
        {
            e.SetAttribute("Id", dr["Id"].ToString());
            e.SetAttribute("Name",dr["Name"].ToString());
            e.SetAttribute("ConnId", dr["ConnId"].ToString());
            e.SetAttribute("TransSig", dr["TransSi"].ToString());
            e.SetAttribute("Address", dr["address"].ToString());
            e.SetAttribute("ValueType", dr["valuetype"].ToString());
            return e;
        }
        #endregion

        #region 连接管理器的操作
        /// <summary>
        /// 写连接管理器的设置
        /// 该管理器的设置较多，暂时还未决定如何处理
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static int writeConnectorSetting(ConnectorSetting setting,string type)
        {
            int ret = 0;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlfile);

            }
            catch(Exception ex)
            {
                ret = -1;

            }
            return ret;
        }
        /// <summary>
        /// 获取连接管理器的设置
        /// </summary>
        /// <returns></returns>
        public static ConnectorSetting getConnectorSetting(string type)
        {
            return null;
        }
        #endregion

        #region 其他一些方法

        /// <summary>
        /// 根据数据定义类来创建数据表
        /// 适用于Field，而不是Properties
        /// </summary>
        /// <param name="type">类名</param>
        /// <returns>数据表</returns>
        public static DataTable CreateClassDataTable(Type type)
        {
            DataTable ret = new DataTable(type.Name);
            try {
                FieldInfo[] flds = type.GetFields();
                if (flds != null)
                {
                    for(int i = 0; i < flds.Length; i++)
                    {
                        if(flds[i].FieldType.IsGenericType && flds[i].FieldType.GetGenericTypeDefinition() == typeof( Nullable<>)){
                            ret.Columns.Add(flds[i].Name, Nullable.GetUnderlyingType(flds[i].FieldType));
                        }
                        else
                        {
                            ret.Columns.Add(flds[i].Name, flds[i].FieldType);
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }
            return ret;
        }

        public static object ParsingHost(Type hosttype,string hoststring)
        {
            object ret = null;
            return ret;
        }
        public static object ParsingAddress(Type addresstype,string address)
        {
            object ret = null;
            return ret;
        }

        #endregion
    }


    public class MyMessageBox { 
        public static DialogResult ShowTipMessage(string message)
        {
            return MessageBox.Show(message, "提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }
        public static DialogResult ShowErrorMessage(string message)
        {
            return MessageBox.Show(message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static DialogResult ShowSelectionMessage(string message)
        {
            return MessageBox.Show(message, "选择", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }
    }
}
