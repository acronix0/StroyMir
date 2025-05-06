using SimpleShop.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Core.Dtos
{
    public class BasketProductDto
    {
        public int Id { get; set; }
        public string ProductArticle { get; set; }
        public string Image {  get; set; }
        public string Name {  get; set; }
        public decimal Price {  get; set; }
        public int Count { get; set; }
        public int InStock { get; set; }
    }
}
