using System;
using System.Collections.Generic;

namespace NetflixTitles.API.Entities
{
    public partial class User
    {
        public User()
        {
            Lists = new HashSet<List>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? UserType { get; set; }
        public DateTime? DateCreated { get; set; }

        public virtual ICollection<List> Lists { get; set; }
    }
}
