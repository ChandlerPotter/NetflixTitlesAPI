using System;
using Microsoft.EntityFrameworkCore;
using NetflixTitles.API.DbContexts;
using NetflixTitles.API.Entities;
using NetflixTitles.API.Models;

namespace NetflixTitles.API.Services
{
    public class NetflixTitlesRepository : INetflixTitlesRepository
    {
        private readonly netflix_appContext _context;

        public NetflixTitlesRepository(netflix_appContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<(IEnumerable<Entities.List>, PaginationMetadata)> GetAllListsAsync(
            string? name, string? searchQuery, int pageNumber, int pageSize)
        {
            var collection = _context.Lists.Include(l => l.User) as IQueryable<List>;

            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                collection = collection
                    .Where(l => l.ListName != null && l.ListName == name);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(l => l.ListName.Contains(searchQuery)
                || (l.User != null && l.User.UserName.Contains(searchQuery)));
            }

            var totalItemCount = await collection.CountAsync();
            var paginationMetadata = new PaginationMetadata(
                totalItemCount, pageSize, pageNumber);

            var collectionToReturn = await collection.OrderBy(l => l.ListName)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (collectionToReturn, paginationMetadata);
        }

        public async Task<(IEnumerable<Entities.List>, PaginationMetadata)> GetUserListsAsync(
            string? claimId, string? name, string? searchQuery, int pageNumber, int pageSize)
        {
            var collection = _context.Lists
                .Where(l => l.UserId.ToString() == claimId)
                .Include(l => l.User) as IQueryable<List>;

            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                collection = collection
                    .Where(l => l.ListName != null && l.ListName == name);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(l => l.ListName.Contains(searchQuery));
            }

            var totalItemCount = await collection.CountAsync();
            var paginationMetadata = new PaginationMetadata(
                totalItemCount, pageSize, pageNumber);

            var collectionToReturn = await collection.OrderBy(l => l.ListName)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (collectionToReturn, paginationMetadata);
        }

        public async Task<Entities.List?> GetListAsync(int listId)
        {
            return await _context.Lists
                .Where(l => l.ListId == listId)
                .Include(l => l.TitleLists)
                .ThenInclude(tl => tl.Title)
                .Include(l => l.User)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ListExistsAsync(int listId)
        {
            return await _context.Lists.AnyAsync(l => l.ListId == listId);
        }

        public async Task<IEnumerable<TitleList>> GetTitlesForListAsync(int listId)
        {
            return await _context.TitleLists
                .Where(tl => tl.ListId == listId)
                .Include(t => t.Title)
                .ToListAsync();
        }

        public async Task<(IEnumerable<TitleList>, PaginationMetadata)> GetTitlesForListAsync(
            int listId, string? name, string? searchQuery, int pageNumber, int pageSize)
        {
            var collection = _context.TitleLists
                .Where(tl => tl.ListId == listId)
                .Include(tl => tl.Title) as IQueryable<TitleList>;

            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                collection = collection
                    .Where(tl => tl.Title != null && tl.Title.TitleName == name)
                    .Include(tl => tl.Title);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(tl => tl.Title != null
                && tl.Title.TitleName != null && tl.Title.TitleName.Contains(searchQuery)
                || (tl.Title != null && tl.Title.Description != null && tl.Title.Description.Contains(searchQuery))
                || (tl.Title != null && tl.Title.Cast != null && tl.Title.Cast.Contains(searchQuery)))
                .Include(tl => tl.Title);
            }

            var totalItemCount = await collection.CountAsync();
            var paginationMetadata = new PaginationMetadata(
                totalItemCount, pageSize, pageNumber);

            var collectionToReturn = await collection.OrderBy(t => t.Title!.TitleName)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (collectionToReturn, paginationMetadata);
        }

