using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Core.Dtos
{
    public class AddProductDto
    {
        public string ProductArticle { get; set; }
        public int Count { get; set; }
    }
}
