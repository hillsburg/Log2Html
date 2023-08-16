using LoggerControlTest.Model;
using System.Collections.ObjectModel;

namespace LoggerControlTest.ViewModel
{
    public class LogControlTestVM
    {
        public ObservableCollection<LogItemModel> LogItemList { get; set; }

        public LogControlTestVM()
        {
            LogItemList = new ObservableCollection<LogItemModel>();
        }

    }
}
