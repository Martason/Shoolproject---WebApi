using GeoComment.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GeoComment.Services.Data
{
    public class GeoCommentDbContext :IdentityDbContext<User>
    {
        public GeoCommentDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<User> Users { get; set; }
        

    }
}
