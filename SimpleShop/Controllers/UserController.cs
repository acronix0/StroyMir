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
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            var role = await _userManager.GetRolesAsync(user);
            var dto = _mapper.Map<UserRegistrationDto>(user);
            dto.Role = role.First();
            return Ok(dto);
        }

       
        

        [HttpPost("change-info")]
        public async Task<IActionResult> ChangeInfo([FromBody] UserRegistrationDto userDto)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null || userDto == null)
                return BadRequest();

            var result = await _repositoryManager.UserAuthentication.ChangeInfo(user, userDto);
            if (result.EmailChangeSucceeded && result.PhoneChangeSucceeded)
                return Ok(new { Token = result.Token });

            var errors = string.Join("; ", result.Errors);
            return BadRequest(new { Errors = errors });
        }

        
    }
}
