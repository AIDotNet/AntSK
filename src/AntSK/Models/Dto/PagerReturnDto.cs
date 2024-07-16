namespace AntSK.Models.Dto
{
    public class PagerReturnDto<T> where T : class
    {
        public List<T> Items { get; set; }

        public int TotalCount { get; set; }
    } 
}
