using AntSK.Domain.Repositories;

namespace AntSK.Domain.Domain.Model
{
    public class ImportKMSTaskDTO
    {
        /// <summary>
        /// 导入类型
        /// </summary>
        public ImportType ImportType { get; set; }

        /// <summary>
        /// 知识库Id
        /// </summary>
        public string KmsId { get; set; }

        /// <summary>
        /// 导入链接
        /// </summary>
        public string Url { get; set; } = "";

        /// <summary>
        /// 导入文本
        /// </summary>
        public string Text { get; set; } = "";

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; } = "";

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; } = "";

        /// <summary>
        /// 是否QA切分
        /// </summary>
        public bool IsQA { get; set; } = false;
    }


    public class ImportKMSTaskReq : ImportKMSTaskDTO
    {
        public bool IsQA { get; set; }=false;
        public KmsDetails KmsDetail { get; set; } = new KmsDetails();
    }

    public enum ImportType
    {
        File = 1,
        Url = 2,
        Text = 3,
        Excel=4
    }

    public class QAModel
    {
        public string ChatModelId { get; set; }
        public string Context { get; set; }
    }
}
