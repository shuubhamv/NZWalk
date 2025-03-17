using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalk.Api.CustomActionfilters;
using NZWalk.Api.Data;
using NZWalk.Api.Models.Domain;
using NZWalk.Api.Models.DTO;
using NZWalk.Api.Repositories;
using System.Diagnostics.Eventing.Reader;
using System.Text.Json;

namespace NZWalk.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IMapper mapper;
        private readonly ILogger<RegionsController> logger;

        public IRegionRepository regionRepository { get; }

        public RegionsController(NZWalksDbContext dbContext, IRegionRepository regionRepository,
            IMapper mapper,
            ILogger<RegionsController> logger)
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
            this.logger = logger;
        }
        [HttpGet]
       // [Authorize(Roles ="Reader,Writer")]
        public async Task< IActionResult >GetAll()
        {
            
                //var regions = new List<Region> 
                //{
                //    new Region
                //    { 
                //        id = Guid.NewGuid(),
                //        Name = "Auckland Region",
                //        Code = "AUK",                    
                //        RegionImageUrl = "https://www.doc.govt.nz/globalassets/images/regions/auckland/auckland-landscape-2.jpg"
                //    },
                //    new Region
                //    {
                //        id = Guid.NewGuid(),
                //        Name = "Bay of Plenty Region",
                //        Code = "BOP",
                //        RegionImageUrl = "https://www.doc.govt.nz/globalassets/images/regions/bay-of-plenty/bay-of-plenty-landscape-2.jpg"
                //    }

                //};

                //get data from database

                //  var regionsDomain = await dbContext.Regions.ToListAsync();//without using repositories

                var regionsDomain = await regionRepository.GetAllAsync();// importing interface and concret class using repositories


                logger.LogInformation($"Finishe get all region request with data:{JsonSerializer.Serialize(regionsDomain)}");

                //map domani model to dto model
                // var regionDto = new List<RegionDto>();
                //foreach (var regionDomain in regionsDomain)
                //{
                //    regionDto.Add(new RegionDto()
                //    {
                //        id = regionDomain.id,
                //        Code = regionDomain.Code,
                //        Name = regionDomain.Name,
                //        RegionImageUrl = regionDomain.RegionImageUrl
                //    });

                //}

                //map domain model to dtos
                //var regionDto = mapper.Map<List<RegionDto>>(regionsDomain);
                //return Ok(regionDto);
                //or

                return Ok(mapper.Map<List<RegionDto>>(regionsDomain));






            

           
         
}

        [HttpGet]
        [Route("{id:guid}")]
       // [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> GetById([FromRoute]Guid id)
        {
            // var region = dbContext.Regions.Find(id);
            //  var regionDomain =await dbContext.Regions.FirstOrDefaultAsync(x => x.id == id);//without using repositories

            var regionDomain = await regionRepository.GetByIdAsync(id);// importing interface and concret class using repositories

            if (regionDomain == null)
            {
                return NotFound();
            }
            //var regionDto = new RegionDto()
            //{
            //    id = regionDomain.id,
            //    Code = regionDomain.Code,
            //    Name = regionDomain.Name,
            //    RegionImageUrl = regionDomain.RegionImageUrl
            //};
            return Ok(mapper.Map<RegionDto>(regionDomain));
        }
        //post to create new region
        //post:https://localhost:5001/api/regions
        [HttpPost]
        [ValidateModel]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
            // map or convert dto to domain model
            //var regionDomainModel = new Region()
            //{
            //    Code = addRegionRequestDto.Code,
            //    Name =  addRegionRequestDto.Name,
            //    RegionImageUrl = addRegionRequestDto.RegionImageUrl
            //};
            
                var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);

                //use domain mode to to create region

                //await dbContext.Regions.AddAsync(regionDomainModel);
                //await dbContext.SaveChangesAsync();

                regionDomainModel = await regionRepository.CreateAsync(regionDomainModel);

                //map domain model back to dto model

                //var regionDto = new RegionDto()
                //{
                //    id = regionDomainModel.id,
                //    Code = regionDomainModel.Code,
                //    Name = regionDomainModel.Name,
                //    RegionImageUrl = regionDomainModel.RegionImageUrl
                //};

                var regionDto = mapper.Map<RegionDto>(regionDomainModel);
                return CreatedAtAction(nameof(GetById), new { id = regionDomainModel.id }, regionDto);
            
          
        }

        [HttpPut]
        [Route("{id:guid}")]
        [ValidateModel]
       // [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        { // map Dto to domain model
            //var regionDomain = new Region()
            //{
            //    Code = updateRegionRequestDto.Code,
            //    Name = updateRegionRequestDto.Name,
            //    RegionImageUrl = updateRegionRequestDto.RegionImageUrl
            //};
            
                var regionDomain = mapper.Map<Region>(updateRegionRequestDto);


                //find check region by id
                //  var regionDomain = await dbContext.Regions.FirstOrDefaultAsync(x => x.id == id);

                regionDomain = await regionRepository.UpdateAsync(id, regionDomain);


                if (regionDomain == null)
                {
                    return NotFound();
                }
                //update region
                // regionDomain.Code = updateRegionRequestDto.Code;
                // regionDomain.Name = updateRegionRequestDto.Name;
                // regionDomain.RegionImageUrl = updateRegionRequestDto.RegionImageUrl;
                // //save changes
                //await dbContext.SaveChangesAsync();

                //map domain model back to dto model
                //var regionDto = new RegionDto()
                //{
                //    id = regionDomain.id,
                //    Code = regionDomain.Code,
                //    Name = regionDomain.Name,
                //    RegionImageUrl = regionDomain.RegionImageUrl
                //};

                var regionDto = mapper.Map<RegionDto>(regionDomain);
                return Ok(regionDto);
            
        }
       

        [HttpDelete]
        [Route("{id:guid}")]
       // [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            //find region by id
           // var regionDomain = await dbContext.Regions.FirstOrDefaultAsync(x => x.id == id);

           var regionDomain = await regionRepository.DeleteAsync(id);
            if (regionDomain == null)
            {
                return NotFound();
            }
            //delete region
            dbContext.Regions.Remove(regionDomain);
            //save changes
          await dbContext.SaveChangesAsync();

            //return deleted region back
            //map domain model to dto

            //var regionDto = new RegionDto()
            //{
            //    id = regionDomain.id,
            //    Code = regionDomain.Code,
            //    Name = regionDomain.Name,
            //    RegionImageUrl = regionDomain.RegionImageUrl
            //};
           // var regionDto = mapper.Map<RegionDto>(regionDomain);
            return Ok(mapper.Map<RegionDto>(regionDomain));
        }
    }
}
