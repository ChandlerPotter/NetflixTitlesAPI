using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using NetflixTitles.API.Entities;
using NetflixTitles.API.Models;
using NetflixTitles.API.Services;

namespace NetflixTitles.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/lists/{listId}/titles")]
    

    public class TitlesController : ControllerBase
    {
        private readonly INetflixTitlesRepository _netflixTitlesRepository;
        private readonly ILogger<TitlesController> _logger;
        private readonly IMapper _mapper;
        const int maxTitlesPageSize = 25;
        const string USER_CLAIM_ID_STR = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

        public TitlesController(INetflixTitlesRepository netflixTitlesRepository,
            ILogger<TitlesController> logger,
            IMapper mapper)
        {
            _netflixTitlesRepository = netflixTitlesRepository ?? throw new ArgumentNullException(nameof(netflixTitlesRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TitleDto>>> GetTitlesForList(int listId,
            string? name, string? searchQuery, int pageNumber = 1, int pageSize = 5)
        {
            pageSize = (pageSize > maxTitlesPageSize) ? maxTitlesPageSize : pageSize;

            var userIdClaim = User.Claims.FirstOrDefault(
                l => l.Type == USER_CLAIM_ID_STR)?.Value;
            var userRoleClaim = User.Claims.FirstOrDefault(u => u.Type == "user_type")?.Value;

            if (userRoleClaim == "admin")
            {
                var (adminTitlesToReturn, adminPaginationMetadata) = await _netflixTitlesRepository
                .GetTitlesForListAsync(listId, name, searchQuery, pageNumber, pageSize);

                Response.Headers.Add("X-Pagination",
                    JsonSerializer.Serialize(adminPaginationMetadata));

                return Ok(_mapper.Map<IEnumerable<TitleDto>>(adminTitlesToReturn));
            }

            if (!await _netflixTitlesRepository.ListExistsAsync(listId))
            {
                _logger.LogInformation(
                    $"List with id {listId} was not found when accessing titles.");
                return NotFound();
            }

            if (!await _netflixTitlesRepository.ListNameMatchesListId(listId, userIdClaim))
            {
                return Forbid();
            }

            var (titlesForList, paginationMetadata) = await _netflixTitlesRepository
                .GetTitlesForListAsync(listId, name, searchQuery, pageNumber, pageSize);

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            return Ok(_mapper.Map<IEnumerable<TitleDto>>(titlesForList));
        }

        [HttpPost]

        public async Task<ActionResult<TitleForCreationDto>> AddTitleToList(
            int listId, TitleForCreationDto titleForCreationDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(
                l => l.Type == USER_CLAIM_ID_STR)?.Value;

            if (!await _netflixTitlesRepository.ListExistsAsync(listId))
            {
                _logger.LogInformation(
                    $"List with id {listId} was not found when accessing titles.");
                return NotFound();
            }

            if (!await _netflixTitlesRepository.ListNameMatchesListId(listId, userIdClaim))
            {
                return Forbid();
            }

            titleForCreationDto.ListId = listId;
            var finalTitle = _mapper.Map<Entities.TitleList>(titleForCreationDto);
            finalTitle.Title =
                await _netflixTitlesRepository.GetTitleAsync(finalTitle.TitleId);

            if (finalTitle.Title == null)
            {
                _logger.LogInformation(
                   $"Title with id {titleForCreationDto.TitleId} was not found when accessing titles.");
                return NotFound();
            }

            if (await _netflixTitlesRepository.TitleExistsInList(listId, titleForCreationDto.TitleId))
            {
                _logger.LogInformation($"Title with id {titleForCreationDto.TitleId} already exists in this list.");
                return BadRequest();
            }

            await _netflixTitlesRepository.AddTitleToListAsync(finalTitle);
            await _netflixTitlesRepository.SaveChangesAsync();

            var createdTitle = _mapper.Map<Models.TitleDto>(finalTitle);

            return CreatedAtRoute("GetTitle",
                new
                {
                    id = createdTitle.TitleId
                },
                createdTitle);
        }

        [HttpPatch("{id}")]

        public async Task<ActionResult> PatchUpdateTitle(int id,int listId,
            JsonPatchDocument<TitleForUpdateDto> patchDocument)
        {
            var userIdClaim = User.Claims.FirstOrDefault(
                l => l.Type == USER_CLAIM_ID_STR)?.Value;

            if (!await _netflixTitlesRepository.ListExistsAsync(listId))
            {
                _logger.LogInformation(
                    $"List with id {listId} was not found when accessing titles.");
                return NotFound();
            }

            if (!await _netflixTitlesRepository.ListNameMatchesListId(listId, userIdClaim))
            {
                return Forbid();
            }

            var titleEntity = await _netflixTitlesRepository.GetTitleForList(listId, id);
            var titleToPatch = _mapper.Map<TitleForUpdateDto>(titleEntity);

            patchDocument.ApplyTo(titleToPatch, ModelState);

            if (!ModelState.IsValid || !TryValidateModel(titleToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(titleToPatch, titleEntity);
            await _netflixTitlesRepository.SaveChangesAsync();

            return NoContent();

        }


        [HttpDelete("{id}")]

        public async Task<ActionResult> DeleteTitleFromList(int listId, int id)
        {
            var userIdClaim = User.Claims.FirstOrDefault(
                l => l.Type == USER_CLAIM_ID_STR)?.Value;

            if (!await _netflixTitlesRepository.ListExistsAsync(listId))
            {
                _logger.LogInformation(
                    $"List with id {listId} was not found when accessing titles.");
                return NotFound();
            }

            if (!await _netflixTitlesRepository.ListNameMatchesListId(listId, userIdClaim))
            {
                return Forbid();
            }

            var titleForList = await _netflixTitlesRepository.GetTitleForList(listId, id);

            if (titleForList == null)
            {
                return NotFound();
            }

            _netflixTitlesRepository.DeleteTitleFromList(titleForList);
            await _netflixTitlesRepository.SaveChangesAsync();

            _logger.LogInformation($"Title with id {id} was removed from list with id {listId}.");

            return NoContent();
        }

    } 
}
