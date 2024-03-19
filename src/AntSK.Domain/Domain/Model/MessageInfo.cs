namespace AntSK.Domain.Domain.Model
{
    public class MessageInfo
    {
        public string ID { get; set; } = "";
        public string Context { get; set; } = "";
        public string HtmlAnswers { get; set; } = "";

        /// <summary>
        /// 发送是true  接收是false
        /// </summary>
        public bool IsSend { get; set; } = false;

        public DateTime CreateTime { get; set; }

        public string? FilePath { get; set; }

        public string? FileName { get; set; }
    }
}