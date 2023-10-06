using CMS_Design.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.DTOs.DataResponsePhieuXemNha
{
    public class PhieuXemNhaDTO
    {
        public int Id { get; set; }
        public string CustumerName { get; set; }
        public string CustumerPhoneNumber { get; set; }
        public string CustumerId { get; set; }
        public string CustumerIdImg1 { get; set; }
        public string CustumerIdImg2 { get; set; }
        public string Desciption { get; set; }
        public int NhaId { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
