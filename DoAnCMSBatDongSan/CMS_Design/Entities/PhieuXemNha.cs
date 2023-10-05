using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Entities
{
    public class PhieuXemNha : BaseEntity
    {
        public string CustumerName { get; set; }
        public string CustumerPhoneNumber { get; set; }
        public string CustumerId { get; set; }
        public string CustumerIdImg1 { get; set; }
        public string CustumerIdImg2 { get; set; }
        public string Desciption { get; set; }
        public int NhaId { get; set; }
        public Product Nha { get; set; }
        public int NhanVienId { get; set; }
        public DateTime CreateTime { get; set; }
        public bool BanThanhCong { get; set; }
    }
}
