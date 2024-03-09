namespace AntSK.Domain.Options
{
    public class KernelMemoryOption
    {
        /// <summary>
        /// 向量库
        /// </summary>
        public static string VectorDb { get; set; }
        /// <summary>
        /// 连接字符串
        /// </summary>
        public static string ConnectionString { get; set; }
        /// <summary>
        /// 表前缀
        /// </summary>
        public static string TableNamePrefix { get; set; }
    }
}
