using System;
using System.Collections.Generic;

namespace NetflixTitles.API.Entities
{
    public partial class TitleList
    {
        public int Id { get; set; }
        public int? ListId { get; set; }
        public int? TitleId { get; set; }
        public int? UserRating { get; set; }
        public bool? Watched { get; set; }

        public virtual List? List { get; set; }
        public virtual Title? Title { get; set; }
    }
}
