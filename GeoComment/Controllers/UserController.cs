using GeoComment.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeoComment.Controllers
{
    [Route("api/user")]
    [ApiController]
    [ApiVersion("0.2")]
    public class UserController : ControllerBase
    {
        /*

        [Route("register")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RegisterUser(NewUserDto newUser)
        {
            var newUser = new User()
            {
               
            };

            try
            {
                await _context.AddAsync(newUser);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, comment);
            }
            catch (Exception)
            {
                return StatusCode(404);
            }

        }*/

    }
}
