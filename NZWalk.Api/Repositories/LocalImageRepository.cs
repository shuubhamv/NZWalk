using Microsoft.EntityFrameworkCore;
using NZWalk.Api.Data;
using NZWalk.Api.Models.Domain;

namespace NZWalk.Api.Repositories
{
    public class LocalImageRepository : IImageRepository
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly NZWalksDbContext dbContext;

        public LocalImageRepository(IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor, NZWalksDbContext dbContext )
        {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
            this.dbContext = dbContext;
        }
        public async Task<Image> Upload(Image image)  
        {
            var localFilePath = Path.Combine(webHostEnvironment.ContentRootPath, "Images", 
               $"{image.FileName}{image.FileExtension}" );
             
            //upload image to the local path
            using var stream = new FileStream(localFilePath, FileMode.Create);  //reads files steram at localFilePath location and create 
            await image.File.CopyToAsync(stream); // iformfile method


            //http://Loccalhost:7100/images/image.jpg
            var urlFilePath = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}/Images/{image.FileName}{image.FileExtension}";

            image.FilePath = urlFilePath;


            // add image to images table

            await dbContext.Images.AddAsync(image);

            await dbContext.SaveChangesAsync();

            return image;


             
        }
    }
}
