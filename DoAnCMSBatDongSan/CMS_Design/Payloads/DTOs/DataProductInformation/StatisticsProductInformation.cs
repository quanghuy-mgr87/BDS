using CMS_Design.Entities;
using CMS_Design.Payloads.DTOs.DataResponsePhieuXemNha;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.DTOs.DataProductInformation
{
    public class StatisticsProductInformation
    {
        public int ProductId { get; set; }
        public double Price { get; set; }
        public string Address { get; set; }
        public double  Commission { get; set; }
        public IQueryable<PhieuXemNhaDTO> PhieuXemNhas { get; set; }
    }
}
