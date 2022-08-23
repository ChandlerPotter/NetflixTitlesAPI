using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetflixTitles.API.Entities;
using NetflixTitles.API.Models;
using NetflixTitles.API.Services;

namespace NetflixTitles.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/titles")]
    
    public class AllTitlesController : ControllerBase
    {
        private readonly INetflixTitlesRepository _netflixTitlesRepository;
        private readonly ILogger<AllTitlesController> _logger;
        private readonly IMapper _mapper;
        const int maxTitlesPageSize = 50;

        public AllTitlesController(INetflixTitlesRepository netflixTitlesRepository,
            ILogger<AllTitlesController> logger,
            IMapper mapper)
        {
            _netflixTitlesRepository = netflixTitlesRepository ?? throw new ArgumentNullException(nameof(netflixTitlesRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }

        [HttpGet]

        public async Task<ActionResult<IEnumerable<AllTitlesDto>>> GetAllTitles(
            string? type, string? genre, string? rating, string? searchQuery,
            int pageNumber = 1, int pageSize = 10)
        {
            pageSize = (pageSize > maxTitlesPageSize) ? maxTitlesPageSize : pageSize;

            var (Alltitles, paginationMetadata) = await _netflixTitlesRepository
                .GetAllTitlesAsync(type, genre, rating, searchQuery, pageNumber, pageSize);

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            return Ok(_mapper.Map<IEnumerable<AllTitlesDto>>(Alltitles)); ;
        }

        [HttpGet("{id}", Name = "GetTitle")]

        public async Task<ActionResult<AllTitlesDto>> GetTitle(int id)
        {
            var title = await _netflixTitlesRepository.GetTitleAsync(id);
            if (title == null)
            {
                _logger.LogInformation($"Title with id {id} was not found when accessing all titles.");
                return NotFound();
            }

            return Ok(_mapper.Map<AllTitlesDto>(title));
        }
    }
}
