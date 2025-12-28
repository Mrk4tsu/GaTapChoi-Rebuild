using GaVL.Application.Systems;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GaVL.API.Controllers
{
    [Route("api/file")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IR2Service _r2Service;
        public FilesController(IR2Service r2Service)
        {
            _r2Service = r2Service; 
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var key = $"test/{Guid.NewGuid()}_{file.FileName}"; // Tạo key unique
            var resultUrl = await _r2Service.UploadFileGetUrl(file, key);
            return Ok(new { location = resultUrl });
            //return Ok(new { Message = "File uploaded successfully", Key = key });
        }
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteImage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return BadRequest("File name required.");

            await _r2Service.DeleteFileAsync(fileName, "test");
            return Ok();
        }
    }
}
