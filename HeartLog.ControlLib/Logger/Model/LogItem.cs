using HeartLog.ControlLib.Logger.Enum;

namespace HeartLog.ControlLib.Logger.Model
{
    public class LogItem
    {
        public string TimeStamp { get; set; }

        public LogLevel LogLevel { get; set; }

        public string LogContent { get; set; }

        public LogDestination Destination { get; set; }
    }
}
