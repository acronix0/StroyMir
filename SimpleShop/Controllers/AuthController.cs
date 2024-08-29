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
    [Route("api/auth")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        private MailManager _mailManager;
        private UserManager<ApplicationUser> _userManager;
        public AuthController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, UserManager<ApplicationUser> userManager, MailManager mailManager) : base(repository, logger, mapper)
        {
            _mailManager = mailManager;
            _userManager = userManager;
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
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
                return BadRequest();
            var password = GeneratePassword(6);
            var x = await _repositoryManager.UserAuthentication.ChangePassword(user, password);
            if (x.Succeeded)
            {
                await _mailManager.SendMail("Восстановление учетной записи",
                    $"Добрый день!\n\n" +
                    $"На сайте DMTrade был сделан запрос на изменение вашего пароля.\n\n" +
                    $"Новый пароль: {password}\n\n\n" +
                    $"С уважением,\nадминистрация DMTrade\nhttps://dm-trade.pro", Email);
                return Ok();
            }
            return BadRequest();
        }
        static string GeneratePassword(int length)
        {
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string specialChars = "!@#$%^&*()-_=+";

            Random random = new Random();

            char[] password = new char[length];
            password[0] = upperChars[random.Next(upperChars.Length)];
            password[1] = lowerChars[random.Next(lowerChars.Length)];
            password[2] = digits[random.Next(digits.Length)];
            password[3] = specialChars[random.Next(specialChars.Length)];

            string allChars = upperChars + lowerChars + digits + specialChars;
            for (int i = 4; i < length; i++)
            {
                password[i] = allChars[random.Next(allChars.Length)];
            }

            // Перемешиваем символы для случайного порядка
            return new string(password.OrderBy(x => random.Next()).ToArray());
        }
    }
}
