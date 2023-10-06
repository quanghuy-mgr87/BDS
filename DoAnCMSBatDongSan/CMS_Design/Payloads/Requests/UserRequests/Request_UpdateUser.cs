using CMS_Design.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.Requests.UserRequests
{
    public class Request_UpdateUser
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int TeamId { get; set; }
    }
}
