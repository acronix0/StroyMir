using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Core.Model
{
    public class Basket : BaseEntity
    {
        public List<BasketProduct> BasketProducts { get; set; } = new List<BasketProduct>();
        public ApplicationUser User { get; set; }
    }
}
