using CMS_Design.Entities;
using CMS_Design.Payloads.DTOs.DataResponsePhieuXemNha;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.Converters
{
    public class PhieuXemNhaConverter
    {
        public PhieuXemNhaDTO EntityToDTO(PhieuXemNha phieuXemNha)
        {
            return new PhieuXemNhaDTO
            {
                CreateTime = phieuXemNha.CreateTime,
                NhaId = phieuXemNha.NhaId,
                CustumerName = phieuXemNha.CustumerName,
                CustumerPhoneNumber = phieuXemNha.CustumerPhoneNumber,
                Desciption = phieuXemNha.Desciption,
                CustumerId = phieuXemNha.CustumerId,
                CustumerIdImg1 = phieuXemNha.CustumerIdImg1,
                CustumerIdImg2 = phieuXemNha.CustumerIdImg2
            };
        }
    }
}
