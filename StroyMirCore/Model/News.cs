using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Core.Model
{
    public class News : BaseEntity
    {
        public string Title{ get; set; }
        public string Content { get; set; }
        public DateTime PublishedDate { get; set; }
        public ApplicationUser Author{ get; set; }
        public bool IsActive{ get; set; }
    }
}
