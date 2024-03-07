using HeartLog.SimpleDbTool.Attribute;
using HeartLog.SimpleDbTool.Enum;
using HeartLog.SimpleDbTool.Interface;
using HeartLog.SimpleDbTool.SimpleORM;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace HeartLog.SimpleDbTool.SimpleDbServer
{

    [DbType(DatabaseType.SqlServer)]
    public class SimpleSqlServer : ISimpleDbOperation
    {
        private SqlConnection _sqlConn = null;

        private string _connectionString;
        public SqlConnection SqlConn
        {
            get
            {
                if (_sqlConn == null)
                {
                    _sqlConn = new SqlConnection(_connectionString);
                    return _sqlConn;
                }
                else
                    return _sqlConn;
            }
        }

        public SimpleSqlServer(string connecStr)
        {
            _connectionString = connecStr;
        }

        public bool Create<T>(T t) where T : ISimpleOrm
        {
            return false;
        }
        public bool Update<T>(T t) where T : ISimpleOrm
        {
            var sql = SimpleOrmCore.GenerateDeleteSql(t);
            var rowsAffected = ExecuteNonQuery(sql);
            if (rowsAffected <= 0)
                return false;

            var insertSql = SimpleOrmCore.GenerateInsertSql(t);
            rowsAffected = ExecuteNonQuery(insertSql);
            if (rowsAffected > 0)
                return true;

            return false;
        }

        public bool Insert<T>(T t) where T : ISimpleOrm
        {
            var sql = SimpleOrmCore.GenerateInsertSql(t);
            var rowsAffected = ExecuteNonQuery(sql);
            if (rowsAffected > 0)
                return true;
            return false;
        }
        public bool Delete<T>(T t) where T : ISimpleOrm
        {
            var sql = SimpleOrmCore.GenerateDeleteSql(t);
            var rowsAffected = ExecuteNonQuery(sql);
            if (rowsAffected <= 0)
                return false;
            return true;
        }

        public List<T> ExecuteReaderGetList<T>(T t) where T : ISimpleOrm, new()
        {
            using (SqlCommand _sqlCmd = new SqlCommand())
            {
                try
                {
                    _sqlCmd.CommandText = SimpleOrmCore.GenerateSelectSql<T>(t);
                    _sqlCmd.Connection = SqlConn;

                    if (SqlConn.State != ConnectionState.Open)
                    {
                        SqlConn.Open();
                    }
                    var reader = _sqlCmd.ExecuteReader();
                    var dataList = ParseDbReader<T>(reader);
                    SqlConn.Close();
                    return dataList;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
        private void PrepareCommand(SqlCommand cmd, SqlConnection conn, ref SqlTransaction trans, bool useTrans, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (useTrans)
            {
                trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                cmd.Transaction = trans;
            }
            cmd.CommandType = cmdType;
            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
        public DataTable QueryData()
        {
            return new DataTable();
        }
        public int ExecuteNonQuery(string commandText, CommandType commandType = CommandType.Text)
        {
            lock (SqlConn)
            {
                int result = 0;
                if (commandText == null || commandText.Length == 0)
                    throw new ArgumentNullException("commandText");
                SqlCommand cmd = new SqlCommand();
                SqlTransaction trans = null;
                PrepareCommand(cmd, SqlConn, ref trans, true, commandType, commandText);
                try
                {
                    result = cmd.ExecuteNonQuery();
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
                return result;
            }
        }
        public object ExecuteScalar(string commandText, CommandType commandType = CommandType.Text)
        {
            if (commandText == null || commandText.Length == 0)
                throw new ArgumentNullException("commandText");

            SqlCommand cmd = new SqlCommand();
            SqlTransaction trans = null;
            PrepareCommand(cmd, SqlConn, ref trans, false, commandType, commandText);

            try
            {
                var obj = cmd.ExecuteScalar();
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<T> ExecuteReaderGetList<T>(string sql) where T : ISimpleOrm, new()
        {
            using (SqlCommand _sqlCmd = new SqlCommand())
            {
                try
                {
                    _sqlCmd.CommandText = sql;
                    _sqlCmd.Connection = SqlConn;

                    if (SqlConn.State != ConnectionState.Open)
                    {
                        SqlConn.Open();
                    }
                    var reader = _sqlCmd.ExecuteReader();
                    var dataList = ParseDbReader<T>(reader);
                    SqlConn.Close();
                    return dataList;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
        public void Query()
        {

        }

        public void Dispose()
        {
            if (SqlConn != null)
            {
                SqlConn.Dispose();
            }
        }
        /// <summary>
        /// 根据特性反射，解析DbDataReader
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<T> ParseDbReader<T>(DbDataReader reader) where T : ISimpleOrm, new()
        {
            List<T> resultList = new List<T>();
            if (reader == null || reader.IsClosed)
                return resultList;
            try
            {
                while (reader.Read())
                {
                    T t = new T();
                    var type = t.GetType();
                    var props = type.GetProperties();
                    foreach (var prop in props)
                    {
                        var attrs = prop.GetCustomAttributes(typeof(ColumnAttribute), false);
                        if (attrs != null && attrs.Length > 0)
                        {
                            var colunAttr = (ColumnAttribute)attrs[0];
                            if (colunAttr.ColumnType == ColumnType.TEXT)
                            {
                                prop.SetValue(t, reader[prop.Name] == null ? "" : reader[prop.Name].ToString());
                            }
                            else if (colunAttr.ColumnType == ColumnType.INTEGER)
                            {
                                if (int.TryParse(reader[prop.Name].ToString(), out int intData))
                                {
                                    prop.SetValue(t, intData);
                                }
                                else
                                {
                                    prop.SetValue(t, 0);
                                }
                            }
                            else
                            {
                                prop.SetValue(t, null);
                            }
                        }
                    }
                    resultList.Add(t);
                }
                return resultList;
            }
            catch (Exception ex)
            {
                return resultList;
            }
        }
    }
}
