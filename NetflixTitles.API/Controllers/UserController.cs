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
    [Authorize(Policy = "MustBeAdmin")]
    [Route("api/users")]


    public class UserController : ControllerBase
    {
        private readonly INetflixTitlesRepository _netflixTitlesRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;
        const int maxUserPageSize = 50;

        public UserController(INetflixTitlesRepository netflixTitlesRepository,
            IMapper mapper,
            ILogger<UserController> logger)
        {
            _netflixTitlesRepository = netflixTitlesRepository ?? throw new ArgumentNullException(nameof(netflixTitlesRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]

        public async Task<ActionResult<IEnumerable<User>>> GetUsers(
            string? name, string? userType, string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            pageSize = (pageSize > maxUserPageSize) ? maxUserPageSize : pageSize;
            var (users, paginationMetadata) = await _netflixTitlesRepository
                .GetUsersAsync(name, userType, searchQuery, pageNumber, pageSize);

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            return Ok(_mapper.Map<IEnumerable<UserWithoutListsDto>>(users));
        }

        [HttpGet("{id}", Name = "GetUser")]

        public async Task<ActionResult<User?>> GetUser(int id)
        {
            var user = await _netflixTitlesRepository.GetUserAsync(id);
            if (user == null)
            {
                _logger.LogInformation($"User with id {id} was not found when accessing users.");
                return NotFound();
            }

            return Ok(_mapper.Map<UserDto>(user));
        }
    }
}
