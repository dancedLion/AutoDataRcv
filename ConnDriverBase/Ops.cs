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
namespace CHQ.RD.ConnDriverBase
{
    public class Ops
    {
        static string xmlfile = AppDomain.CurrentDomain.BaseDirectory + "connectorsettings.xml";
        static string dataitemsfile = AppDomain.CurrentDomain.BaseDirectory + "ConnectorDataItems.xml";
        static string logfile = AppDomain.CurrentDomain.BaseDirectory + "logs\\ConnectorBasetracelog.log";
        static string errorfile = AppDomain.CurrentDomain.BaseDirectory + "logs\\ConnectorBaseerrorlog.log";
        

        #region 驱动及驱动连接器
        public static string getConnDriverAttribute(int connId,string attType,string attName)
        {
            string ret = "";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlfile);
                XmlNodeList nodes = doc.DocumentElement.SelectNodes("ConnDrivers/ConnDriver[@Id=" + connId + "]/" + attType);
                if (nodes != null && nodes.Count > 0)
                {
                    ret = nodes[0].Attributes[attName].Value;
                }
                else
                {
                    throw new Exception("未找到指定节点！");
                }
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, "getConnDriverAttribute("+connId+";"+attType+";"+attName+"): Error" + ex.Message);
            }
            return ret;
        }

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
                        ConnDriverClass = Type.GetType(e.Attributes["ConnDriverClass"].Value),
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
                    set.ClassFile = getDriverClass(int.Parse(e.Attributes["AssemblieId"].Value));
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
                XmlNodeList nodes = doc.DocumentElement.SelectNodes("ConnDrivers/ConnDriver[@Id=" + connDriverId + "]");
                XmlNode pNode = null;
                if (nodes == null || nodes.Count == 0) throw new Exception("指定的驱动连接器ID不存在");
                pNode = nodes[0];
                //doc.GetElementsByTagName("ConnDriver");
                nodes = nodes[0].SelectNodes("Driver");
                if (nodes != null && nodes.Count > 1)
                {
                    for(int j = nodes.Count; j > 0; j--)
                    {
                        pNode.RemoveChild(nodes[j - 1]);
                    }
                }
                XmlElement elem = null;
                if (nodes == null||nodes.Count==0)
                {
                    elem = doc.CreateElement("Driver");
                    pNode.AppendChild(elem);
                }
                else
                {
                    elem = (XmlElement)nodes[0];
                    elem.SetAttribute("ReadInterval", setting.ReadInterval.ToString());
                    elem.SetAttribute("TransMode", setting.ReadMode.ToString());
                    elem.SetAttribute("TransMode", setting.TransMode.ToString());
                    elem.SetAttribute("Host", setting.Host);
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
                node.SetAttribute("ConnDriverClass", setting.ConnDriverClass.FullName);
                node.SetAttribute("AssemblieId",setting.ClassFile.Id.ToString());
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
                        ret.Name = e.Attributes["Name"].Value;
                        ret.ConnDriverClass = Type.GetType(e.Attributes["ConnDriverClass"].Value);
                        ret.ReadInterval = int.Parse(e.Attributes["ReadInterval"].Value);
                        ret.ReadMode = int.Parse(e.Attributes["ReadMode"].Value);
                        ret.TransMode = int.Parse(e.Attributes["TransMode"].Value);
                        ret.DriverSet = getDriverSetting(connDriverId);
                        ret.ClassFile = getDriverClass(int.Parse(e.Attributes["AssemblieId"].Value));
                    }
                }
                //获取数据项
                List<ConnectorDataItem> dataitems = new List<ConnectorDataItem>();
                DataTable tbitems= getConnDriverDataItems(connDriverId);
                foreach(DataRow dr in tbitems.Rows)
                {
                    dataitems.Add(new ConnectorDataItem
                    {
                        Id = int.Parse(dr["Id"].ToString()),
                        Name=dr["Name"].ToString(),
                        ConnId = int.Parse(dr["ConnId"].ToString()),
                        TransSig=dr["TransSig"].ToString(),
                        ValueType=dr["ValueType"].ToString(),
                        Address=dr["Address"].ToString()
                    });
                }
                ret.DataItems = dataitems;
            }
            catch (Exception ex)
            {
                TxtLogWriter.WriteErrorMessage("ConnectorBase.Ops.getConnDriverSetting(" + connDriverId.ToString() + "):" + ex.Message);
                ret = null;
            }
            return ret;
        }
        /// <summary>
        /// 删除驱动连接器设置
        /// </summary>
        /// <param name="connDriverId">整型，驱动连接器ID</param>
        /// <returns></returns>
        public static int removeConnDriverSetting(int connDriverId)
        {
            int ret = -1;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlfile);
                //TODO:判断数据项是否存在
                XmlDocument dataitems = new XmlDocument();
                doc.Load(dataitemsfile);
                XmlNodeList items = doc.GetElementsByTagName("DataItems");
                if (items[0].SelectNodes("Item[@ConnId=" + connDriverId.ToString() + "]") != null)
                {
                    throw new Exception("数据变量尚未处理！");
                }
                XmlNodeList nodes = doc.GetElementsByTagName("ConnDriver");
                foreach(XmlElement e in nodes)
                {
                    if (e.Attributes["Id"].Value == connDriverId.ToString())
                    {
                        e.ParentNode.RemoveChild(e);
                        ret = 0;
                        break;
                    }
                }
                doc.Save(xmlfile);
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, "RemoveConnDriverSetting(" + connDriverId.ToString() + "):" + ex.Message);
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
                    if (e.Attributes["Id"].Value == connDriverId.ToString())
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
                            ret.ReadMode = int.Parse(ec.Attributes["ReadMode"].Value);
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
                XmlNodeList nodes = doc.GetElementsByTagName("Assemblie");
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
                    XmlElement mele = doc.CreateElement("DriverAssemblies");
                    doc.DocumentElement.AppendChild(mele);
                    //doc.Save(xmlfile);
                }
                nodes = doc.GetElementsByTagName("DriverAssemblies");
                XmlElement ele = null;
                foreach (XmlElement e in nodes[0].ChildNodes)
                {
                    if(e.Attributes["ClassName"].Value==driverclass.ClassName||
                        e.Attributes["Id"].Value == driverclass.Id.ToString()||
                        e.Attributes["DriverName"].Value==driverclass.DriverName)
                    {
                        //throw new Exception("指定的类型或者名称或者ID已经存在！");
                        ele = (XmlElement)e;
                        break;
                    }
                }
                if (ele == null) { ele = doc.CreateElement("Assemblie");
                    nodes[0].AppendChild(ele);
                }
                ele.SetAttribute("Id", driverclass.Id.ToString());
                ele.SetAttribute("DriverName", driverclass.DriverName);
                ele.SetAttribute("ClassName", driverclass.ClassName);
                ele.SetAttribute("AssemblyInfo", driverclass.AssemblyInfo);
                ele.SetAttribute("FileName", driverclass.FileName);               
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
                //先找有没有被引用
                XmlNodeList conndrivers = doc.GetElementsByTagName("ConnDriver");
                foreach(XmlElement e in conndrivers)
                {
                    if (e.Attributes["AssemblieId"].Value == driverclass.Id.ToString())
                    {
                        throw new Exception("指定的驱动类型已经被引用，无法删除！");
                    }
                }
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
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlfile);
                XmlNodeList files = doc.GetElementsByTagName("ConnDriver");
                foreach(XmlElement e in files)
                {
                    if (e.Attributes["AssemblieId"].Value == olddriver.Id.ToString())
                    {
                        e.SetAttribute("AssemblieId", newdriver.Id.ToString());
                    }
                }
                doc.Save(xmlfile);
                //加入更新代码
                XmlNodeList asms = doc.GetElementsByTagName("Assemblie");
                foreach(XmlElement e in asms)
                {
                    if (e.Attributes["Id"].Value == olddriver.Id.ToString())
                    {
                        e.SetAttribute("Id", newdriver.Id.ToString());
                        e.SetAttribute("DriverName", newdriver.DriverName);
                        e.SetAttribute("FileName", newdriver.FileName);
                        e.SetAttribute("AssemblyInfo", newdriver.AssemblyInfo);
                        break;
                    }
                }
                doc.Save(xmlfile);
                ret = 0;
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
                doc.Load(dataitemsfile);
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
                            if(dataitems.Item(0).SelectNodes("Item[@Id=" + dr["Id"].ToString() + "]").Count>0)
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
                            if(dataitems.Item(0).SelectNodes("Item[@Id="+dr["Id"].ToString()+ "]").Count==0)
                            {
                                throw new Exception("该ID对应的驱动连接器ID并不存在，修改失败！");
                            }
                            else
                            {
                                data =(XmlElement)dataitems[0].SelectNodes("Item[@Id=" + dr["Id"].ToString() + " and @ConnId=" + connDriverId + "]").Item(0);
                                data.SetAttribute("Name", dr["name"].ToString());
                                data.SetAttribute("TransSig", dr["TransSig"].ToString());
                                data.SetAttribute("Address", dr["Address"].ToString());
                                data.SetAttribute("ValueType", dr["ValueType"].ToString());
                            }
                            break;
                        case DataRowState.Deleted:
                            if (dataitems.Item(0).SelectNodes("Item[@Id=" + dr["Id",DataRowVersion.Original].ToString()+ "]") == null)
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
                doc.Save(dataitemsfile);
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
                doc.Load(dataitemsfile);
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

        public static List<ConnectorDataItem> getConnectorDataItemList()
        {
            List<ConnectorDataItem> ret = new List<ConnectorDataItem>();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(dataitemsfile);
                XmlNodeList nodes = doc.GetElementsByTagName("DataItems");
                if (nodes != null)
                {
                    foreach (XmlElement e in nodes[0].ChildNodes) {
                        ConnectorDataItem item = new ConnectorDataItem
                        {
                            Id = int.Parse(e.Attributes["Id"].Value),
                            Name = e.Attributes["Name"].Value,
                            ConnId = int.Parse(e.Attributes["ConnId"].Value),
                            TransSig = e.Attributes["TransSig"].Value,
                            ValueType = e.Attributes["ValueType"].Value,
                            Address = e.Attributes["Address"].Value
                        };
                        ret.Add(item);
                    }
                }
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, "getConnectorDataItemList Error:" + ex.Message);
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
                doc.Load(dataitemsfile);
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

        //#region 连接管理器的操作
        ///// <summary>
        ///// 获取当前指定运行的连接管理器的ID
        ///// </summary>
        ///// <returns>0,-1，获取失败，连接管理器的ID</returns>
        //public static int getCurrentConnector()
        //{
        //    int ret = 0;
        //    try
        //    {
        //        XmlDocument doc = new XmlDocument();
        //        doc.Load(xmlfile);
        //        XmlNodeList node = doc.DocumentElement.SelectNodes("AppSettings");
        //        if (node == null || node.Count == 0)
        //        {
        //            XmlElement e = doc.CreateElement("AppSettings");
        //            doc.DocumentElement.AppendChild(e);
        //            XmlElement r = doc.CreateElement("RunningConnector");
        //            r.SetAttribute("Id", "");
        //            e.AppendChild(r);
        //            doc.Save(xmlfile);
        //            throw new Exception("未选择当前运行的连接管理器");
        //        }
        //        else
        //        {
        //            XmlNodeList rcs = node[0].SelectNodes("RunningConnector");
        //            if (rcs == null || rcs.Count == 0)
        //            {
        //                XmlElement e = doc.CreateElement("RunningConnector");
        //                e.SetAttribute("Id", "");
        //                node[0].AppendChild(e);
        //                doc.Save(xmlfile);
        //                throw new Exception("未选择当前运行的连接管理器");
        //            }
        //            else
        //            {
        //                ret = int.Parse(rcs[0].Attributes["Id"].Value);
        //            }
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        TxtLogWriter.WriteErrorMessage(errorfile, "getCurrentConnector Error:" + ex.Message);
        //        ret = -1;
        //    }
        //    return ret;
        //}
        ///// <summary>
        ///// 设置当前指定运行的连接管理器
        ///// </summary>
        ///// <param name="connectorId">连接管理器ID</param>
        ///// <returns>0-成功，1-失败</returns>
        //public static int saveCurrentConnector(int connectorId)
        //{
        //    int ret = 0;
        //    try
        //    {
        //        XmlDocument doc = new XmlDocument();
        //        doc.Load(xmlfile);
        //        XmlNodeList node = doc.DocumentElement.SelectNodes("AppSettings");
        //        XmlElement r = null;
        //        if (node == null || node.Count == 0)
        //        {
        //            XmlElement e = doc.CreateElement("AppSettings");
        //            doc.DocumentElement.AppendChild(e);
        //            r = doc.CreateElement("RunningConnector");
        //            r.SetAttribute("Id", "");
        //            e.AppendChild(r);
        //        }
        //        else
        //        {
        //            XmlNodeList rcs = node[0].SelectNodes("RunningConnector");
        //            if (rcs == null || rcs.Count == 0)
        //            {
        //                r = doc.CreateElement("RunningConnector");
        //                r.SetAttribute("Id", "");
        //                node[0].AppendChild(r);
        //            }
        //            else
        //            {
        //                r = (XmlElement)rcs[0];
        //            }
        //        }
        //        r.SetAttribute("Id", connectorId.ToString());
        //        doc.Save(xmlfile);
        //    }
        //    catch(Exception ex)
        //    {
        //        TxtLogWriter.WriteErrorMessage(errorfile, "saveCurrentConnector Error:" + ex.Message);
        //        ret = -1;
        //    }
        //    return ret;
        //}


        ///// <summary>
        ///// 准备连接管理器要素节点
        ///// </summary>
        ///// <returns></returns>
        //public static int prepareConnectorsNode()
        //{
        //    int ret = 0;
        //    try
        //    {
        //        XmlDocument doc = new XmlDocument();
        //        doc.Load(xmlfile);
        //        XmlNodeList nodes = doc.GetElementsByTagName("Connectors");
        //        if (nodes == null || nodes.Count == 0)
        //        {
        //            //不存在，则创建第一要素
        //            XmlElement e = doc.CreateElement("Connectors");
        //            doc.DocumentElement.AppendChild(e);
        //            //没有任何Connector，无需其他操作
        //            doc.Save(xmlfile);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TxtLogWriter.WriteErrorMessage(errorfile, "prepareSendingSettings Error:" + ex.Message);
        //        ret = -1;
        //    }
        //    return ret;
        //}
        ///// <summary>
        ///// 写连接管理器的设置
        ///// 该管理器的设置较多，暂时还未决定如何处理
        ///// </summary>
        ///// <param name="setting"></param>
        ///// <returns></returns>
        //public static int writeConnectorSetting(ConnectorSetting setting,string type)
        //{
        //    int ret = 0;
        //    try
        //    {
        //        XmlDocument doc = new XmlDocument();
        //        doc.Load(xmlfile);

        //    }
        //    catch(Exception ex)
        //    {
        //        ret = -1;

        //    }
        //    return ret;
        //}
        ///// <summary>
        ///// 获取连接管理器的设置
        ///// </summary>
        ///// <returns></returns>
        //public static ConnectorSetting getConnectorSetting(string type)
        //{
        //    return null;
        //}
        //#endregion

        //#region 发送数据设置
        

        ///// <summary>
        ///// 获取发送设置列表
        ///// </summary>
        ///// <returns>发送设置列表</returns>
        //public static List<DataSendingSet> getDataSendingList(int connectorId)
        //{
        //    List<DataSendingSet> ret = new List<DataSendingSet>();
        //    try
        //    {
        //        XmlDocument doc = new XmlDocument();
        //        doc.Load(xmlfile);
        //        XmlNodeList nodes = doc.DocumentElement.SelectNodes("Connectors/Connector[@Id=" + connectorId.ToString() + "]");
        //        if (nodes != null&&nodes.Count>0)
        //        {
        //            XmlNodeList sendings = nodes[0].SelectNodes("Sendings/Sending");
        //            if (sendings != null)
        //            {
        //                foreach (XmlElement e in sendings)
        //                {

        //                    DataSendingSet dss = new DataSendingSet
        //                    {
        //                        Id = int.Parse(e.Attributes["Id"].Value),
        //                        Host = e.Attributes["Host"].Value,
        //                        HostPort = int.Parse(e.Attributes["HostPort"].Value),
        //                        Name = e.Attributes["Name"].Value,
        //                        Memo = e.Attributes["Memo"].Value,
        //                        SendInterval = int.Parse(e.Attributes["SendInterval"].Value),
        //                        ConnDrivers = e.Attributes["ConnDrivers"].Value,
        //                        Via = int.Parse(e.Attributes["Via"].Value)
        //                    };
        //                    ret.Add(dss);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            throw new Exception("指定的连接管理器未设置！");
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        TxtLogWriter.WriteErrorMessage(errorfile, "getDataSendingList Error:" + ex.Message);
        //    }
        //    return ret;
        //}
        ///// <summary>
        ///// 保存数据发送设置
        ///// </summary>
        ///// <param name="connectorId">连接管理器ID</param>
        ///// <param name="dss">连接管理器设置，DataSendingSet类型</param>
        ///// <returns></returns>
        //public static int saveDataSending(int connectorId,DataSendingSet dss)
        //{
        //    int ret = 0;
        //    try
        //    {
        //        XmlDocument doc = new XmlDocument();
        //        doc.Load(xmlfile);
        //        XmlNodeList ctors = doc.DocumentElement.SelectNodes("Connectors/Connector[@Id=" + connectorId + "]");
        //        if (ctors != null && ctors.Count > 0)
        //        {
        //            XmlNodeList sendings = ctors[0].SelectNodes("Sendings");
        //            XmlElement e=null;
        //            if (sendings == null || sendings.Count == 0)
        //            {
        //                e = doc.CreateElement("Sendings");
        //                ctors[0].AppendChild(e);
        //            }
        //            else
        //            {
        //                e = (XmlElement)sendings[0];
        //            }
        //            sendings = e.SelectNodes("Sending[@Id=" + dss.Id + "]");
        //            XmlElement dse = null;
        //            if (sendings == null || sendings.Count == 0)
        //            {
        //                //新增
        //                dse = doc.CreateElement("Sending");
        //                dse.SetAttribute("Id", dss.Id.ToString());
        //            }
        //            else
        //            {
        //                //修改
        //                dse = (XmlElement)sendings[0];
        //            }
        //            //属性节点
        //            dse.SetAttribute("Via", dss.Via.ToString());
        //            dse.SetAttribute("Name", dss.Name);
        //            dse.SetAttribute("Host", dss.Host);
        //            dse.SetAttribute("HostPort", dss.HostPort.ToString());
        //            dse.SetAttribute("Memo", dss.Memo);
        //            dse.SetAttribute("SendInterval", dss.SendInterval.ToString());
        //            dse.SetAttribute("ConnDrivers", dss.ConnDrivers);
        //            doc.Save(xmlfile);
        //        }
        //        else
        //        {
        //            throw new Exception("指定的连接管理器未设置！");
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        TxtLogWriter.WriteErrorMessage(errorfile, "saveDataSening Error:" + ex.Message);
        //        ret = -1;
        //    }
        //    return ret;
        //}
        //#endregion

        //#region 本地数据报备设置
        //public static List<ConnectorLocalData> getConnectorLocalDataList()
        //{
        //    List<ConnectorLocalData> ret = new List<ConnectorLocalData>();
        //    try
        //    {
        //        XmlDocument doc = new XmlDocument();
        //        doc.Load(xmlfile);
        //        XmlNodeList nodes = doc.DocumentElement.SelectNodes("AppSettings/LocalDatas");
        //        if (nodes != null && nodes.Count > 0)
        //        {
        //            foreach(XmlElement e in nodes[0].ChildNodes)
        //            {
        //                ConnectorLocalData d = new ConnectorLocalData
        //                {
        //                    Id = int.Parse(e.Attributes["Id"].Value),
        //                    Desc = e.Attributes["Desc"].Value,
        //                    ConnectString = e.Attributes["ConnectString"].Value,
        //                    DBDriverType = int.Parse(e.Attributes["DBDriverType"].Value),
        //                    RDType=int.Parse(e.Attributes["RDType"].Value)
        //                };
        //                ret.Add(d);
        //            }
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        TxtLogWriter.WriteErrorMessage(errorfile, "getConnectorLocalDataList Error" + ex.Message);
        //    }
        //    return ret;
        //}
        //public static int saveConnectorLocalData(ConnectorLocalData cld)
        //{
        //    int ret = -1;
        //    try
        //    {
        //        XmlDocument doc = new XmlDocument();
        //        doc.Load(xmlfile);
        //        XmlNodeList nodes = doc.DocumentElement.SelectNodes("AppSettings/LocalDatas");
        //        XmlElement e=null;
        //        if (nodes == null || nodes.Count == 0)
        //        {
        //            XmlNodeList tmp = doc.DocumentElement.SelectNodes("AppSettings");
        //            if (tmp == null || tmp.Count == 0)
        //            {
        //                e = doc.CreateElement("AppSettings");
        //                doc.DocumentElement.AppendChild(e);
        //                XmlElement t = doc.CreateElement("LocalDatas");
        //                e.AppendChild(t);
        //                e = t;
        //                t = doc.CreateElement("LocalData");
        //                t.SetAttribute("Id", cld.Id.ToString());
        //                e.AppendChild(t);
        //                e = t;
        //            }
        //            else
        //            {
        //                e = (XmlElement)tmp[0];
        //                tmp = tmp[0].SelectNodes("LocalDatas");
        //                if (tmp == null || tmp.Count == 0)
        //                {
        //                    XmlElement t = doc.CreateElement("LocalDatas");
        //                    e.AppendChild(t);
        //                    e = t;
        //                    t = doc.CreateElement("LocalData");
        //                    t.SetAttribute("Id", cld.Id.ToString());
        //                    e = t;
        //                }
        //                else
        //                {
        //                    e = (XmlElement)tmp[0];
        //                    tmp = e.SelectNodes("LocalData[@Id=" + cld.Id + "]");
        //                    if (tmp == null || tmp.Count == 0)
        //                    {
        //                        XmlElement t = doc.CreateElement("LocalData");
        //                        t.SetAttribute("Id", cld.Id.ToString());
        //                        e.AppendChild(t);
        //                        e = t;
        //                    }
        //                    else
        //                    {
        //                        e = (XmlElement)tmp[0];
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            e =(XmlElement)nodes[0];
        //            nodes = e.SelectNodes("LocalData[@Id=" + cld.Id.ToString() + "]");
        //            if (nodes == null || nodes.Count == 0)
        //            {
        //                XmlElement t = doc.CreateElement("LocalData");
        //                t.SetAttribute("Id", cld.Id.ToString());
        //                e.AppendChild(t);
        //                e = t;
        //            }
        //            else
        //            {
        //                e = (XmlElement)nodes[0];
        //            }
        //        }
        //        e.SetAttribute("Desc", cld.Desc);
        //        e.SetAttribute("ConnectString", cld.ConnectString);
        //        e.SetAttribute("RDType", cld.RDType.ToString());
        //        e.SetAttribute("DBDriverType", cld.DBDriverType.ToString());
        //        doc.Save(xmlfile);
        //        ret = 0;
        //    }
        //    catch(Exception ex)
        //    {
        //        TxtLogWriter.WriteErrorMessage(errorfile, "saveConnectorLocalData Error("+cld.Desc+"):" + ex.Message);
        //    }
        //    return ret;
        //}
        //#endregion

        //#region 报警设置

        //#endregion

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
                TxtLogWriter.WriteErrorMessage(errorfile, "CreateClassDataTable(" + type.Name + "):" + ex.Message);
            }
            return ret;
        }

        public static object ParsingHost(Type hosttype,string hoststring)
        {
            object ret = null;
            try
            {
                ret = hosttype.Assembly.CreateInstance(hosttype.FullName);
                string[] keyvalues = hoststring.Split(';');
                for(int i = 0; i < keyvalues.Length; i++)
                {
                    string[] keyvalue = keyvalues[i].Split('=');
                    if (keyvalue.Length > 1)
                    {
                        hosttype.GetField(keyvalue[0]).SetValue(ret, keyvalue[1]);
                    }
                }
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, "ParsingHost(" + hosttype.Name + "," + hoststring + "):" + ex.Message);
            }
            return ret;
        }
        public static object ParsingAddress(Type addresstype,string address)
        {
            object ret = null;
            try
            {
                ret = addresstype.Assembly.CreateInstance(addresstype.FullName);
                string[] add = address.Split(';');
                FieldInfo[] flds = addresstype.GetFields();
                for (int i = 0; i < flds.Length; i++)
                {
                    for (int j = 0; j < add.Length; j++)
                    {
                        string[] keyvalue = add[j].Split('=');
                        if (keyvalue[0] == flds[i].Name && keyvalue.Length > 1)
                        {
                            if (typeof(System.Enum).IsAssignableFrom(flds[i].FieldType)) {
                                //TODO:以后修改此处，适应字符串保存的是啥值，目前可适用于数据行自动的值 
                                flds[i].SetValue(ret,int.Parse(keyvalue[1]));
                            }
                            else
                            {
                                flds[i].SetValue(ret, Convert.ChangeType(keyvalue[1], flds[i].FieldType));
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(errorfile, "ParsingAddress(" + addresstype.Name + "," + address + "):" + ex.Message);
            }
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
