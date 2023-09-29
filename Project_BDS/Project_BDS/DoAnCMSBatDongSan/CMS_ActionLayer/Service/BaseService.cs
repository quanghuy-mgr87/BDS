using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_ActionLayer.Service
{
    public class BaseService
    {
        public readonly DAO.CDSContext _context;
        public BaseService()
        {
            _context = new DAO.CDSContext();
        }
    }
}
