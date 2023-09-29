using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Entities
{
    public class VideoBaiHocStatus : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public virtual IEnumerable<VideoBaiHoc> VideoBaiHocs { get; set; }

    }
}
