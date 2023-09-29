using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.Requests.AuthRequests
{
    public class Request_ConfirmCreateNewPassword
    {
        public string ConfirmCode { get; set; }
        public string NewPassword { get; set; }
    }
}
