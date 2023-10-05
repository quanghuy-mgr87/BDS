using CMS_Design.Entities;
using CMS_Design.Payloads.DTOs.DataResponseProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.Converters
{
    public class ProductConverter
    {
        public ProductDTO EntityToDTO(Product product)
        {
            return new ProductDTO
            {
                Id = product.Id,
                HostName = product.HostName,
                BatDauBan = DateTime.Now,
                Build = product.Build,
                CertificateOfLand1 = product.CertificateOfLand1,
                CertificateOfLand2 = product.CertificateOfLand2,
                PhanTramChiaNV = product.PhanTramChiaNV,
                DauChuId = product.DauChuId,
                GiaBan = product.GiaBan,
                Address = product.Address,
                HostPhoneNumber = product.HostPhoneNumber,
                StatusId = product.StatusId
            };
        }
    }
}