        public async Task<IEnumerable<Title>> GetAllTitlesAsync()
        {
            return await _context.Titles
                .OrderBy(t => t.TitleName)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Title>, PaginationMetadata)> GetAllTitlesAsync(
            string? type, string? genre, string? rating, string? searchQuery, int pageNumber, int pageSize)
        {
            var collection = _context.Titles as IQueryable<Title>;

            if (!string.IsNullOrWhiteSpace(type))
            {
                type = type.Trim();
                collection = collection.Where(t => t.Type == type);
            }

            if (!string.IsNullOrWhiteSpace(genre))
            {
                genre = genre.Trim();
                collection = collection.Where(t => t.ListedIn.Contains(genre));
            }

            if (!string.IsNullOrWhiteSpace(rating))
            {
                rating = rating.Trim();
                collection = collection.Where(t => t.Rating == rating);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(t => t.TitleName.Contains(searchQuery)
                || (t.Description != null && t.Description.Contains(searchQuery))
                || (t.Cast != null && t.Cast.Contains(searchQuery))
                || (t.Director != null && t.Director.Contains(searchQuery)));
            }

            var totalItemCount = await collection.CountAsync();
            var paginationMetadata = new PaginationMetadata(
                totalItemCount, pageSize, pageNumber);

            var collectionToReturn = await collection.OrderBy(t => t.TitleName)
                .Skip(pageSize * (pageNumber -1))
                .Take(pageSize)
                .ToListAsync();

            return (collectionToReturn, paginationMetadata);
        }

        public async Task<Title?> GetTitleAsync(int? titleId)
        {
            return await _context.Titles
                .Where(t => t.TitleId == titleId)
                .Include(t => t.TitleLists)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.Users
                .OrderBy(u => u.UserId)
                .ToListAsync();
        }

        public async Task<(IEnumerable<User>, PaginationMetadata)> GetUsersAsync(
            string? name, string? userType, string? searchQuery, int pageNumber, int pageSize)
        {
            var collection = _context.Users as IQueryable<User>;
            
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                collection = collection.Where(u => u.UserName == name);
            }

            if (!string.IsNullOrWhiteSpace(userType))
            {
                userType = userType.Trim();
                collection = collection.Where(u => u.UserType == userType);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(u => u.UserName.Contains(searchQuery)
                || (u.Email != null && u.Email.Contains(searchQuery)));
            }

            var totalItemCount = await collection.CountAsync();
            var paginationMetadata = new PaginationMetadata(
                totalItemCount, pageSize, pageNumber);

            var collectionToReturn = await collection.OrderBy(u => u.UserName)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();


            return (collectionToReturn, paginationMetadata);
        }

        public async Task<User?> GetUserAsync(int userId)
        {
            return await _context.Users
                .Where(u => u.UserId == userId)
                .Include(u => u.Lists)
                .ThenInclude(l => l.TitleLists)
                .ThenInclude(tl => tl.Title)
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserAsync(string? userName)
        {
            return await _context.Users
                .Where(u => u.UserName == userName)
                .Include(u => u.Lists)
                .ThenInclude(l => l.TitleLists)
                .ThenInclude(tl => tl.Title)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ListNameMatchesListId(int listId, string? userClaim)
        {
            return await _context.Lists.AnyAsync(l => l.ListId == listId && l.UserId.ToString() == userClaim);
        }

        public async Task AddListAsync(Entities.List listToAdd)
        {
            await _context.Lists.AddAsync(listToAdd);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        public async Task AddTitleToListAsync(TitleList titleToAdd)
        {
            await _context.TitleLists.AddAsync(titleToAdd);
        }

        public async Task<bool> TitleExistsInList(int listId, int titleId)
        {
            return await _context.TitleLists.AnyAsync(tl => tl.ListId == listId && tl.TitleId == titleId);
        }

        public async Task<TitleList?> GetTitleForList(int listId, int titleId)
        {
            return await _context.TitleLists
               .Where(tl => tl.ListId == listId && tl.TitleId == titleId)
               .FirstOrDefaultAsync();
        }

        public void DeleteTitleFromList(TitleList tl)
        {
            _context.TitleLists.Remove(tl);
        }

        public void DeleteTitlesFromList(IEnumerable<TitleList> titlesToRemove)
        {
            _context.TitleLists.RemoveRange(titlesToRemove);
        }

        public void DeleteList(List list)
        {
            _context.Lists.Remove(list);
        }

        public void AddUser(User userToAdd)
        {
            _context.Users.Add(userToAdd);
        }
    }
}

