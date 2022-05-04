using GeoComment.Models;
using Microsoft.AspNetCore.Identity;

namespace GeoComment.Services.Data
{
    public class DatabaseHandler
    {
        private readonly GeoCommentDbContext _context;
        private readonly UserManager<User> _userManager;

        public DatabaseHandler(GeoCommentDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task SeedTestData()
        {
            var user = new User()
            {
                UserName = "TestUser",
            };

            await _userManager.CreateAsync(user, "Passw0rd!");
        }
        public async Task Recreate()
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();
        }

        public async Task CreateIfNotExist()
        {
            await _context.Database.EnsureCreatedAsync();
        }


    }
}
