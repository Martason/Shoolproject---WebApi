using GeoComment.DTO;
using GeoComment.Models;
using GeoComment.Services.Data;
using Microsoft.AspNetCore.Identity;

namespace GeoComment.Services
{
    public class GeoUserService
    {

        public readonly UserManager<User> _userManager;
        public readonly JwtManager _jwtManager;

        public GeoUserService(UserManager<User> userManager, JwtManager jwtManager)
        {
            _userManager = userManager;
            _jwtManager = jwtManager;
        }

        public async Task<User?> RegisterNewUser(DtoNewUserInputDto user)
        {
            var newUser = new User
            {
                UserName = user.UserName,
            };

            var identityResult = await _userManager.CreateAsync(newUser, user.Password);

            return identityResult.Succeeded ? await _userManager.FindByNameAsync(user.UserName) : null;
        }


        public async Task<string?> Login(DtoNewUserInputDto user)
        {
            var userToLogin = await _userManager.FindByNameAsync(user.UserName);
            if (userToLogin == null) return null;

            var passWordCheck = await _userManager.CheckPasswordAsync(userToLogin, user.Password);
            return passWordCheck ? _jwtManager.GenerateJwtToken(userToLogin) : null;
        }

        public async Task<User?> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return user ?? null;
        }
    }
}
