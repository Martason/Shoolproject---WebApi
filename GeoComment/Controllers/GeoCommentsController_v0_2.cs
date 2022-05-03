using System.Text;
using GeoComment.Models;
using GeoComment.Services.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GeoComment.Controllers
{
    [Route("api/geo-comments")]
    [ApiController]
    [ApiVersion("0.2")]
    public class GeoCommentsController_v0_2 : ControllerBase
    {
        private readonly GeoCommentDbContext _context;
        private readonly UserManager<UserController> _userManager;

        public GeoCommentsController_v0_2(GeoCommentDbContext context, UserManager<UserController> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        private string FromBase64(string base64)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateComment(NewCommentDto newComment)
        {
            //TODO kan man hantera här så att message och author måste vara satta, not null, men long och lat är valfritt? 
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
                return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, comment);
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PartiallyHiddenCommentDto>> GetComment(int id)
        {
            /*
            var authorizationHeader = Request.Headers.FirstOrDefault(header => header.Key == "Authorization");
            //Check authorizationHeader exists
            if (authorizationHeader.Key != null) return Unauthorized();

            //Check authorizationHeader is of type Basic
            if (!authorizationHeader.ToString().StartsWith("Basic ")) return Unauthorized();

            var Base64 = authorizationHeader.Value.ToString().Replace("Basic ", "");
            var userConvertedBase64 = FromBase64(Base64);
            var username = userConvertedBase64.Split(":")[0];
            var password = userConvertedBase64.Split(":")[1];

            var user = await _userManager.FindByNameAsync(username);

            //Check userExist
            if ( user == null) return Unauthorized();
            //Check passwordMatch
            if (!await _userManager.CheckPasswordAsync(user, password)) return Unauthorized();
            */

            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
            if (comment == null) return StatusCode(404);

            var partiallyHiddenComment = new PartiallyHiddenCommentDto()
            {
                Id = comment.Id,
                Author = comment.Author,
                Message = comment.Message,
                Longitude = comment.Longitude,
                Latitude = comment.Latitude,
                Title = comment.Title
            };

            return Ok(partiallyHiddenComment);
        }

    }
}
