using System;
using System.Collections.Generic;

namespace NetflixTitles.API.Entities
{
    public partial class Title
    {
        public Title()
        {
            TitleLists = new HashSet<TitleList>();
        }

        public int TitleId { get; set; }
        public string TitleName { get; set; } = null!;
        public string? Type { get; set; }
        public string? Director { get; set; }
        public string? Cast { get; set; }
        public string? Country { get; set; }
        public DateOnly? DateAdded { get; set; }
        public short? ReleaseYear { get; set; }
        public string? Rating { get; set; }
        public string? Duration { get; set; }
        public int? UserRating { get; set; }
        public bool? Watched { get; set; }
        public string ListedIn { get; set; } = null!;
        public string Description { get; set; } = null!;

        public virtual ICollection<TitleList> TitleLists { get; set; }
    }
}
