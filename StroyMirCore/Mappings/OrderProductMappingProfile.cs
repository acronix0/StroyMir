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
    public class OrderProductMappingProfile: Profile
    {
        public OrderProductMappingProfile() 
        {
            CreateMap<OrderProduct, OrderProductDto>().ForMember(dist=>dist.Image, opt=>opt.MapFrom(src=>src.Product.Image)).ReverseMap();
        }
    }
}
