using SimpleShop.Core.Dtos;
using SimpleShop.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
namespace SimpleShop.Core.Mappings
{
    public class UserMappingProfile : AutoMapper.Profile
    {
        public UserMappingProfile()
        {
            CreateMap<UserRegistrationDto, ApplicationUser>()
                .ForMember(dist=> dist.UserName, opt=>opt.MapFrom(src=>src.Name))
                .ForMember(dist=> dist.PhoneNumber, opt=>opt.MapFrom(src=>src.Phone));
        }
    }
}
