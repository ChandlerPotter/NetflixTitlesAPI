using System;
using NetflixTitles.API.Entities;

namespace NetflixTitles.API.Models
{
    public class ListWithoutTitlesDto
    {
        public int ListId { get; set; }

        public string? UserName { get; set; }

        public string ListName { get; set; } = String.Empty;
    }
}