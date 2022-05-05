
using System.Runtime.InteropServices;
using GeoComment.DTO;
using GeoComment.Models;
using GeoComment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        public async Task<ActionResult<DtoResponseComment_v02>> GetComment(int id)
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

     
        [HttpGet]
        [Route("{username}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DtoResponseComment_v02>>> GetComment(string username)
        {
            var comments = await _geoCommentManager.GetComment(username);

            if (comments.Count == 0) return StatusCode(404);

            var returnComments = new List<DtoResponseComment_v02>();
            foreach (var comment in comments)
            {
                returnComments.Add(
                    new DtoResponseComment_v02()
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
                    });

            };

            return Ok(returnComments);
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DtoResponseComment_v02>>> GetComment([BindRequired] double minLon, [BindRequired] double maxLon, [BindRequired] double minLat,
            [BindRequired] double maxLat)
        {
            var comments =
                await _geoCommentManager.GetComment(minLon, maxLon, minLat, maxLat);

            if (comments.Count == 0) return StatusCode(404);

            var returnComments = new List<DtoResponseComment_v02>();
            foreach (var comment in comments)
            {
                returnComments.Add(
                    new DtoResponseComment_v02()
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
                    });

            };

            return Ok(returnComments);
        }

        [HttpDelete]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteComment([BindRequired] int id)
        {
            var commentToDelete = await _geoCommentManager.GetComment(id);
            if (commentToDelete == null) return StatusCode(404);
            return null;

        }

        




    }
}
