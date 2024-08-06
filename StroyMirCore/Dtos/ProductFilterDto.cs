using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Core.Dtos
{
    public class ProductFilterDto
    {
        public ProductsPropertySearch Property {  get; set; }
        public string SearchText { get; set; }
    }
    public enum ProductsPropertySearch
    {
        Name,
        Category
    }
}
