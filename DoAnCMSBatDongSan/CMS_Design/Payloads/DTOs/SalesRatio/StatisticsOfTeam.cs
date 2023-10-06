using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.DTOs.SalesRatio
{
    public class StatisticsOfTeam
    {
        public int TeamId { get; set; }
        public int WeekNumber { get; set; }
        public double Sales { get; set; }
        public double Profit { get; set; }
        public double SalesRatio { get; set; }
        public double ProfitRatio { get; set; }
    }
}
