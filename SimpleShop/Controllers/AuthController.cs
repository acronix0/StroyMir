using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleShop.Core.Dtos;
using SimpleShop.Core.Model;
using SimpleShop.Repo.Data;
using SimpleShop.Service.Interfaces;
using SimpleShop.Service.Services;

namespace SimpleShop.WebApi.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        
        public AuthController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, UserManager<ApplicationUser> userManager) : base(repository, logger, mapper)
        {
        }

        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] UserLoginDto user)
        {
            var valid = await _repositoryManager.UserAuthentication.ValidateUserAsync(user);
            if (valid)
            {
               return  Ok(new { Token = await _repositoryManager.UserAuthentication.CreateTokenAsync() });
            }
            
            return Unauthorized();
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto user)
        {
            var result = await  _repositoryManager.UserAuthentication.AddUserAsync(user);
            if (result.Succeeded)
            {
                _repositoryManager.UserAuthentication.AddUserRoleAsync(user, "Admin").Wait();
                return Ok(new { Token = await _repositoryManager.UserAuthentication.CreateTokenAsync() });
            }
            var errorMessage = result.Errors.Select(e => e.Description).ToList();
            return BadRequest( new { status = "error", message = String.Join(", ",errorMessage)});
        }
        [Authorize (Roles =UserRoles.Admin)]
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] UserRegistrationDto user)
        {
            user.Role = "Admin";
            return await Register(user);
        }
    }
}
