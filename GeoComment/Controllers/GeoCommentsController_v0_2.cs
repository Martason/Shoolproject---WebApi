
using System.Runtime.InteropServices;
using System.Security.Claims;
using GeoComment.DTO;
using GeoComment.Models;
using GeoComment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace GeoComment.Controllers
{
    [Route("api/geo-comments")]
    [ApiController]
    [ApiVersion("0.2")]
    public class GeoCommentsControllerV02 : ControllerBase
    {
        private readonly GeoCommentManager _geoCommentManager;
        private readonly GeoUserService _geoUserService;
        public GeoCommentsControllerV02(GeoCommentManager geoCommentManager, GeoUserService geoUserService)
        {
            _geoCommentManager = geoCommentManager;
            _geoUserService = geoUserService;
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateComment(DtoCommentInputV02 commentInput)
        {

            var principal = HttpContext.User;
            var userId = principal.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var user = await _geoUserService.GetUser(userId);
            if (user == null) return Unauthorized();

            try
            {
                var newComment = new Comment
                {
                    Message = commentInput.Body.Message,
                    Longitude = commentInput.Longitude,
                    Latitude = commentInput.Latitude,
                    Author = user.UserName,
                    Created = DateTime.UtcNow,
                    User = user,
                    Title = commentInput.Body.Title != null
                        ? commentInput.Body.Title
                        : commentInput.Body.Message.Split(" ")[0]
                };

                var comment = await _geoCommentManager.CreateComment(newComment);
                if (comment == null) return BadRequest();


                var returnComment = DtoResponseComment_v02.CreateResponseComment_v02(comment);

                return CreatedAtAction(nameof(GetComment), new {id = comment.Id}, returnComment);
            }
            catch (DbUpdateException)
            {
                return StatusCode(500);
            }
        }

        [Route("{id:int}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DtoResponseComment_v02>> GetComment(int id)
        {
            var comment = await _geoCommentManager.GetComment(id);

            if (comment == null) return StatusCode(404);

            var returnComment = DtoResponseComment_v02.CreateResponseComment_v02(comment);
            
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

            var returnComments = comments.Select(DtoResponseComment_v02.CreateResponseComment_v02).ToList();

            return Ok(returnComments);
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DtoResponseComment_v02>>> GetCommentAsync([BindRequired] double minLon, [BindRequired] double maxLon, [BindRequired] double minLat,
            [BindRequired] double maxLat)
        {
            var comments =
                await _geoCommentManager.GetComment(minLon, maxLon, minLat, maxLat);

            if (comments.Count == 0) return StatusCode(404);

            var returnComments = comments.Select(DtoResponseComment_v02.CreateResponseComment_v02).ToList();

            return Ok(returnComments);
        }

        [HttpDelete]
        [Authorize]
        [Route("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteComment([BindRequired] int id)
        {
            var commentToDelete = await _geoCommentManager.GetComment(id);
            if (commentToDelete == null) return StatusCode(404);

            var principal = HttpContext.User;
            var userId = principal.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return Unauthorized();

            var user = await _geoUserService.GetUser(userId);
            if (user == null) return Unauthorized();


            //cleanCode kap. 7 sid 105

            try
            {
                var deletedComment = await _geoCommentManager.DeleteComment(commentToDelete, user);
                if (deletedComment == null) return StatusCode(401);

                var returnComment = DtoResponseComment_v02.CreateResponseComment_v02(deletedComment);

                return Ok(returnComment);
            }
            catch(DbUpdateException)
            {
                return StatusCode(500);
            }

        }

        




    }
}
