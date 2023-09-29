using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Entities
{
    public class Notification : BaseEntity
    {
        public int CreateId { get; set; }
        public virtual User Create { get; set; }
        public DateTime CreateTime { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
        public int StatusId { get; set; }
        public virtual NotificationStatus Status { get; set; }
    }
}
