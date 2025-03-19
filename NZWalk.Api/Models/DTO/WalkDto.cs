﻿using NZWalk.Api.Models.Domain;

namespace NZWalk.Api.Models.DTO
{
    public class WalkDto
    {
        public Guid id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public double LengthInKm { get; set; }
        public string? WalkImageUrl { get; set; }

        //public Guid DifficultyId { get; set; }
        //public Guid RegionId { get; set; }

        public RegionDto Region { get; set; }
        public DifficultyDto Difficulty { get; set; }


    }
}
