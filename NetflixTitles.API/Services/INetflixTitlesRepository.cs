using System;
using NetflixTitles.API.Entities;
using NetflixTitles.API.Models;

namespace NetflixTitles.API.Services
{
    public interface INetflixTitlesRepository
    {
        Task<(IEnumerable<Entities.List>, PaginationMetadata)> GetAllListsAsync(
            string? name, string? searchQueary, int pageNumber, int pageSize);

        Task<(IEnumerable<Entities.List>, PaginationMetadata)> GetUserListsAsync(
            string? claimId, string? name, string? searchQuery, int pageNumber, int pageSize);

        Task<Entities.List?> GetListAsync(int listId);

        Task<bool> ListExistsAsync(int listId);

        Task<IEnumerable<TitleList>> GetTitlesForListAsync(int listId);

        Task<(IEnumerable<TitleList>, PaginationMetadata)> GetTitlesForListAsync(
            int listId, string? name, string? searchQuery, int pageNumber, int pageSize);

        Task<IEnumerable<Title>> GetAllTitlesAsync();

        Task<(IEnumerable<Title>, PaginationMetadata)> GetAllTitlesAsync(
            string? type, string? genre, string? rating, string? searchQuery, int pageNumber, int pageSize);

        Task<Title?> GetTitleAsync(int? titleId);

        Task<IEnumerable<User>> GetUsersAsync();

        Task<(IEnumerable<User>, PaginationMetadata)> GetUsersAsync(
            string? name, string? userType, string? searchQuery, int pageNumber, int pageSize);

        Task<User?> GetUserAsync(int userId);

        Task<User?> GetUserAsync(string? userName);

        Task<bool> ListNameMatchesListId(int listId, string? userClaim);

        Task AddListAsync(Entities.List listToAdd);

        Task<bool> SaveChangesAsync();

        Task AddTitleToListAsync(TitleList titleToAdd);

        Task<bool> TitleExistsInList(int listId, int titleId);

        Task<TitleList?> GetTitleForList(int listId, int titleId);

        void DeleteTitleFromList(TitleList tl);

        void DeleteTitlesFromList(IEnumerable<TitleList> titlesToRemove);

        void DeleteList(Entities.List list);

        void AddUser(User userToAdd);
    }
}

