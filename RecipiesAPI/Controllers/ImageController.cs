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
            const long MAX_FILE_SIZE = 5 * 1024 * 1024; // 5MB
            const int MAX_FILE_COUNT = 10; // Maximum 10 files per upload

            if (files == null || files.Count == 0)
                return BadRequest("No files uploaded.");

            if (files.Count > MAX_FILE_COUNT)
                return BadRequest($"Maximum {MAX_FILE_COUNT} files allowed per upload.");

            var uploadsFolder = Path.Combine(_environment.ContentRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            // Validate all files before processing
            foreach (var file in files)
            {
                if (file.Length > MAX_FILE_SIZE)
                {
                    return BadRequest($"File '{file.FileName}' exceeds maximum size of 5MB.");
                }

                if (!allowedMimeTypes.Contains(file.ContentType) || !allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
                {
                    return BadRequest($"File '{file.FileName}' has invalid type. Only JPEG, PNG, and GIF files are allowed.");
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
