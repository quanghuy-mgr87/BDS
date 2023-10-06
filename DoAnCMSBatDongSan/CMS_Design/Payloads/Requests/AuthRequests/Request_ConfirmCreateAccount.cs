using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.Requests.AuthRequests
{
    public class Request_ConfirmCreateAccount
    {
        public string ConfirmCode { get; set; }
    }
}
