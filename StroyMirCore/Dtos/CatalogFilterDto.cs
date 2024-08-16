using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Core.Dtos
{
    public class CatalogFilterDto
    {
        public int CategoryId { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}
