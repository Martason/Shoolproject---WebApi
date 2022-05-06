
using System.Runtime.InteropServices;
using System.Security.Claims;
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
            /*
             * The following example extracts the claims presented by a user in an HTTP request and writes them
             * to the HTTP response. The current user is read from the HttpContext as a ClaimsPrincipal.
             * The claims are then read from it and then are written to the response.
             *
             * ClaimsPrincipal exposes a collection of identities, each of which is a ClaimsIdentity.
             * In the common case, this collection, which is accessed through the Identities property,
             * will only have a single element.
             *
             * ClaimTypes.Name is for username and ClaimTypes.NameIdentifier specifies identity of
             * the user as object perspective. If you add them in a kind of ClaimIdentity object that
             * provides you to reach User.Identity methods(for example in the dotnet world) which are
             * GetUserName() and GetUserId().
             */

            //var username = _httpContextAccessor.HttpContext.User.Identity.Name;

            var principal = HttpContext.User;
            var userId = principal.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            //var username = principal.FindFirst(c => c.Type == ClaimTypes.Name).Value;

            var user = await _geoUserService.GetUser(userId);
            if (user == null) return Unauthorized();

            try
            {
                var newComment = new Comment()
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

                //TODO flytta bort detta

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
            catch (Exception)
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
                        Body = new ()
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


            //enligt cleanCode lägg detta i en egen metod kap. 7 sid 105

            try
            {
                var deletedComment = await _geoCommentManager.DeleteComment(commentToDelete, user);
                if (deletedComment == null) return StatusCode(401);
                var responseComment = new DtoResponseComment_v02()
                {
                    Id = deletedComment.Id,
                    Longitude = deletedComment.Longitude,
                    Latitude = deletedComment.Latitude,
                    Body = new()
                    {
                        Title = deletedComment.Title,
                        Message = deletedComment.Message,
                        Author = deletedComment.Author,
                    }
                };

                return Ok(responseComment);
            }
            catch
            {
                return StatusCode(500);
            }

            

        }

        




    }
}
