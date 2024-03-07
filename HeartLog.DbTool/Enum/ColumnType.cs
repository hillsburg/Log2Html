namespace HeartLog.SimpleDbTool.Enum
{
    /// <summary>
    /// 列的数据类型
    /// </summary>
    public enum ColumnType : int
    {
        /// <summary>
        /// 未知类型，sql语句默认为此
        /// </summary>
        UNKNOWN = 0,
        /// <summary>
        /// string
        /// </summary>
        TEXT = 1,
        /// <summary>
        /// 为此类型，ORM拼接sql insert等语句不会添加''
        /// </summary>
        INTEGER = 2,
        /// <summary>
        /// 浮点类型
        /// </summary>
        DOUBLE = 3,
        /// <summary>
        /// 整型LONG
        /// </summary>
        LONG = 4,
        /// <summary>
        /// 日期类型
        /// </summary>
        DATETIME = 5,
        /// <summary>
        /// 枚举类型
        /// </summary>
        ENUM = 6
    }
}
