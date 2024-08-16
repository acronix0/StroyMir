using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Core.Model
{
   
    public enum DeliveryType
    {
        Pickup,     
        Delivery   
    }
    public class Order : BaseEntity
    {
        public ApplicationUser User { get; set; }
        public List<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
        public decimal TotalPrice{ get; set; }
        public DateTime OrderDate{ get; set; }
        public string DeliveryType { get; set; }
        public string RecipientName { get; set; }
        public string RecipientPhone { get; set; }
        public string RecipientEmail { get; set; }
        public string Address { get; set; }
        public string Comment { get; set; }
    }
}
