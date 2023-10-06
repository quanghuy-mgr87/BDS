using CMS_Design.Payloads.DTOs.DataResponseProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.DTOs.DataResponseListProductSoldAndPrice
{
    public class ProductNotYetSoldAndPrice
    {
        public double Price { get; set; }
        public double Commission { get; set; }
        public IQueryable<ProductDTO> ProductDTOs { get; set; }
    }
}
