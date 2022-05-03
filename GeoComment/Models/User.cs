using Microsoft.AspNetCore.Identity;

namespace GeoComment.Models
{
    public class User : IdentityUser
    {
        public ICollection<Comment> Comments { get; set; }
    }
}
