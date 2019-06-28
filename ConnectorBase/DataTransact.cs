using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using GeneralOPs;
using CHQ.RD.DataContract;
namespace CHQ.RD.ConnectorBase
{
    public class VariableDeclare
    {
        public static string errorfile = AppDomain.CurrentDomain.BaseDirectory + "logs\\DataTransactError.log";
        public static List<ConnectorDataItem> AllItems = Ops.getConnectorDataItemList();
        public static string TypeConvert(string sourceType)
        {
            string targetType = "VarChar";
            switch (sourceType.ToUpper())
            {
                case "INT":
                case "BIT":
                case "INT16":
                case "INT32":
                    targetType = "Int";
                    break;
                case "TEXT":
                    break;
                case "REAL":
                    targetType = "Float";
                    break;
            }
            return targetType;
        }
    }

    public class DataTransact
    {
    }
    
    public class SqlRealTimeDataTransact
    {

        public SqlRealTimeDataTransact(string connectionString)
        {
            sqlcn = new SqlConnection(connectionString);
        }
        SqlConnection sqlcn = null;
        SqlCommand cmd = null;
        int prepareTable()
        {
            int ret = 1;
            try
            {
                string sql = "select count(*) from sysobjects where name='RD'";
                sqlcn.Open();
                SqlCommand sqlcmd = new SqlCommand(sql, sqlcn);
                if (int.Parse(sqlcmd.ExecuteScalar().ToString()) == 0)
                {
                    sql = "Create table rd(" +
                        "VarId int not null unique," +
                        "VarValue sql_variant," +
                        "lastTime datetime)";
                    sqlcmd.CommandText = sql;
                    sqlcmd.ExecuteNonQuery();
                }
                sqlcn.Close();
            }
            catch (Exception ex)
            {
                if (sqlcn.State == ConnectionState.Open) sqlcn.Close();
                TxtLogWriter.WriteErrorMessage(VariableDeclare.errorfile,
                    this.GetType().ToString() + ".prepareTable Error:" + ex.Message);
                ret = -1;
            }
            return ret;
        }

        public int startDataTransact()
        {
            int ret = 0;
            try
            {
                if (prepareTable() > -1)
                {
                    sqlcn.Open();
                    cmd = new SqlCommand();
                    cmd.Connection = sqlcn;
                    cmd.CommandText = "sp_x_tranRealData";
                    cmd.CommandType = CommandType.StoredProcedure;
                }
                else
                {
                    ret = -1;
                }
            }
            catch (Exception ex)
            {
                ret = -1;
                TxtLogWriter.WriteErrorMessage(VariableDeclare.errorfile,
                    this.GetType().ToString() + ".startTransactData Error:" + ex.Message);
            }
            return ret;
        }
        public int stopDataTransact()
        {
            int ret = 0;
            try
            {
                cmd.Dispose();
                sqlcn.Close();
            }
            catch (Exception ex)
            {
                ret = -1;
                TxtLogWriter.WriteErrorMessage(VariableDeclare.errorfile,
                    this.GetType().ToString() + ".stopTransactData Error:" + ex.Message);
            }
            return ret;
        }
        public void TransactData(int itemid,object value)
        {
            try
            {
                SqlParameter pid = new SqlParameter("@id", itemid);
                SqlParameter pvalue = new SqlParameter("@value", value);
                cmd.Parameters.Clear();
                cmd.Parameters.Add(pid);
                cmd.Parameters.Add(pvalue);
                cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                TxtLogWriter.WriteErrorMessage(VariableDeclare.errorfile,
                    this.GetType().ToString() + ".TransactData Error:" + ex.Message);
            }
        }

    }
}
