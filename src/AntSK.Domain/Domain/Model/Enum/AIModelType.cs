using System.ComponentModel.DataAnnotations;

namespace AntSK.Domain.Domain.Model.Enum
{
    /// <summary>
    /// AI类型
    /// </summary>
    public enum AIType
    {
        [Display(Name = "Open AI")]
        OpenAI = 1,

        [Display(Name = "Azure Open AI")]
        AzureOpenAI = 2,

        [Display(Name = "星火大模型")]
        SparkDesk = 4,

        [Display(Name = "灵积大模型")]
        DashScope = 5,

        [Display(Name = "LLamaFactory")]
        LLamaFactory = 6,
        [Display(Name = "Bge Embedding")]
        BgeEmbedding = 7,
        [Display(Name = "Bge Rerank")]
        BgeRerank = 8,

        [Display(Name = "Ollama")]
        Ollama = 10,
        [Display(Name = "OllamaEmbedding")]
        OllamaEmbedding = 11,
        [Display(Name = "模拟输出")]
        Mock = 100,

    }

    /// <summary>
    /// 模型类型
    /// </summary>
    public enum AIModelType
    {
        Chat = 1,
        Embedding = 2,
        Rerank=4
    }
}
