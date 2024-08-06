using Microsoft.AspNetCore.Identity;
using SimpleShop.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Service.Interfaces
{
    public interface IUserRepository
    {
        Task<IdentityResult> AddUser(UserRegistrationDto userRegistrationDto);
        Task<bool> ValidateUserAsync(UserLoginDto loginDto);
    }
}
