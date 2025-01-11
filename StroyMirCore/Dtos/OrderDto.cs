using SimpleShop.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Core.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        public List<OrderProductDto> OrderProducts { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public string DeliveryType { get; set; }
        public string RecipientName { get; set; }
        public string RecipientPhone { get; set; }
        public string RecipientEmail { get; set; }
        public string Address { get; set; }
        public string Comment { get; set; }
    }
}
