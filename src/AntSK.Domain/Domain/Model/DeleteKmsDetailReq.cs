namespace AntSK.Domain.Domain.Model
{
    public class DeleteKmsDetailReq
    {
        /// <summary>
        /// 知识库Id
        /// </summary>
        public string KmsId { get; set; } = null!;

        /// <summary>
        /// 文档Id
        /// </summary>
        public string DocumentId { get; set; } = null!;

        /// <summary>
        /// 重新切片
        /// </summary>
        public bool ReCut { get; set; } = false;
    }
}
