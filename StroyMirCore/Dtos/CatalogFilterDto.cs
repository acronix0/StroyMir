using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Core.Dtos
{
    public enum SortedType
    {
        name,
        priceUp,
        priceDown,
        count,

    }
    public class CatalogFilterDto
    {
        public int CategoryId { get; set; }
        public string SearchText { get; set; }
        public SortedType SortedType { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public bool InStock { get; set; }
    }
}
