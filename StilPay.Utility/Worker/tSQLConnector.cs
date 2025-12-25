using DocumentFormat.OpenXml.Office2010.Excel;
using StilPay.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace StilPay.Utility.Worker
{
    public class tSQLConnector
    {
        public readonly SqlConnection SqlConn = null;
        private SqlTransaction transaction = null;

        public tSQLConnector()
        {
            try
            {
                SqlConn = new SqlConnection();
                OpenConnection();
            }
            catch (Exception ex)
            {
                Log("tSQLConnection - " + ex.Message);
                CloseConnection();
                throw new Exception("Could not connect to SQL Server.");
            }
        }

        public void OpenConnection()
        {
            if (SqlConn.State != ConnectionState.Open)
            {
                SqlConn.ConnectionString = tAES3.Instance.Decrypt();
                SqlConn.Open();
                SetDateFormat();
            }
        }

        public void SetDateFormat()
        {
            using (var cmd = SqlConn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "set dateformat dmy";
                cmd.ExecuteNonQuery();
            }
        }

        public void BeginTransaction()
        {
            transaction = SqlConn.BeginTransaction();
        }

        public void CommitOrRollBackTransaction(Enums.TransactionType transactionType)
        {
            try
            {
                if (transaction != null && SqlConn != null)
                {
                    if (transactionType == Enums.TransactionType.Commit)
                        transaction.Commit();
                    else
                        transaction.Rollback();
                }
            }
            catch (Exception ex)
            {
                Log("CommitOrRollBackTransaction - " + ex.Message);
                throw new Exception("Bilinmeyen işlem hatası!");
            }
            finally
            {
                CloseConnection();
            }
        }

        public void CloseConnection()
        {
            if (transaction != null)
                transaction.Dispose();

            if (SqlConn != null)
            {
                if (SqlConn.State == ConnectionState.Open)
                    SqlConn.Close();

                SqlConn.Dispose();
            }
        }

        public string RunSqlCommand(string sp, List<FieldParameter> parameters)
        {
            try
            {
                string ID = null;

                using (var cmd = SqlConn.CreateCommand())
                {
                    if (transaction != null)
                        cmd.Transaction = transaction;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = sp;
                    cmd.Parameters.Add("@IDOut", SqlDbType.NVarChar, 50);
                    cmd.Parameters["@IDOut"].Direction = ParameterDirection.Output;
                    AddParametersToCommand(cmd, parameters);
                    cmd.ExecuteNonQuery();
                    ID = cmd.Parameters["@IDOut"].Value.ToString();
                }

                using (var cmd = SqlConn.CreateCommand())
                {
                    if (transaction != null)
                        cmd.Transaction = transaction;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "Loggers_Log";
                    string strParamsWithValue = "@IDOut=" + ID + ", " + GetParamsWithValue(parameters);
                    var LogPrmtrs = new List<FieldParameter>{
                        new FieldParameter("LogDate", Enums.FieldType.DateTime, DateTime.Now),
                        new FieldParameter("LogSP", Enums.FieldType.NVarChar, sp),
                        new FieldParameter("LogData", Enums.FieldType.NVarChar, strParamsWithValue),
                    };
                    AddParametersToCommand(cmd, LogPrmtrs);
                    cmd.ExecuteNonQuery();
                }

                return ID;
            }
            catch (Exception ex)
            {
                //Errors(sp, ex.Message, GetParamsWithValue(parameters));

                Log("RunSqlCommand - " + sp + " - " + ex.Message);
                if (ex.Message.Contains("FK"))
                    throw new Exception("İlişkisel veri kayıt hatası!");
                else if (ex.Message.Contains("UQ") || ex.Message.Contains("PK"))
                {
                    var fi = ex.Message.IndexOf('|');
                    var li = ex.Message.LastIndexOf('|');
                    string UQ = "";
                    if (fi >= 0 && li >= 0 && fi < li)
                    {
                        var m = ex.Message.Substring(fi + 1, li - fi - 1);
                        var s = m.Split('_');
                        if (s.Length > 0)
                            UQ = string.Concat(" (", s[s.Length - 1], ") ");
                    }

                    throw new Exception(string.Concat($"Tekrarlı veri kayıt hatası! --------------- {sp} ------------ {ex.Message}", UQ));
                }
                else if (ex.Message.Contains("MY"))
                {
                    string[] messages = ex.Message.Split('\n');
                    throw new Exception(messages[1].Replace("\r", ""));
                }
                else
                    throw new Exception("Bilinmeyen işlem hatası!");
            }
        }

        public DataSet GetDataSet(string sp, List<FieldParameter> parameters)
        {
            try
            {
                using (var cmd = SqlConn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = sp;
                    AddParametersToCommand(cmd, parameters);
                    var adptr = new SqlDataAdapter(cmd);
                    var ds = new DataSet();
                    adptr.Fill(ds);
                    return ds;
                }
            }
            catch (Exception ex)
            {
                Log("GetDataSet - " + sp + " - " + ex.Message);
                return new DataSet();
            }
            finally
            {
                CloseConnection();
            }
        }

        public DataTable GetDataTable(string sp, List<FieldParameter> parameters)
        {
            try
            {
                using (var cmd = SqlConn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = sp;
                    AddParametersToCommand(cmd, parameters);
                    var adptr = new SqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adptr.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                Log("GetDataTable - " + sp + " - " + ex.Message);
                return new DataTable();
            }
            finally
            {
                CloseConnection();
            }
        }

        public DataRow GetDataRow(string sp, List<FieldParameter> parameters)
        {
            try
            {
                using (var cmd = SqlConn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = sp;
                    AddParametersToCommand(cmd, parameters);
                    var adptr = new SqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adptr.Fill(dt);
                    if (dt.Rows.Count > 0)
                        return dt.Rows[0];
                    else
                        return null;
                }
            }
            catch (Exception ex)
            {
                Log("GetDataRow - " + sp + " - " + ex.Message);
                return null;
            }
            finally
            {
                CloseConnection();
            }
        }

        public string GetString(string sp, List<FieldParameter> parameters)
        {
            try
            {
                using (var cmd = SqlConn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = sp;
                    AddParametersToCommand(cmd, parameters);
                    string ret = (string)cmd.ExecuteScalar();
                    return ret;
                }
            }
            catch (Exception ex)
            {
                Log("GetString - " + sp + " - " + ex.Message);
                return null;
            }
            finally
            {
                CloseConnection();
            }
        }

        public long? GetInt64(string sp, List<FieldParameter> parameters)
        {
            try
            {
                using (var cmd = SqlConn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = sp;
                    AddParametersToCommand(cmd, parameters);
                    long ret = (long)cmd.ExecuteScalar();
                    return ret;
                }
            }
            catch (Exception ex)
            {
                Log("GetInt64 - " + sp + " - " + ex.Message);
                return null;
            }
            finally
            {
                CloseConnection();
            }
        }

        public int? GetInt32(string sp, List<FieldParameter> parameters)
        {
            try
            {
                using (var cmd = SqlConn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = sp;
                    AddParametersToCommand(cmd, parameters);
                    int ret = (int)cmd.ExecuteScalar();
                    return ret;
                }
            }
            catch (Exception ex)
            {
                Log("GetInt32 - " + sp + " - " + ex.Message);
                return null;
            }
            finally
            {
                CloseConnection();
            }
        }

        public short? GetInt16(string sp, List<FieldParameter> parameters)
        {
            try
            {
                using (var cmd = SqlConn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = sp;
                    AddParametersToCommand(cmd, parameters);
                    short ret = (short)cmd.ExecuteScalar();
                    return ret;
                }
            }
            catch (Exception ex)
            {
                Log("GetInt16 - " + sp + " - " + ex.Message);
                return null;
            }
            finally
            {
                CloseConnection();
            }
        }

        public decimal? GetDecimal(string sp, List<FieldParameter> parameters)
        {
            try
            {
                using (var cmd = SqlConn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = sp;
                    AddParametersToCommand(cmd, parameters);
                    decimal ret = (decimal)cmd.ExecuteScalar();
                    return ret;
                }
            }
            catch (Exception ex)
            {
                Log("GetDecimal - " + sp + " - " + ex.Message);
                return null;
            }
            finally
            {
                CloseConnection();
            }
        }

        public DateTime? GetDatetime(string sp, List<FieldParameter> parameters)
        {
            try
            {
                using (var cmd = SqlConn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = sp;
                    AddParametersToCommand(cmd, parameters);
                    DateTime ret = Convert.ToDateTime(cmd.ExecuteScalar());
                    return ret;
                }
            }
            catch (Exception ex)
            {
                Log("GetDatetime - " + sp + " - " + ex.Message);
                return null;
            }
            finally
            {
                CloseConnection();
            }
        }

        public bool? GetBoolean(string sp, List<FieldParameter> parameters)
        {
            try
            {
                bool result = true;

                using (var cmd = SqlConn.CreateCommand())
                {
                    if (transaction != null)
                        cmd.Transaction = transaction;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = sp;
                    AddParametersToCommand(cmd, parameters);

                    cmd.Parameters.Add("@Result", SqlDbType.Bit, 1);
                    cmd.Parameters["@Result"].Direction = ParameterDirection.Output;
                    cmd.ExecuteScalar();
                    result = (bool)cmd.Parameters["@Result"].Value;
                    return result;
                }
            }
            catch (Exception ex)
            {
                Log("GetBoolean - " + sp + " - " + ex.Message);
                return true;
            }
            finally
            {
                CloseConnection();
            }
        }

        public void AddParametersToCommand(SqlCommand cmd, List<FieldParameter> parameters)
        {
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    switch (parameter.FielType)
                    {
                        case Enums.FieldType.Int:
                            cmd.Parameters.Add(parameter.Name, SqlDbType.Int).Value = (parameter.Value == DBNull.Value || parameter.Value == null ? SqlInt32.Null : Convert.ToInt32(parameter.Value));
                            break;
                        case Enums.FieldType.SmallInt:
                            cmd.Parameters.Add(parameter.Name, SqlDbType.SmallInt).Value = (parameter.Value == DBNull.Value || parameter.Value == null ? SqlInt16.Null : Convert.ToInt16(parameter.Value));
                            break;
                        case Enums.FieldType.NVarChar:
                            cmd.Parameters.Add(parameter.Name, SqlDbType.NVarChar).Value = (parameter.Value == DBNull.Value || parameter.Value == null ? SqlString.Null : Convert.ToString(parameter.Value));
                            break;
                        case Enums.FieldType.Decimal:
                            cmd.Parameters.Add(parameter.Name, SqlDbType.Decimal).Value = (parameter.Value == DBNull.Value || parameter.Value == null ? SqlDecimal.Null : Convert.ToDecimal(parameter.Value));
                            break;
                        case Enums.FieldType.Tinyint:
                            cmd.Parameters.Add(parameter.Name, SqlDbType.TinyInt).Value = (parameter.Value == DBNull.Value || parameter.Value == null ? SqlByte.Null : Convert.ToByte(parameter.Value));
                            break;
                        case Enums.FieldType.Bit:
                            cmd.Parameters.Add(parameter.Name, SqlDbType.Bit).Value = (parameter.Value == DBNull.Value || parameter.Value == null ? SqlBoolean.Null : Convert.ToBoolean(parameter.Value));
                            break;
                        case Enums.FieldType.DateTime:
                            cmd.Parameters.Add(parameter.Name, SqlDbType.DateTime).Value = (parameter.Value == DBNull.Value || parameter.Value == null ? SqlDateTime.Null : Convert.ToDateTime(parameter.Value));
                            break;
                        case Enums.FieldType.VarBinary:
                            cmd.Parameters.Add(parameter.Name, SqlDbType.VarBinary).Value = (parameter.Value == DBNull.Value || parameter.Value == null ? SqlBinary.Null : (byte[])parameter.Value);
                            break;
                    }
                }
            }
        }

        public string GetParamsWithValue(List<FieldParameter> parameters)
        {
            string strParamsWithValue = "";

            foreach (var parameter in parameters)
                strParamsWithValue += parameter.Name + "=" + (parameter.Value == DBNull.Value ? "NULL" : parameter.Value) + ", ";

            return strParamsWithValue;
        }

        private static object _lockSQLLogTxt = new object();
        private void Log(string Exception)
        {
            //lock (_lockSQLLogTxt)
            //{
            //    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(System.Web.Hosting.HostingEnvironment.MapPath("~/Files/Documents/SQLLog.txt"), append: true))
            //    {
            //        sw.WriteLine(string.Format("Tarih:{0} Hata:{1}", DateTime.Now.ToString("dd.MM.yy-HH:mm:ss"), Exception));
            //    }
            //}
        }

        //private void Errors(string sp, string exception, string data)
        //{
        //    try
        //    {
        //        using (var cmd = SqlConn.CreateCommand())
        //        {
        //            if (transaction != null)
        //                cmd.Transaction = transaction;

        //            cmd.CommandText = "INSERT INTO Errors (LogDate, LogSP, LogData) VALUES (@LogDate, @LogSP, @LogData)";

        //            cmd.Parameters.Add("@LogDate", SqlDbType.DateTime).Value = DateTime.Now;
        //            cmd.Parameters.Add("@LogSP", SqlDbType.NVarChar).Value = sp;
        //            cmd.Parameters.Add("@LogData", SqlDbType.NVarChar).Value = "Exception: " + exception + ", Data: " + data;

        //            cmd.ExecuteNonQuery();

        //            transaction.Commit();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }

        //}
    }
}