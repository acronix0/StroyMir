using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Core.Model
{
    public class Feedback : BaseEntity
    {
        public Product Product { get; set; }
        public ApplicationUser User{ get; set; }
        public int Rating{ get; set; }
        public string Comment{ get; set; }
        public DateTime CreatedAt{ get; set; }
    }
}
