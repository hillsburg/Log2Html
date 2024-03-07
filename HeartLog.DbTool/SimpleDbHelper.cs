using HeartLog.SimpleDbTool.Enum;
using HeartLog.SimpleDbTool.Interface;
using HeartLog.SimpleDbTool.SimpleDbServer;

namespace HeartLog.SimpleDbTool
{
    public class SimpleDbHelper
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DatabaseType DbType;

        /// <summary>
        /// Database操作对象
        /// </summary>
        public ISimpleDbOperation DbOperation;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbType"></param>
        public SimpleDbHelper(DatabaseType dbType, string connecStr)
        {
            DbType = dbType;
            ConnectionString = connecStr;
            DbOperation = GetTargetDbDriver(dbType, connecStr);
        }

        /// <summary>
        /// 获取指定类型的数据库驱动类
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public ISimpleDbOperation GetTargetDbDriver(DatabaseType dbType, string connecStr)
        {
            switch (dbType)
            {
                case DatabaseType.Sqlite:
                    return new SimpleSqlite(connecStr);
                case DatabaseType.SqlServer:
                    return new SimpleSqlServer(connecStr);
                case DatabaseType.MySql:
                    return new SimpleMysql(connecStr);
                default:
                    return null;
            }
        }
    }
}