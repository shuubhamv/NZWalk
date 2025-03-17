using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalk.Api.CustomActionfilters;
using NZWalk.Api.Models.Domain;
using NZWalk.Api.Models.DTO;
using NZWalk.Api.Repositories;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.InteropServices;

namespace NZWalk.Api.Controllers
{
    //api/walks
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper mapper;

        public WalksController(IMapper mapper, IWalkRepository walkRepository)
        {
            this.mapper = mapper;
            WalkRepository = walkRepository;
        }

        public IWalkRepository WalkRepository { get; }

        //create Walk
        //post:/api/walks
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addWalkRequestDto)
        {
            {
                //map dto to domain model using automapper

                var walkDomainModel = mapper.Map<Walk>(addWalkRequestDto);

                await WalkRepository.CreateAsync(walkDomainModel);

                //map domain model to dto

                return Ok(mapper.Map<WalkDto>(walkDomainModel));


            }
        }


        //get walk
        //get:/api/walks?filterOn=Name&filterQuery=Trak&sortBy=Name&isAscending=true&pageNumber=1&pageSize=10

        [HttpGet] 
            public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
                [FromQuery] string? sortBy, [FromQuery] bool? isAscending,
                [FromQuery] int pageNumber = 1, int pageSize = 1000 )
            {
                var walksDomainModel = await WalkRepository.GetAllAsync(filterOn,filterQuery, sortBy, isAscending ?? true,
                    pageNumber,pageSize);

            // Creat an new  excepion

            throw new Exception("This is new exception ");
                //mape domain model to dto

                return Ok(mapper.Map<List<WalkDto>>(walksDomainModel));
            }

            [HttpGet]   
            [Route("{id:Guid}")]
            public async Task<IActionResult> GetById([FromRoute] Guid id)
            {
                var walkDomainModel = await WalkRepository.GetByIdAsync(id);
                if (walkDomainModel == null)
                {
                    return NotFound();
                }
                //map domain model to dto
                return Ok(mapper.Map<WalkDto>(walkDomainModel));
            }


            //update walk by id
            //put:/api/walks/{id}
            [HttpPut]
            [Route("{id:Guid}")]
            [ValidateModel]
            public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateWalkRequestDto updateWalkRequestDto)
            {


                //map dto to domain model 

                var walkDomainModel = mapper.Map<Walk>(updateWalkRequestDto);

                walkDomainModel = await WalkRepository.UpdateAsync(id, walkDomainModel);
                if (walkDomainModel == null)
                {
                    return NotFound();
                }
                //map domain model to dto
                return Ok(mapper.Map<WalkDto>(walkDomainModel));
            }
            //delete walk by id
            //delete:/api/walks/{id}
            [HttpDelete]
            [Route("{id:Guid}")]
            public async Task<IActionResult> Delete([FromRoute] Guid id)
            {
                var deletedWalkDomainModel = await WalkRepository.DeleteAsync(id);
                if (deletedWalkDomainModel == null)
                {
                    return NotFound();
                }
                //map domain model to dto
                return Ok(mapper.Map<WalkDto>(deletedWalkDomainModel));
            }



        } }
