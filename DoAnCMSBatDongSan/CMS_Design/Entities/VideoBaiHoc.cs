using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Entities
{
    public class VideoBaiHoc : BaseEntity
    {
        public int CreateId  { get; set; }
        public virtual User Create { get; set; }
        public string URLPath { get; set; }
        public DateTime CraeteTime { get; set; }
        public string Name { get; set; }
        public int StatusId { get; set; }
        public virtual VideoBaiHocStatus Status { get; set; }
    }
}
