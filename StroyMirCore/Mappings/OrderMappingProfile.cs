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
    public class OrderMappingProfile: Profile
    {
        public OrderMappingProfile() 
        {
            CreateMap<Order,OrderDto>().ForMember(dist=>dist.OrderProducts, opt => opt.MapFrom(src=>src.OrderProducts));
        }
    }
}
