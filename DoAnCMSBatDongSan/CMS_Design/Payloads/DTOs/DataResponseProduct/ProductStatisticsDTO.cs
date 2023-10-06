using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.DTOs.DataResponseProduct
{
    public class ProductStatisticsDTO
    {
        public int OwnerId { get; set; }
        public int NumberOfProduct { get; set; }
        public  IQueryable<ProductDTO> ProductDTOs { get; set; }

    }
}
