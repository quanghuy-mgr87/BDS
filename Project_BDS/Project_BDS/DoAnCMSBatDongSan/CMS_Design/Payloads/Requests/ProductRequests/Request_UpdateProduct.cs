using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.Requests.ProductRequests
{
    public class Request_UpdateProduct
    {
        public int Id { get; set; }
        public string HostName { get; set; }
        public string HostPhoneNumber { get; set; }
        public DateTime? Build { get; set; }
        public string CertificateOfLand1 { get; set; }
        public string CertificateOfLand2 { get; set; }
        public string Address { get; set; }
        public DateTime BatDauBan { get; set; }
        public double GiaBan { get; set; }
        public double HoaHong { get; set; }
        public double PhanTramChiaNV { get; set; }
    }
}
