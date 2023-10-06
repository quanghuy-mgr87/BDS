using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.Requests.ProductRequests
{
    public class Request_UpdateImageProduct
    {
        public int? ProductImgId { get; set; }
        public string Title { get; set; }
        public IFormFile? ImageProduct { get; set; }
        public int? ProductId { get; set; }
    }
}
