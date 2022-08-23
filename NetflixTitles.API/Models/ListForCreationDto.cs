using System;
using NetflixTitles.API.Entities;

namespace NetflixTitles.API.Models
{
    public class ListForCreationDto
    {
        public string ListName { get; set; } = null!;

        public int? UserId { get; set; }
    }
}

