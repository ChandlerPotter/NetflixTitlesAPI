using System;
namespace NetflixTitles.API.Models
{
    public class UserWithoutListsDto
    {
        public int UserId { get; set; }

        public string UserName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? UserType { get; set; }

        public DateTime? DateCreated { get; set; }
    }
}

