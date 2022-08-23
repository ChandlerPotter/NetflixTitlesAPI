using System;
using NetflixTitles.API.Entities;

namespace NetflixTitles.API.Models
{
    public class ListDto
    {
        public int ListId { get; set; }

        public string? UserName { get; set; }

        public string ListName { get; set; } = String.Empty;

        public int NumberOfTitles
        {
            get
            {
                return Titles.Count;
            }
        }

        public ICollection<TitleDto> Titles { get; set; }
            = new List<TitleDto>();
    }
}

