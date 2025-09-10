using System;
using SqlSugar;

namespace Log2Html.Dao
{
    public class DbUtils
    {
        private string _connStr;
        private DbType _dbType;

        /// <summary>
        /// DB helper
        /// </summary>
        public SqlSugarClient Db
        {
            get => new SqlSugarClient(
                new ConnectionConfig()
                {
                    ConnectionString = _connStr,
                    DbType = _dbType,
                    IsAutoCloseConnection = true,
                    InitKeyType = InitKeyType.Attribute
                },
                db =>
                {
                    db.Aop.OnLogExecuting = (sql, pars) =>
                    {
                        // Log SQL
                        Console.WriteLine(UtilMethods.GetNativeSql(sql, pars));
                    };
                    db.Aop.OnError = (exp) =>
                    {
                        // Log error
                        // Console.WriteLine(exp.Message);
                    };
                });
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="connStr"></param>
        public DbUtils(string connStr, DbType dbType)
        {
            _connStr = connStr;
            _dbType = dbType;
        }
    }
}
