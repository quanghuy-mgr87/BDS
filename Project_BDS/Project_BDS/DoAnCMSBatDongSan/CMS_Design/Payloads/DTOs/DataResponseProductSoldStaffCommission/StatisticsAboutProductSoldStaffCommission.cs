using CMS_Design.Payloads.DTOs.DataResponseProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.DTOs.DataResponseProductSoldStaffCommission
{
    public class StatisticsAboutProductSoldStaffCommission
    {
        public int OwnerId { get; set; }
        public double Price { get; set; }
        public double Commission { get; set; }
        public IQueryable<ProductDTO> ProductDTOs { get; set; }
    }
}
