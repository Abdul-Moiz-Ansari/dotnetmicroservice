using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlatformService.Controllers
{
    [Route("api/{controller}")]
    [ApiController]
    public class PlatformsController:ControllerBase
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;

        public PlatformsController(
            IPlatformRepo repository,
            IMapper mapper,
            ICommandDataClient commandDataClient
            
            )
        {
            _repository = repository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms() {
            Console.WriteLine("--> Getting Platforms");

            var platformItems = _repository.GetAllPlatforms();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            var platformItem = _repository.GetPlatformById(id);

            if (platformItem != null)
            {
                return Ok(_mapper.Map<PlatformReadDto>(platformItem));
            }

            return NotFound();            
        }

        // [HttpGet(Name = "getOrganizationsSummary")]
        // public async Task<ActionResult<List<OrganizationSummary>>> GetOrganizationsSummary()
        // {
        //     using (var httpClient = new HttpClient())
        //     {
        //         var organizationService = new OrganizationService(httpClient);

        //         try
        //         {
        //             var organizationSummaries = await organizationService.GetOrganizationsSummary();

        //             return Ok(organizationSummaries);

        //             // Output or return organization summaries as needed
        //             //foreach (var summary in organizationSummaries)
        //             //{
        //             //    Console.WriteLine($"Organization Id: {summary.Id}");
        //             //    Console.WriteLine($"Organization Name: {summary.Name}");
        //             //    Console.WriteLine($"blacklistTotal: {summary.BlacklistTotal}");
        //             //    Console.WriteLine($"Total Phones: {summary.TotalCount}");

        //             //    foreach (var userSummary in summary.Users)
        //             //    {
        //             //        Console.WriteLine($"-- User Id: {userSummary.Id}");
        //             //        Console.WriteLine($"-- User Email: {userSummary.Email}");
        //             //        Console.WriteLine($"-- Phone Count: {userSummary.PhoneCount}");
        //             //    }
        //             //    Console.WriteLine();
        //             //}
        //         }
        //         catch (Exception ex)
        //         {
        //             Console.WriteLine($"An error occurred: {ex.Message}");
        //         }
        //     }

        //     return NotFound();
        // }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platform)
        {
            var platoformModel = _mapper.Map<Platform>(platform);

            _repository.CreatePlatform(platoformModel);
            _repository.SaveChanges();

            _mapper.Map<PlatformReadDto>(platoformModel);

            var platformReadDto = _mapper.Map<PlatformReadDto>(platoformModel);

            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not send synchronously {ex.Message}");
            }

            return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id }, platformReadDto);
        }
    }
}