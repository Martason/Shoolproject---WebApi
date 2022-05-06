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

        public GeoCommentManager(GeoCommentDbContext context)
        {
            _context = context;
          
        }
        /// <summary>
        /// Saves the provided comment in the database
        /// </summary>
        /// <param name="comment"></param>
        /// <returns>The saved comment</returns>
        public async Task<Comment?> CreateComment(Comment comment)
        {
            await _context.AddAsync(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        /// <summary>
        /// Gets the comment that matches with provided id from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The comment that matches if successful or null</returns>
        public async Task<Comment?> GetComment(int id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
            return comment ?? null;
        }

        /// <summary>
        /// Gets a list with comment that matches with provided username from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List containing matching comments</returns>
        public async Task<List<Comment>> GetComment(string username)
        {
            var comment = await _context.Comments.Where(c => c.Author == username)
                .ToListAsync();

            return comment;
        }

        /// <summary>
        /// Gets a list with comment that matches the provided coordinates
        /// </summary>
        /// <param name="minLon"></param>
        /// <param name="maxLon"></param>
        /// <param name="minLat"></param>
        /// <param name="maxLat"></param>
        /// <returns>List containing matching comments</returns>

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

        /// <summary>
        /// Deletes the provided comment if the provided user is the author.
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="user"></param>
        /// <returns>The deleted comment if successful or null</returns>
        public async Task<Comment?> DeleteComment(Comment comment, User user)
        {
            var userIsCommentAuthor = comment.Author == user.UserName;

            if (!userIsCommentAuthor) return null;

            var copyDeletedComment = new Comment
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
