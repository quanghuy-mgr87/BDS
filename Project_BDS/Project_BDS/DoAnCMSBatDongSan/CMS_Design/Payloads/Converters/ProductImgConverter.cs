using CMS_Design.Entities;
using CMS_Design.Payloads.DTOs.DataResponseProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.Converters
{
    public class ProductImgConverter
    {
        public ProductImgDTO EntityToDTO(ProductImg productImg)
        {
            return new ProductImgDTO
            {
                LinkImg = productImg.LinkImg
            };
        }
    }
}
