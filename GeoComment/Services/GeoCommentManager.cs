using GeoComment.DTO;
using GeoComment.Models;
using GeoComment.Services.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GeoComment.Services
{
    public class GeoCommentManager
    {
        //TODO endast hantering av comment! Ta bort bort Dto hantering här

        private readonly GeoCommentDbContext _context;
        public readonly UserManager<User> _userManager;

        public GeoCommentManager(GeoCommentDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<Comment?> CreateComment(Comment comment)
        {

            // Inte använda httpContextAccessor
            // Och ta in username i metoden. 

            await _context.AddAsync(comment);
                await _context.SaveChangesAsync();
                return comment;


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

        public async Task<Comment?> DeleteComment(Comment comment, User user)
        {
            var userIsCommentAuthor = comment.Author == user.UserName;

            if (!userIsCommentAuthor) return null;
            
            var copyDeletedComment = new Comment()
            {
                Id = comment.Id,
                Longitude = comment.Longitude,
                Latitude = comment.Latitude,
                Title = comment.Title,
                Message = comment.Message,
                Author = comment.Author,
                User = null
            };

            _context.Remove(comment);
            await _context.SaveChangesAsync();

            return copyDeletedComment;
        }
    }
}
