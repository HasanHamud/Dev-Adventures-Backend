using Microsoft.AspNetCore.Mvc;

namespace Dev_Adventures_Backend.Controllers.Videos
{

    [ApiController]
    [Route("api/[controller]")]
    public class VideoController : ControllerBase
    {

        private readonly IWebHostEnvironment _hostingEnvironment;

        public VideoController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadVideo(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { Message = "No file was uploaded." });

                var validExtensions = new[] { ".mp4", ".avi", ".mov", ".wmv", ".flv", ".mkv" };
                var extension = Path.GetExtension(file.FileName).ToLower();

                if (!validExtensions.Contains(extension))
                    return BadRequest(new { Message = "Invalid file type. Only video files are allowed." });

                var uploadPath = Path.Combine(_hostingEnvironment.WebRootPath, "videos");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Ok(new { FileName = fileName, Path = $"/videos/{fileName}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while uploading the file.", Details = ex.Message });
            }
        }

    }
}
