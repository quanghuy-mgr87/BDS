using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.Requests.ProductRequests
{
    public class Request_CreateProductImg
    {
        public IFormFile LinkImg { get; set; }
    }
}
