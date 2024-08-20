using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Core.Dtos
{
    public class UserRegistrationDto
    {
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; init; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; init; }

        [Required(ErrorMessage = "Phone is required")]
        public string? Phone { get; init; }

        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; init; }


        public string Role { get; init; }
    }
}
