using HeartLog.SimpleDbTool;
using Log2Html.Model;

namespace Log2Html.Dao
{
    internal class DbUtils
    {
        private SimpleDbHelper _dbHelper;

        /// <summary>
        /// DB helper
        /// </summary>
        public SimpleDbHelper DbHelper
        {
            get { return _dbHelper; }
            set { _dbHelper = value; }
        }

        public DbUtils()
        {
        }

        public ReturnInfo Init(string connStr)
        {
            _dbHelper = new SimpleDbHelper(HeartLog.SimpleDbTool.Enum.DatabaseType.Sqlite, connStr);
            return new ReturnInfo(true);
        }
    }
}
