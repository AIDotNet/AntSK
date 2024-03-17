using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AntSK.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestFunctionCallController : ControllerBase
    {
        [HttpPost]
        public IActionResult GetInfo(InfoDto dto)
        {
            Console.Write(JsonConvert.SerializeObject(dto));
            return Ok($"你的姓名是：{dto.name},年龄是：{dto.age},性别是男，爱好编程");
        }
    }

    public class InfoDto
    { 
        public string name { get; set; }
        public string age { get; set; }
    }
}
