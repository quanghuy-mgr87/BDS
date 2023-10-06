using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Handler.HandlePagination
{
    public class Pagination
    {
        public static PageResult<T> GetPagedData<T>(IQueryable<T> data, int pageSize, int pageNumber)
        {
            int totalItems = data.Count();
            int totalPages = (int)Math.Ceiling((decimal)totalItems / pageSize);

            var pagedData = data.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return new PageResult<T>(pagedData, totalItems, totalPages, pageNumber, pageSize);
        }
    }
}
