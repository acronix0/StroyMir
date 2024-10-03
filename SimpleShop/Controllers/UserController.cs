using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet("get-users")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> GetUsers()
        {
            // Получаем всех пользователей
            var users = await _userManager.Users.ToListAsync();

            // Создаем список для DTO
            var usersDto = new List<UserRegistrationDto>();

            foreach (var user in users)
            {
                // Получаем роли пользователя
                var roles = await _userManager.GetRolesAsync(user);

                // Маппим пользователя на DTO
                var dto = _mapper.Map<UserRegistrationDto>(user);

                // Добавляем первую роль (или объединяем все роли, если нужно)
                dto.Role = roles.FirstOrDefault();

                usersDto.Add(dto);
            }

            return Ok(usersDto);
        }
        [HttpPost("unblock-users")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> UnBlockUser([FromBody] string userEmail)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null || userEmail == null|| userEmail == "")
                    return BadRequest();

                var result = await _repositoryManager.UserAuthentication.UnBlocked(user);
                if (result.Succeeded)
                {
                    return Ok();
                }
                return BadRequest(result);
            }
            catch (Exception e)
            {

                return BadRequest(new { Errors = e.Message });
            }
        }  [HttpPost("block-users")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> BlockUser([FromBody] string userEmail)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null || userEmail == null|| userEmail == "")
                    return BadRequest();

                var result = await _repositoryManager.UserAuthentication.Blocked(user);
                if (result.Succeeded)
                {
                    return Ok();
                }
                return BadRequest(result);
            }
            catch (Exception e)
            {

                return BadRequest(new { Errors = e.Message });
            }
        }
        [HttpPost("change-info")]
        public async Task<IActionResult> ChangeInfo([FromBody] UserRegistrationDto userDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                if (user == null || userDto == null)
                    return BadRequest();

                var result = await _repositoryManager.UserAuthentication.ChangeInfo(user, userDto);
                if (result.EmailChangeSucceeded && result.PhoneChangeSucceeded && result.PasswordChangeSucceeded)
                    return Ok(new { Token = result.Token });

                var errors = string.Join("; ", result.Errors);
                return BadRequest(new { Errors = errors });
            }
            catch (Exception e )
            {

                return BadRequest(new { Errors = e.Message });
            }
           
        }
        
        
    }
}
