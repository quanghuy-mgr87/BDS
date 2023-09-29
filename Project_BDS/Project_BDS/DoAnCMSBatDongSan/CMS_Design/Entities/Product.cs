using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Entities
{
    public class Product : BaseEntity
    {
        public string HostName { get; set; }
        public string HostPhoneNumber { get; set; }
        public DateTime? Build { get; set; }
        public string CertificateOfLand1 { get; set; }
        public string CertificateOfLand2 { get; set; }
        public int StatusId { get; set; } = 2;
        public ProductStatus Status { get; set; }
        public int DauChuId { get;set; }
        public User DauChu { get; set; }
        public DateTime BatDauBan { get; set; }
        public double GiaBan { get; set; }
        public double HoaHong { get; set; }
        public string Address { get; set; }
        public double PhanTramChiaNV { get; set; }
        public bool? IsActive { get; set; } = true;
        public virtual IEnumerable<PhieuXemNha> PhieuXemNhas { get; set; }
        public virtual IEnumerable<ProductImg> ProductImgs { get; set; }
    }
}
