namespace AntSK.Domain.Options
{
    public class LLamaSharpOption
    {
        public static string RunType { get; set; }
        public static uint? ContextSize { get; set; }
        public static int? GpuLayerCount { get; set; }
    }
}
