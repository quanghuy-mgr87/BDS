using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.DTOs.DataResponseSalesAndNumberOfPassengers
{
    public class SalesAndNumberOfPassengersByMonthDTO
    {
        public int TeamId { get; set; }
        public int MonthNumber { get; set; }
        public int EmployeeId { get; set; }
        public double Sales { get; set; }
        public int NumberOfPassengers { get; set; }
    }
}
