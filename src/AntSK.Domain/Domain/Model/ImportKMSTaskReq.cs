using AntSK.Domain.Repositories;

namespace AntSK.Domain.Domain.Model
{
    public class ImportKMSTaskDTO
    {

        public ImportType ImportType { get; set; }

        public string KmsId { get; set; }

        public string Url { get; set; } = "";


        public string Text { get; set; } = "";

        public string FilePath { get; set; } = "";

        public string FileName { get; set; } = "";
    }


    public class ImportKMSTaskReq : ImportKMSTaskDTO
    {
        public KmsDetails KmsDetail { get; set; } = new KmsDetails();
    }

    public enum ImportType
    {
        File = 1,
        Url = 2,
        Text = 3
    }
}
