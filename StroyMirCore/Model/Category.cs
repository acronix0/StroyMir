using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Core.Model
{
    public class Category : BaseEntity
    {
        public string Article { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public override bool Equals(object? obj)
        {
            return obj == null? false : CategoryEquals(obj as Category);
        }
        public bool CategoryEquals(Category other)
        {
            return Article == other.Article &&
             Image == other.Image &&
                Name == other.Name;
        }
    }
}
