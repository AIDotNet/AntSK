namespace AntSK.Domain.Domain.Model.Enum
{
    /// <summary>
    /// AI类型
    /// </summary>
    public enum AIType
    {
        OpenAI = 1,
        AzureOpenAI = 2,
        LLamaSharp = 3,
        SparkDesk = 4,
        Mock = 5,
        LLamaFactory = 6,
    }

    /// <summary>
    /// 模型类型
    /// </summary>
    public enum AIModelType
    {
        Chat = 1,
        Embedding = 2,
    }
}
