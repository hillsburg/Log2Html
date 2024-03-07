namespace HeartLog.SimpleDbTool.Attribute
{
    /// <summary>
    /// ORM表名特性
    /// </summary>
    public class TableAttribute : System.Attribute
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tableName">Model对应的表名</param>
        public TableAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}
