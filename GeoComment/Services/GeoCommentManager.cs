using GeoComment.DTO;
using GeoComment.Models;
using GeoComment.Services.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GeoComment.Services
{
    public class GeoCommentManager
    {
        private readonly GeoCommentDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public readonly UserManager<User> _userManager;

        public GeoCommentManager(GeoCommentDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<Comment?> CreateComment(DtoNewComment_v02 newComment)
        {

            //TODO måste lägga till en user istället för string på authour
            var comment = new Comment()
            {
                Message = newComment.Body.Message,
                Longitude = newComment.Longitude,
                Latitude = newComment.Latitude,
                Author = _httpContextAccessor.HttpContext.User.Identity.Name,
                Created = DateTime.UtcNow,
                Title = newComment.Body.Title != null ? newComment.Body.Title : newComment.Body.Message.Split(" ")[0]
            };

            try
            {
                await _context.AddAsync(comment);
                await _context.SaveChangesAsync();
                return comment;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public async Task<Comment?> GetComment(int id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
            return comment ?? null;
        }
        public async Task<List<Comment?>> GetComment(string username)
        {
            var comment = await _context.Comments.Where(c => c.Author == username)
                .ToListAsync();
            
            return comment ?? null;
        }

        
        public async Task<List<Comment>> GetComment(double minLon, double maxLon, double minLat, double maxLat)
        {
            var comments = await _context.Comments
                .Where(c =>
                    c.Longitude >= minLon &&
                    c.Longitude <= maxLon &&
                    c.Latitude >= minLat &&
                    c.Latitude <= maxLat)
                .ToListAsync();
            return comments;
        }

        public async Task<DtoResponseComment_v02> DeleteComment(Comment comment)
        {
          //  var userToLogin = await _userManager.FindByNameAsync(user.UserName);
          //  if (userToLogin == null) return null;

            return null;
        }
    }
}
