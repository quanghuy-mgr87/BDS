using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Entities
{
    public class TeamStatus: BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public virtual IEnumerable<Team> Teams { get; set; }

    }
}
