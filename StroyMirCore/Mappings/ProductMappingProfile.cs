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
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductDto>()
                //dest = result object, opt = mapping obj, src = request obj 
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Category.Id.ToString()))
                .ForMember(dest=> dest.Id, opt=> opt.MapFrom(src => src.Article ));
        }
    }
}
