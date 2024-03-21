using System.Collections.ObjectModel;
using HeartLog.ControlLib.Logger.Model;

namespace HeartLog.ControlLib.Logger.ViewModel
{
    public class LoggerControlVM
    {
        public ObservableCollection<LogItem> LogItemList { get; set; } = new();

        public void AddLog(LogItem logItem)
        {
            LogItemList.Add(logItem);
        }

        public void ClearLog()
        {
            LogItemList.Clear();
        }
    }
}
