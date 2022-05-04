
using GeoComment.DTO;
using GeoComment.Models;
using GeoComment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace GeoComment.Controllers
{
    [Route("api/geo-comments")]
    [ApiController]
    [ApiVersion("0.2")]
    public class GeoCommentsControllerV02 : ControllerBase
    {
        private readonly GeoCommentManager _geoCommentManager;
        public GeoCommentsControllerV02(GeoCommentManager geoCommentManager)
        {
            _geoCommentManager = geoCommentManager;
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateComment(DtoNewComment_v02 newComment)
        {
            
            var comment = await _geoCommentManager.CreateComment(newComment);
            if (comment == null) return BadRequest();

            var returnComment = new DtoResponseComment_V01()
            {
                Id = comment.Id,
                Longitude = comment.Longitude,
                Latitude = comment.Latitude,
                Body = new()
                {
                    Title = comment.Title,
                    Message = comment.Message,
                    Author = comment.Author
                }
            };

            return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, returnComment);
        }

        [Route("{id:int}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DtoResponseComment_V01>> GetComment(int id)
        {
            var comment = await _geoCommentManager.GetComment(id);

            if (comment == null) return StatusCode(404);

            var returnComment = new DtoResponseComment_v02()
            {
                Id = comment.Id,
                Longitude = comment.Longitude,
                Latitude = comment.Latitude,
                Body = new () 
                {
                    Title = comment.Title,
                    Message = comment.Message,
                    Author = comment.Author
                }
            };

            return Ok(returnComment);
        }

        [Route("{id:int}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DtoResponseComment_V01>> GetComment(string username)
        {
            var comment = await _geoCommentManager.GetComment(id);

            if (comment == null) return StatusCode(404);

            var returnComment = new DtoResponseComment_v02()
            {
                Id = comment.Id,
                Longitude = comment.Longitude,
                Latitude = comment.Latitude,
                Body = new()
                {
                    Title = comment.Title,
                    Message = comment.Message,
                    Author = comment.Author
                }
            };

            return Ok(returnComment);
        }

    }
}
