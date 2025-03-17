using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalk.Api.Models.Domain;
using NZWalk.Api.Models.DTO;
using NZWalk.Api.Repositories;

namespace NZWalk.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository imageRepository;

        public ImagesController(IImageRepository imageRepository)
        {
            this.imageRepository = imageRepository;
        }
        //post: api/images/upload
        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload([FromForm] ImageUploadRequestDto request )
        {
            ValidateFileUpload(request);
            if (ModelState.IsValid)
            {
                // Convert DTO to domain model
                var imageDomainModel = new Image
                {
                    File = request.File,     //That is  main file that we are going to upload
                    FileExtension = Path.GetExtension(request.File.FileName),// automatically extract the extension of the file
                    FileSizeInByte = request.File.Length,  // size of the file
                    FileName = request.File.FileName,
                    FileDescription = request.FileDescription
                };

                //Use repository to upload image

                await imageRepository.Upload(imageDomainModel);

                return Ok(imageDomainModel);

            }
            return BadRequest(ModelState);
        }
        private void ValidateFileUpload(ImageUploadRequestDto request)
        {
           var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            if (!allowedExtensions.Contains(Path.GetExtension(request.File.FileName)))
            {
                ModelState.AddModelError("file", "Invalid file type");
            }
            if (request.File.Length > 10485760)
            {
                ModelState.AddModelError("file", "File size should not exceed 10MB");
            }
        }
    }
}
