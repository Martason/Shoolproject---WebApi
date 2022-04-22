using System.ComponentModel.DataAnnotations;
using GeoComment.Models;
using GeoComment.Services.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GeoComment.Controllers
{
    [Route("api/geo-comments")]
    [ApiController]
    public class GeoCommentsController : ControllerBase
    {

        private readonly GeoCommetDBContext _context;
        public class NewComment //DTO data transfer object. Används bara för att föra över information från ett ställe till en annan
        {

            public string Message { get; set; }
            public int Longitude { get; set; }
            public int Latitude { get; set; }
            public string Author { get; set; }
        }

        public class PartiallyHiddenComment //DTO
        {
            public int Id { get; set; }
            public string Message { get; set; }
            public int Longitude { get; set; }
            public int Latitude { get; set; }
            public string Author { get; set; }
        }


        public GeoCommentsController(GeoCommetDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateComment(NewComment newComment)
        {
            if (newComment == null) return BadRequest();

            var comment = new Comment()
            {
                Message = newComment.Message,
                Longitude = newComment.Longitude,
                Latitude = newComment.Latitude,
                Author = newComment.Author,
                Created = DateTime.UtcNow
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
        public async Task<ActionResult<PartiallyHiddenComment>> GetComment(int id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
            if (comment == null) return StatusCode(404);

            var partiallyHiddenComment = new PartiallyHiddenComment()
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

            return Ok(partiallyHiddenComment);
        }

        //[Route("{minLon:int},{maxLon:int},{minLat:int},{maxLat:int}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<PartiallyHiddenComment>>> GetComment(int? minLon, int? maxLon, int? minLat,
            int? maxLat)
        {
            if (maxLat == null || minLat == null || maxLon == null || minLon == null )
            {
                return StatusCode(400);
            }

            var comments = await _context.Comments
                .Where(c => 
                    c.Longitude >= minLon &&
                    c.Longitude <= maxLon &&
                    c.Latitude >= minLat &&
                    c.Latitude <= maxLat)
                .ToListAsync();
            
            if (comments == null) return StatusCode(404);


            return Ok(comments);
        }



    }
}
