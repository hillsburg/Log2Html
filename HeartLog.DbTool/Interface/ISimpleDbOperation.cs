using System.Data;

namespace HeartLog.SimpleDbTool.Interface
{
    /// <summary>
    /// Database操作的接口，不同的数据库封装应当实现该接口
    /// @author Qihang
    /// </summary>
    public interface ISimpleDbOperation
    {

        bool Create<T>(T t) where T : ISimpleOrm;

        bool Update<T>(T t) where T : ISimpleOrm;

        bool Delete<T>(T t) where T : ISimpleOrm;

        bool Insert<T>(T t) where T : ISimpleOrm;

        /// <summary>
        /// 通过DataReader读取数据并且返回数据集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        List<T> ExecuteReaderGetList<T>(T t) where T : ISimpleOrm, new();
        List<T> ExecuteReaderGetList<T>(string sql) where T : ISimpleOrm, new();

        int ExecuteNonQuery(string commandText, CommandType commandType = CommandType.Text);

        object ExecuteScalar(string commandText, CommandType commandType = CommandType.Text);
    }
}
