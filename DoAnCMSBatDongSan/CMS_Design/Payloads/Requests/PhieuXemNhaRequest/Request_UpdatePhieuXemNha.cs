using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.Requests.PhieuXemNhaRequest
{
    public class Request_UpdatePhieuXemNha
    {
        public int PhieuXemNhaId { get; set; }
        public string CustumerName { get; set; }
        public string CustumerPhoneNumber { get; set; }
        public string CustumerId { get; set; }
        public string Desciption { get; set; }
    }
}
