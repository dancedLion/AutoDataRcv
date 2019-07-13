using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
namespace CHQ.RD.ConnectorBase
{
    public class CDIronAndBasket:ConnDriverBase
    {
        string m_connectionstring = "";

        public CDIronAndBasket(int id,ConnectorBase host) : base(id,host)
        {
            m_connectionstring = Ops.getConnDriverAttribute(ID, "LocalDataStorage", "ConnectionString");
        }

        public override void onDataChanged(object sender, DataChangeEventArgs e)
        {
            //将数据写入到指定的库
            WriteLocalData(e);
            base.onDataChanged(sender, e);
        }
        void WriteLocalData(DataChangeEventArgs e)
        {
            using(SqlConnection sqlcn=new SqlConnection(m_connectionstring))
            {
                try
                {
                    SqlCommand sqlcmd = new SqlCommand();
                    sqlcmd.CommandText = "insert into il2_msg_to_tc(" +
                        "content_type,sp_data,msg_time,receive_time,sp_status,queue_no)" +
                        " values(@content,@data,@msg_time,@receive_time,1,0)";
                    sqlcmd.Parameters.Add(new SqlParameter("@content",
                        e.ItemId == 1 ? "IRON" : (e.ItemId == 2 ? "SCRA" : "ELEM")
                        ));
                    sqlcmd.Parameters.Add(new SqlParameter("@data", e.Value));
                    sqlcmd.Parameters.Add(new SqlParameter("@msg_time", DateTime.Now));
                    sqlcmd.Parameters.Add(new SqlParameter("@receive_time", DateTime.Now));
                    sqlcmd.Connection = sqlcn;
                    sqlcn.Open();
                    sqlcmd.ExecuteNonQuery();
                    sqlcn.Close();
                }
                catch(Exception ex)
                {
                    if (sqlcn.State == ConnectionState.Open) sqlcn.Close();
                    WriteErrorMessage(".WriteLocalDataError:"+ex.Message);
                }
            }
        }

    }
}
