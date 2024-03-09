namespace AntSK.Domain.Options
{
    public class DBConnectionOption
    {
        /// <summary>
        /// sqlite连接字符串
        /// </summary>
        public static string DbType { get; set; }
        /// <summary>
        /// pg链接字符串
        /// </summary>
        public static string ConnectionStrings { get; set; }
    }
}
