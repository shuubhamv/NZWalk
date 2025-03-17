using System.ComponentModel.DataAnnotations;

namespace NZWalk.Api.Models.DTO
{
    public class UpdateRegionRequestDto
    {
        [Required]
        [MinLength(3, ErrorMessage = "Cod has to be Mininmum 3 Charchters")]
        [MaxLength(3, ErrorMessage = "Cod has to be Maximum 3 Charchters")]
        public string Code { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Name has to be Maximum 100 Charchters")]
        public string Name { get; set; }

        public string? RegionImageUrl { get; set; }
    }
}
