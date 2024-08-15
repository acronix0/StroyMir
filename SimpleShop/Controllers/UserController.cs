using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleShop.Core.Dtos;
using SimpleShop.Core.Model;
using SimpleShop.Repo.Data;
using SimpleShop.Service.Interfaces;
using System.Security.Claims;

namespace SimpleShop.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : BaseApiController
    {
        private UserManager<ApplicationUser> _userManager;
        public UserController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, UserManager<ApplicationUser> userManager) : base(repository, logger, mapper)
        {
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await _userManager.FindByEmailAsync(userEmail);
            var dto = _mapper.Map<UserRegistrationDto>(user);
            return Ok(dto);
        }
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(string userId, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return BadRequest();

            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (result.Succeeded)
                return Ok();
            return BadRequest(new { Status = "Error", Message = "Change password failed", errors = result.Errors });
        }
    }
}
