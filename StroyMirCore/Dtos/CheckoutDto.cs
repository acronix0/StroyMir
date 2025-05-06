using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Core.Dtos
{
    public class CheckoutDto
    {
        public int Id { get; set; }
        public List<BasketProductDto> BasketProducts { get; set; }
        public OrderInfo OrderInfo { get; set; }
        
    }
    public class OrderInfo
    {
        public string UserId { get; set; }
        public string DeliveryType { get; set; }
        public string RecipientName { get; set; }
        public string RecipientPhone { get; set; }
        public string RecipientEmail { get; set; }
        public string Address { get; set; }
        public string Comment { get; set; }
    }
}
