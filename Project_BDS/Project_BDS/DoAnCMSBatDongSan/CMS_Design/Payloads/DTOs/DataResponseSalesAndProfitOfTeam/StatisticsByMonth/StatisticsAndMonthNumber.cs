using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.DTOs.DataResponseSalesAndProfitOfTeam.StatisticsByMonth
{
    public class StatisticsAndMonthNumber
    {
        public int MonthNumber { get; set; }
        public double Sales { get; set; }
        public double Profit { get; set; }
    }
}
