using Log2Html.Enum;

namespace Log2Html.Model
{
    public class LogItem
    {
        public string TimeStamp { get; set; }

        public LogLevel LogLevel { get; set; }

        public string LogContent { get; set; }

        public LogDestination Destination { get; set; }
    }
}
