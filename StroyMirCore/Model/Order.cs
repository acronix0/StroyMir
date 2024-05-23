using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Core.Model
{
    public enum OrderState
    {
        Unpaid,
        InDelivered,
        Delivered,
        Сancelled

    }
    public class Order : BaseEntity
    {
        public ApplicationUser User{ get; set; }
        public decimal TotalPrice{ get; set; }
        public DateTime OrderDate{ get; set; }
        public OrderState OrderState { get; set; }
    }
}
