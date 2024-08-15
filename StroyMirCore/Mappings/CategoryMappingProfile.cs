using AutoMapper;
using SimpleShop.Core.Dtos;
using SimpleShop.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Core.Mappings
{
    public class CategoryMappingProfile: Profile
    {
        public CategoryMappingProfile() 
        { 
            CreateMap<Category,CategoryDto>().ReverseMap();
        }
    }
}
