using Microsoft.AspNetCore.Identity;
using SimpleShop.Core.Dtos;
using SimpleShop.Core.Model;
using SimpleShop.Service.Services;
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
        Task<DetailedOperationResult> ChangeInfo(ApplicationUser user, UserRegistrationDto userRegistrationDto);
        Task<string> CreateTokenAsync();
    }
}
