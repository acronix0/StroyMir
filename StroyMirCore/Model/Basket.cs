﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Core.Model
{
    public class Basket : BaseEntity
    {
        public List<BasketProduct> BasketProducts { get; set; } = new List<BasketProduct>();
        public ApplicationUser User { get; set; }
        public string DeliveryType { get; set; }
        public string RecipientName { get; set; }
        public string RecipientPhone { get; set; }
        public string RecipientEmail { get; set; }
        public string Address { get; set; }
        public string Comment { get; set; }
    }
}
