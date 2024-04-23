
namespace AntSK.Domain.Domain.Model.Dto
{
    public class RelevantSource
    {
        public string SourceName { get; set; }

        public string Text { get; set; }
        public float Relevance { get; set; }

        public double RerankScore { get; set; }

        public override string ToString()
        {
            return $"[file:{SourceName};Relevance:{(Relevance * 100):F2}%]:{Text}";
        }
    }
}
