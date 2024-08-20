using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using SimpleShop.Core.Dtos;
using SimpleShop.Core.Model;
using SimpleShop.Repo.GenericRepository.Service;
using SimpleShop.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Service.Services
{
    internal sealed class UserAuthenticationRepository : IUserAuthenticationRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private ApplicationUser? _user;

        public UserAuthenticationRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _mapper = mapper;
        }


        public async Task AddRoleIfNotExistAsync(string role)
        {
            var isExist = await _roleManager.RoleExistsAsync(role);
            if (isExist)
                return;
            await _roleManager.CreateAsync(new IdentityRole(role));

        }

        public async Task AddUserRoleAsync(UserRegistrationDto userRegistration, string role)
        {
            var user = await _userManager.FindByEmailAsync(userRegistration.Email);
            await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<IdentityResult> AddUserAsync(UserRegistrationDto userRegistration)
        {
            var user = _mapper.Map<ApplicationUser>(userRegistration);
            user.DisplayName = userRegistration.Name;
            user.UserName = userRegistration.Email;
            var result = await _userManager.CreateAsync(user, userRegistration.Password);
            if (result.Succeeded)
            {
                _user = await _userManager.FindByEmailAsync(userRegistration.Email);
            }
            return result;
        }
        public async Task<DetailedOperationResult> ChangeInfo(ApplicationUser user, UserRegistrationDto userRegistrationDto)
        {
            _user = user;
            var result = new DetailedOperationResult();
            if (userRegistrationDto.Password != "")
            {
                var passwordToken =  await _userManager.GeneratePasswordResetTokenAsync(user);
                var passResult = await _userManager.ResetPasswordAsync(user, passwordToken, userRegistrationDto.Password);
                if (passResult.Succeeded)
                {
                    result.PasswordChangeSucceeded = true;
                }
                else
                {
                    result.PasswordChangeSucceeded = false;
                    result.AddErrors(passResult.Errors, "Name Change");
                }
            }
            if (userRegistrationDto.Name != user.DisplayName)
            {
                user.DisplayName = userRegistrationDto.Name;
                var NameResult = await _userManager.UpdateAsync(user);
                if (NameResult.Succeeded)
                {
                    result.NameChangeSucceeded = true;
                }
                else
                {
                    result.NameChangeSucceeded = false;
                    result.AddErrors(NameResult.Errors, "Name Change");
                }
            }
            if (user.Email != userRegistrationDto.Email)
            {
                var token = await _userManager.GenerateChangeEmailTokenAsync(user, userRegistrationDto.Email);
                var emailResult = await _userManager.ChangeEmailAsync(user, userRegistrationDto.Email, token);
                
                
                if (emailResult.Succeeded)
                {
                    result.EmailChangeSucceeded = true;
                    await _userManager.SetUserNameAsync(user, userRegistrationDto.Email);
                }
                else
                {
                    result.EmailChangeSucceeded = false;
                    result.AddErrors(emailResult.Errors, "Email Change");
                }
            }
            else
            {
                result.EmailChangeSucceeded = true; // Никаких изменений не требуется
            }

            if (user.PhoneNumber != userRegistrationDto.Phone)
            {
                var token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, userRegistrationDto.Phone);
                var phoneResult = await _userManager.ChangePhoneNumberAsync(user, userRegistrationDto.Phone, token);
                if (phoneResult.Succeeded)
                {
                    result.PhoneChangeSucceeded = true;
                }
                else
                {
                    result.PhoneChangeSucceeded = false;
                    result.AddErrors(phoneResult.Errors, "Phone Change");
                }
            }
            else
            {
                result.PhoneChangeSucceeded = true; 
            }

            if (result.EmailChangeSucceeded && result.PhoneChangeSucceeded)
            {
                result.Token = await CreateTokenAsync(); 
            }

            return result;
        }

        public async Task<bool> ValidateUserAsync(UserLoginDto loginDto)
        {
            _user = await _userManager.FindByEmailAsync(loginDto.Email);
            var result = _user != null && await _userManager.CheckPasswordAsync(_user, loginDto.Password);
            return result;
        }

        public async Task<string> CreateTokenAsync()
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims();
            
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private SigningCredentials GetSigningCredentials()
        {
            var jwtConfig = _configuration.GetSection("jwtConfig");
            var key = Encoding.UTF8.GetBytes(jwtConfig["Secret"]);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _user.Email)
            };
            var roles = await _userManager.GetRolesAsync(_user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtConfig");
            var x = DateTime.Now.AddYears(Convert.ToInt32(jwtSettings["expiresIn"]));
            var tokenOptions = new JwtSecurityToken
            (
            issuer: jwtSettings["validIssuer"],
            audience: jwtSettings["validAudience"],
            claims: claims,
            expires: DateTime.Now.AddYears(Convert.ToInt32(jwtSettings["expiresIn"])),
            signingCredentials: signingCredentials
            );
            return tokenOptions;
        }
    }
    public class DetailedOperationResult
    {
        public bool EmailChangeSucceeded { get; set; }
        public bool PhoneChangeSucceeded { get; set; }
        public bool NameChangeSucceeded { get; set; }
        public bool PasswordChangeSucceeded { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public string Token { get; set; }

        public void AddErrors(IEnumerable<IdentityError> errors, string source)
        {
            Errors.AddRange(errors.Select(e => $"{source}: {e.Description}"));
        }
    }
}
