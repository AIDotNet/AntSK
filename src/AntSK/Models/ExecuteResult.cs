using NPOI.SS.Formula.Functions;

namespace AntSK.Models
{
    public class ExecuteResult
    {
        /// <summary>
        /// 编码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = "执行成功";

        public bool IsSuccess() { return Code == 0; }

        public static ExecuteResult Error(string message, int code = 500)
        {
            var result = new ExecuteResult();
            result.Message = message;
            result.Code = code;
            return result;
        }
        public static ExecuteResult Success(string message = "执行成功")
        {
            var result = new ExecuteResult();
            result.Message = message;
            result.Code = 0;
            return result;
        }
    }

    public class ExecuteResult<T> : ExecuteResult
    {
        /// <summary>
        /// 返回数据
        /// </summary>
        public T Data { get; set; }

        public static new ExecuteResult<T> Error(string message, int code = 500)
        {
            var result = new ExecuteResult<T>();
            result.Message = message;
            result.Code = code;
            return result;
        }
        public static  ExecuteResult<T> Success(T data, string message = "执行成功")
        {
            var result = new ExecuteResult<T>();
            result.Message = message;
            result.Data = data;
            result.Code = 0;
            return result;
        }
    }
}
