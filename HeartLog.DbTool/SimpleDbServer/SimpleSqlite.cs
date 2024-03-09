using HeartLog.SimpleDbTool.Attribute;
using HeartLog.SimpleDbTool.Enum;
using HeartLog.SimpleDbTool.Interface;
using HeartLog.SimpleDbTool.SimpleORM;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace HeartLog.SimpleDbTool.SimpleDbServer
{
    [DbType(DatabaseType.Sqlite)]
    public class SimpleSqlite : ISimpleDbOperation, IDisposable
    {
        /// <summary>
        /// 连接对象
        /// </summary>

        private SQLiteConnection _sqlConn;

        /// <summary>
        /// 连接字符串
        /// </summary>
        private string _connStr;

        /// <summary>
        /// 对象查询
        /// </summary>
        public SQLiteConnection SqlConn
        {
            get
            {
                if (_sqlConn == null)
                {
                    try
                    {
                        _sqlConn = new SQLiteConnection(string.Format(_connStr));
                        return _sqlConn;
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
                return _sqlConn;
            }
        }

        public SimpleSqlite(string connStr)
        {
            _connStr = connStr;
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
            {
                return false;
            }

            var insertSql = SimpleOrmCore.GenerateInsertSql(t);
            rowsAffected = ExecuteNonQuery(insertSql);
            if (rowsAffected > 0)
            {
                return true;
            }

            return false;
        }

        public bool Insert<T>(T t) where T : ISimpleOrm
        {
            var sql = SimpleOrmCore.GenerateInsertSql(t);
            var rowsAffected = ExecuteNonQuery(sql);
            if (rowsAffected > 0)
            {
                return true;
            }

            return false;
        }
        public bool Delete<T>(T t) where T : ISimpleOrm
        {
            var sql = SimpleOrmCore.GenerateDeleteSql(t);
            var rowsAffected = ExecuteNonQuery(sql);
            if (rowsAffected <= 0)
            {
                return false;
            }

            return true;
        }

        public List<T> ExecuteReaderGetList<T>(T t) where T : ISimpleOrm, new()
        {
            return null;
        }

        public int ExecuteNonQuery(string commandText, CommandType commandType = CommandType.Text)
        {
            lock (SqlConn)
            {
                int result = 0;
                if (commandText == null || commandText.Length == 0)
                    throw new ArgumentNullException("commandText");
                SQLiteCommand cmd = new SQLiteCommand();
                SQLiteTransaction trans = null;
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

        private void PrepareCommand(SQLiteCommand cmd, SQLiteConnection conn, ref SQLiteTransaction trans, bool useTrans, CommandType cmdType, string cmdText, params SQLiteParameter[] cmdParms)
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
                foreach (SQLiteParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

        public object ExecuteScalar(string commandText, CommandType commandType = CommandType.Text)
        {
            if (commandText == null || commandText.Length == 0)
                throw new ArgumentNullException("commandText");

            SQLiteCommand cmd = new SQLiteCommand();
            SQLiteTransaction trans = null;
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
            using (SQLiteCommand sqlCmd = new SQLiteCommand())
            {
                sqlCmd.CommandText = sql;
                sqlCmd.Connection = SqlConn;

                if (SqlConn.State != ConnectionState.Open)
                {
                    SqlConn.Open();
                }

                var reader = sqlCmd.ExecuteReader();
                var dataList = ParseDbReader<T>(reader);
                SqlConn.Close();
                return dataList;
            }
        }

        public void Dispose()
        {
            SqlConn?.Dispose();
        }

        /// <summary>
        /// 解析DbDataReader
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
                                prop.SetValue(t, reader[colunAttr.ColumnName] == null ? "" : reader[colunAttr.ColumnName].ToString());
                            }
                            else if (colunAttr.ColumnType == ColumnType.INTEGER)
                            {
                                if (int.TryParse(reader[colunAttr.ColumnName].ToString(), out int intData))
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

        public List<T> Query<T>(T t) where T : ISimpleOrm, new()
        {
            var sql = SimpleOrmCore.GenerateSelectSql(t);
            return ExecuteReaderGetList<T>(sql);
        }
    }
}
