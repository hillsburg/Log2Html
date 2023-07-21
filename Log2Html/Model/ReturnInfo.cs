namespace Log2Html.Model
{
    public class ReturnInfo
    {
        public bool IsOk;

        public string Message { get; set; }

        public int Code;

        public ReturnInfo(bool isOk, string message, int code)
        {
            IsOk = isOk;
            Message = message;
            Code = code;
        }

        public ReturnInfo(bool isOk)
        {
            IsOk = isOk;
        }
    }
}
