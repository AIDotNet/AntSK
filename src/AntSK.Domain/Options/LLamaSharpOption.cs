namespace AntSK.Domain.Options
{
    public class LLamaSharpOption
    {
        public static string RunType { get; set; }
        public static string Chat { get; set; }

        public static string Embedding { get; set; }

        public static string FileDirectory { get; set; } = Directory.GetCurrentDirectory();
    }
}
