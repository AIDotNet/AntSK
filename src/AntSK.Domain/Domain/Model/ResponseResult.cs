namespace AntSK.Domain
{
    public class Result
    {
        /// <summary>
        /// 错误码，0是正常返回，异常返回错误码
        /// </summary>
        public string Code { get; set; } = "0";
        /// <summary>
        /// 返回数据
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// 返回信息详情
        /// </summary>
        public string Message { get; set; }


    }

    public static class ResponseResult
    {
        /// <summary>
        /// 执行成功
        /// </summary>
        /// <returns></returns>
        public static Result Success()
        {
            return new Result
            {
                Data = "",
                Code = "0",
                Message = "ok"
            };
        }

        /// <summary>
        /// 执行成功
        /// </summary>
        /// <param name="data"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Result Success(this object data, string code = "0", string message = "ok")
        {
            return new Result
            {
                Data = data,
                Code = code,
                Message = message
            };
        }
        /// <summary>
        /// 执行失败
        /// </summary>
        /// <param name="data"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Result Error(this object data, string code, string message)
        {
            return new Result
            {
                Data = data,
                Code = code,
                Message = message
            };
        }
        /// <summary>
        /// 执行失败
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Result Error(string code, string message)
        {
            return new Result
            {
                Data = "",
                Code = code,
                Message = message
            };
        }
    }
}
