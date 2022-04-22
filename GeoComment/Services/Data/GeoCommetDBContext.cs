using GeoComment.Models;
using Microsoft.EntityFrameworkCore;

namespace GeoComment.Services.Data
{
    public class GeoCommetDBContext : DbContext
    {
        public GeoCommetDBContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Comment> Comments { get; set; }

    }
}
