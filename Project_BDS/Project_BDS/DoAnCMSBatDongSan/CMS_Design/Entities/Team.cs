using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Entities
{
    public class Team : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int Member { get; set; }
        public string Description { get; set; }
        public string Sologan { get; set; }
        public int StatusId { get; set; } = 1;
        public TeamStatus Status { get; set; }
        public DateTime? CreateTime { get; set; } = DateTime.Now;
        public DateTime? UpdateTime { get; set; } = DateTime.Now;
        public int? TruongPhongId { get; set; }
        public virtual IEnumerable<User> Users { get; set; }
    }
}
