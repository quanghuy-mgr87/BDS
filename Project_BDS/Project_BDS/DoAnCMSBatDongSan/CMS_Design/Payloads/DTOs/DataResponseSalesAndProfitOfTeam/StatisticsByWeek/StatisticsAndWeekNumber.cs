using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.DTOs.DataResponseSalesAndProfitOfTeam.StatisticsByWeek
{
    public class StatisticsAndWeekNumber
    {
        public int WeekNumber { get; set; }
        public double Sales { get; set; }
        public double Profit { get; set; }
    }
}
