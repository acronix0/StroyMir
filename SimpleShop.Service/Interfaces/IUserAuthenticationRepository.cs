using Microsoft.AspNetCore.Identity;
using SimpleShop.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Service.Interfaces
{
    public interface IUserAuthenticationRepository
    {
        Task<IdentityResult> AddUserAsync(UserRegistrationDto userForRegistration);
        Task AddRoleIfNotExistAsync(string role);
        Task AddUserRoleAsync(UserRegistrationDto user, string role);
        Task<bool> ValidateUserAsync(UserLoginDto loginDto);
        Task<string> CreateTokenAsync();
    }
}
