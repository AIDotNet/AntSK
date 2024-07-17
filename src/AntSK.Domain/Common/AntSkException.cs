namespace AntSK.Domain.Common
{
    public class AntSkException : Exception
    {
        public AntSkException(int code, string message, Exception ex = null) : base(message, ex)
        {
            ErrorCode = code;
        }
        public int ErrorCode { get; set; }

    }

    public class AntSkUnAuthorizeException : AntSkException
    {
        public AntSkUnAuthorizeException(int code, string message, Exception ex = null) : base(code, message, ex) { }
    }
}
