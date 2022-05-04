using GeoComment.DTO;
using GeoComment.Models;
using GeoComment.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace GeoComment.Services
{
    public class GeoCommentManager
    {
        private readonly GeoCommentDbContext _context;
        private readonly JwtManager _jwtManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GeoCommentManager(GeoCommentDbContext context, JwtManager jwtManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _jwtManager = jwtManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Comment?> CreateComment(DtoNewComment_v02 newComment)
        {
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
    }
}
