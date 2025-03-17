using System.ComponentModel.DataAnnotations;

namespace NZWalk.Api.Models.DTO
{
    public class UpdateWalkRequestDto
    {

        [Required]
        [MaxLength(100, ErrorMessage = "Name has to be Maximum 100 Charchters")]
        public string Name { get; set; }

        [Required]
        [MaxLength(500, ErrorMessage = "Description has to be Maximum 500 Charchters")]
        public string Description { get; set; }

        [Required]
        [Range(0, 50, ErrorMessage = "Length has to be between 0 and 100")]
        public double LengthInKm { get; set; }
        public string? WalkImageUrl { get; set; }

        [Required]
        public Guid DifficultyId { get; set; }

        [Required]
        public Guid RegionId { get; set; }

    }
}
