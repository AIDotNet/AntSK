namespace AntSK.Domain.Domain.Model
{
    public class PageList<T>
    {
        //查询结果
        public List<T> List { get; set; }
        /// <summary>
        /// 当前页，从1开始
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 每页数量
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }
    }
}
