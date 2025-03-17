namespace NZWalk.Api.Models.DTO
{
    public class RegionDto
    {
        public Guid id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public string? RegionImageUrl { get; set; }
    }
}
