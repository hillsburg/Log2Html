using Log2Html.Model;

namespace Log2Html.Interface
{
    /// <summary>
    /// ILogService
    /// </summary>
    public interface ILogService
    {
        void AddLog(LogItem logItem);

        void ClearLog();
    }
}
