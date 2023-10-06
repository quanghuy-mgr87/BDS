using CMS_Design.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.Requests.PhieuXemNhaRequest
{
    public class Request_CreatePhieuXemNha
    {
        public string CustumerName { get; set; }
        public string CustumerPhoneNumber { get; set; }
        public string CustumerId { get; set; }
        public int NhaId { get; set; }
        public int NhanVienId { get; set; }
        //public IFormFile CustumerIdImg1 { get; set; }
        //public IFormFile CustumerIdImg2 { get; set; }
        public string Desciption { get; set; }
    }
}
