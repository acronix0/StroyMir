﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShop.Core.Dtos
{
    public class ProductFilterDto
    {
       
        public string SearchText { get; set; }
        public int Take { get; set; }
    }
   
}
