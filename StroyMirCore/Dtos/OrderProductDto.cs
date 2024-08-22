using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Core.Dtos
{
    public class OrderProductDto
    {
        public int ProductId { get; set; }
        public string ProductArticle { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string Image { get; set; }
        public int Count { get; set; }
    }
}
