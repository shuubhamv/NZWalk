using System.ComponentModel.DataAnnotations.Schema;

namespace NZWalk.Api.Models.Domain
{
    public class Image
    {
        public Guid Id { get; set; }
        [NotMapped]// Does not map this property to the database
        public IFormFile File { get; set; } // IFormFile is a type defined in Microsoft.AspNetCore.Http namespace//type of the file we accept,info about file

        public string? FileDescription { get; set; }
        public string FileExtension { get; set; }

        public long FileSizeInByte { get; set; }

        public string FilePath { get; set; }
        public string FileName { get;  set; }
    }
}
