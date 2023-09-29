using CMS_Design.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.Requests.TeamRequests
{
    public class Request_CreateTeam
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int Member { get; set; }
        public string Description { get; set; }
        public string Sologan { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public int TruongPhongId { get; set; }
    }
}
