using System;
using System.Collections.Generic;

namespace NetflixTitles.API.Entities
{
    public partial class List
    {
        public List()
        {
            TitleLists = new HashSet<TitleList>();
        }

        public int ListId { get; set; }
        public string ListName { get; set; } = null!;
        public int? UserId { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<TitleList> TitleLists { get; set; }
    }
}
