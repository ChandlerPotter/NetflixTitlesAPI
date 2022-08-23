using System;
namespace NetflixTitles.API.Models
{
    public class RegisterUserDto
    {
        public string UserName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

    }
}

