using System;
using Microsoft.AspNetCore.Mvc;
using NetflixTitles.API.Services;
using AutoMapper;
using NetflixTitles.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using System.Text.Json;

namespace NetflixTitles.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/lists")]


    public class ListsController : ControllerBase
    {
        private readonly INetflixTitlesRepository _netflixTitlesRepository;
        private readonly ILogger<ListsController> _logger;
        private readonly IMapper _mapper;
        const int maxListsPageSize = 20;
        const string USER_CLAIM_ID_STR = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

        public ListsController(INetflixTitlesRepository netflixTitlesRepository,
            ILogger<ListsController> logger,
            IMapper mapper)
        {
            _netflixTitlesRepository = netflixTitlesRepository ?? throw new ArgumentNullException(nameof(netflixTitlesRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ListWithoutTitlesDto>), 200)]
        [ProducesResponseType(typeof(IEnumerable<ListWithoutTitlesDto>), 404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ListWithoutTitlesDto>>> GetLists(
            string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            pageSize = (pageSize > maxListsPageSize) ? maxListsPageSize : pageSize;

            var userIdClaim = User.Claims.FirstOrDefault(
                l => l.Type == USER_CLAIM_ID_STR)?.Value;

            var userRoleClaim = User.Claims.FirstOrDefault(u => u.Type == "user_type")?.Value;
            if (userRoleClaim == "admin")
            {
                var (adminListsToReturn, adminPaginationMetadata) =
                    await _netflixTitlesRepository.GetAllListsAsync(name, searchQuery, pageNumber, pageSize);

                Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(adminPaginationMetadata));

                return Ok(_mapper.Map<IEnumerable<ListWithoutTitlesDto>>(adminListsToReturn));
            }
            if (userIdClaim == null)
            {
                _logger.LogInformation($"User with id {userIdClaim} was not found when accessing lists");
                return NotFound();
            }

            var (listsToReturn, paginationMetadata) =
                await _netflixTitlesRepository.GetUserListsAsync(userIdClaim, name, searchQuery, pageNumber, pageSize);

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            return Ok(_mapper.Map<IEnumerable<ListWithoutTitlesDto>>(listsToReturn));
        }

        [HttpGet("{id}", Name = "GetList")]

        public async Task<ActionResult<ListDto>> GetList(int id)
        {
            var userIdClaim = User.Claims.FirstOrDefault(
                l => l.Type == USER_CLAIM_ID_STR)?.Value;

            var userRoleClaim = User.Claims.FirstOrDefault(u => u.Type == "user_type")?.Value;

            if (userRoleClaim == "admin")
            {
                var adminListToReturn = await _netflixTitlesRepository.GetListAsync(id);
                return Ok(_mapper.Map<ListDto>(adminListToReturn));
            }

            var listToReturn = await _netflixTitlesRepository.GetListAsync(id);

            if (listToReturn == null)
            {
                _logger.LogInformation($"List with id {id} was not found when accessing lists");
                return NotFound();
            }

            if (!await _netflixTitlesRepository.ListNameMatchesListId(id, userIdClaim))
            {
                _logger.LogInformation("Forbidded to access this list. User does not own it.");
                return Forbid();
            }

            return Ok(_mapper.Map<ListDto>(listToReturn));
        }

        [HttpPost]

        public async Task<ActionResult<ListDto>> CreateList(ListForCreationDto listForCreationDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(
                l => l.Type == USER_CLAIM_ID_STR)?.Value;
            if (userIdClaim == null)
            {

                return Forbid();
            }

            listForCreationDto.UserId = Int32.Parse(userIdClaim);

            var finalList = _mapper.Map<Entities.List>(listForCreationDto);

            await _netflixTitlesRepository.AddListAsync(finalList);
            await _netflixTitlesRepository.SaveChangesAsync();

            var createdList = _mapper.Map<Models.ListDto>(finalList);

            return CreatedAtRoute("GetList",
                new
                {
                    id = createdList.ListId
                },
                createdList);
        }

        [HttpPatch("{id}")]

        public async Task<ActionResult> PatchUpdateList(
            int id,
            JsonPatchDocument<ListForUpdateDto> patchDocument)
        {
            if (!await _netflixTitlesRepository.ListExistsAsync(id))
            {
                _logger.LogInformation($"List with id {id} was not found when updating lists");
                return NotFound();
            }

            var listEntity = await _netflixTitlesRepository.GetListAsync(id);
            var listToPatch = _mapper.Map<ListForUpdateDto>(listEntity);

            patchDocument.ApplyTo(listToPatch, ModelState);

            //Checks if patch document is attempting to "update" data that the model does not contain.
            //Also checks if the listToPatch is valid after patch document is applied. 
            if (!ModelState.IsValid || !TryValidateModel(listToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(listToPatch, listEntity);
            await _netflixTitlesRepository.SaveChangesAsync();

            return NoContent();

        }


        [HttpDelete("{id}")]

        public async Task<ActionResult> DeleteList(int id)
        {
            var listToDelete = await _netflixTitlesRepository.GetListAsync(id);
            if (listToDelete == null)
            {
                _logger.LogInformation($"List with id {id} was not found when updating lists");
                return NotFound();
            }

            var titlesToDelete = await _netflixTitlesRepository.GetTitlesForListAsync(id);
            _netflixTitlesRepository.DeleteTitlesFromList(titlesToDelete);

            _netflixTitlesRepository.DeleteList(listToDelete);
            await _netflixTitlesRepository.SaveChangesAsync();

            _logger.LogInformation($"List with id {id} has been removed");

            return NoContent();
        }
    }
}

