using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Core.Dtos
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Article { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
    }
}
