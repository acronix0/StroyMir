using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StroyMir.Core.Model
{
    public class Promotion : BaseEntity
    {
        public string Code { get; set; }
        public string Description{ get; set; }
        public int Discount{ get; set; }
        public DateTime ValidFrom{ get; set; }
        public DateTime ValidTo{ get; set; }
    }
}
