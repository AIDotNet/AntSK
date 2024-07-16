namespace AntSK.Models.Dto
{
    public class AiModelListDto : PagerReturnDto<AiModelDto>
    {

    }
    public class AiModelDto
    {
        public string Id { get; set; }

        public string ModelDescription { get; set; }
        public string ModelName { get; set; }
    }
}
