using CMS_Design.Entities;
using CMS_Design.Payloads.DTOs.DataResponseUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.DTOs.DataResponseTeam
{
    public class TeamDTO
    {
        public string Name { get; set; }
        public int Member { get; set; }
        public string Description { get; set; }
        public string Sologan { get; set; }
        public int StatusId { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int TruongPhongId { get; set; }
        public IEnumerable<UserDTO> Users { get; set; }
    }
}
