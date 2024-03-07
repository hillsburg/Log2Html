using HeartLog.SimpleDbTool.Enum;

namespace HeartLog.SimpleDbTool.Attribute
{
    public class DbTypeAttribute : System.Attribute
    {
        private DatabaseType _dbType;

        public DatabaseType DbType
        {
            get { return _dbType; }
            set { _dbType = value; }
        }

        public DbTypeAttribute(DatabaseType dbType)
        {
            DbType = dbType;
        }
    }
}
