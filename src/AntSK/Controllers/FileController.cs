using Microsoft.AspNetCore.Mvc;

namespace AntSK.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        /// <summary>
        /// Upload FileName
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            // 别忘了进行异常处理和文件为空的判断
            if (file == null || file.Length == 0)
            {
                return BadRequest("没有选择要上传的文件。");
            }

            // 创建文件存储的路径
            var uploadsFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files"); // 给定的文件夹名称

            // 如果路径不存在，则创建一个新的目录
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            string extension = Path.GetExtension(file.FileName);
            string fileid = Guid.NewGuid().ToString();
            // 组合目标路径
            var uploads = Path.Combine(uploadsFolderPath, fileid + extension);

            // 保存文件至目标路径
            using var fileStream = System.IO.File.Create(uploads);
            using var uploadStream = file.OpenReadStream();
            await uploadStream.CopyToAsync(fileStream);

            return Ok(uploads);
        }
    }
}
