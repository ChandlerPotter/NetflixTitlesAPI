using System;
namespace NetflixTitles.API.Models
{
    public class AllTitlesDto
    {
        public int TitleId { get; set; }

        public string? TitleName { get; set; }

        public string? Type { get; set; }

        public string? Director { get; set; }

        public string? Cast { get; set; }

        public string? Country { get; set; }

        public DateOnly? DateAdded { get; set; }

        public short? ReleaseYear { get; set; }

        public string? Rating { get; set; }

        public string? Duration { get; set; }

        public string ListedIn { get; set; } = null!;

        public string Description { get; set; } = null!;
    }
}

