using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RecipiesAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ImageController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public ImageController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImages(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return BadRequest("No files uploaded.");

            var uploadsFolder = Path.Combine(_environment.ContentRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            foreach (var file in files)
            {
                if (!allowedMimeTypes.Contains(file.ContentType) || !allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
                {
                    return BadRequest("Invalid file type. Only JPEG, PNG, and GIF files are allowed.");
                }
            }

            var uploadedFiles = new List<object>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var url = $"{Request.Scheme}://{Request.Host}/uploads/{uniqueFileName}";
                    uploadedFiles.Add(new { fileName = uniqueFileName, fileUrl = url });
                }
            }

            return Ok(uploadedFiles);
        }
    }
}
