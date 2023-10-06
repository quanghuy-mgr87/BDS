﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Entities
{
    public class ProductImg : BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string LinkImg { get; set; }
    }
}
