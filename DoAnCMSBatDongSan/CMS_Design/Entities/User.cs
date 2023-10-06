using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Entities
{
    public class User : BaseEntity
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }    
        public DateTime DateOfBirth { get; set; }
        public int? RoleId { get; set; }
        public UserRole? Role { get; set; }
        public int? StatusId { get; set; }
        public UserStatus? Status { get; set; }
        public int? TeamId { get; set; }
        public Team? Team { get; set; }
        public bool? IsActive { get; set; } = true;
        public virtual IEnumerable<Notification> Notifications { get; set; }
        public virtual IEnumerable<Product> Products { get; set; }
        public virtual IEnumerable<VideoBaiHoc> VideoBaiHocs { get; set; }
    }
}
