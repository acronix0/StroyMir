using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Core.Model
{
    public class Product : BaseEntity
    {
        public string Article { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public Category Category { get; set; }
        public decimal Price { get; set; }
        public int Count {  get; set; }
        public List<BasketProduct> basketProducts { get; set; } = new List<BasketProduct>();
        public List<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

        public override bool Equals(object? obj)
        {
            return obj != null && ProductEquals(obj as Product);
        }
        private bool ProductEquals(Product other)
        {
           return Article == other.Article ||
                  Name == other.Name ||
                  Image == other.Image ||
                  Category.Equals(other.Category) ||
                  Price == other.Price ||
                  Count == other.Count;
        }
    }
}
