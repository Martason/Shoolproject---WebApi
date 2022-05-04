using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using GeoComment.Models;
using GeoComment.Services.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace GeoComment.Controllers
{
    [Route("api/geo-comments")]
    [ApiController]
    [ApiVersion("0.1")]

    public class GeoCommentsControllerV01 : ControllerBase
    {

        private readonly GeoCommentDbContext _context;
 
        public GeoCommentsControllerV01(GeoCommentDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateComment(DtoNewCommen_v01 newComment)
        {
            var comment = new Comment()
            {
                Message = newComment.Message,
                Longitude = newComment.Longitude,
                Latitude = newComment.Latitude,
                Author = newComment.Author,
                Created = DateTime.UtcNow,
                Title = newComment.Title != null ? newComment.Title : newComment.Message.Split(" ")[0]
            };

            try
            {
                await _context.AddAsync(comment);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetComment), new {id=comment.Id}, comment);
            }
            catch (Exception)
            {
                return StatusCode(404);
            }
       
        }

        [Route("{id:int}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DtoResponseComment_V01>> GetComment(int id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
            if (comment == null) return StatusCode(404);

            var returnComment = new DtoResponseComment_V01()
            {
                Id = comment.Id,
                Author = comment.Author,
                Message = comment.Message,
                Longitude = comment.Longitude,
                Latitude = comment.Latitude,

                // Email =
                //     "xxxx@" +
                //     user.Email.Split("@")[1],
                // CreditCardNumber =
                //     user.CreditCardNumber.Substring(0, 5) +
                //     "xxxx xxxx xxxx",
            };

            return Ok(returnComment);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<DtoResponseComment_V01>>> GetComment([BindRequired]double minLon, [BindRequired] double maxLon, [BindRequired] double minLat,
            [BindRequired] double maxLat)
        {
            
            var comments = await _context.Comments
                .Where(c => 
                    c.Longitude >= minLon &&
                    c.Longitude <= maxLon &&
                    c.Latitude >= minLat &&
                    c.Latitude <= maxLat)
                .ToListAsync();
            
            if (comments.Count < 0) return StatusCode(404);


            return Ok(comments);
        }

    }
}
