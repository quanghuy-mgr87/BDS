using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.DTOs.DataResponseSalesAndProfitOfTeam.StatisticsByMonth
{
    public class SalesAndProfitOfTeamByMonth
    {
        public int TeamId { get; set; }
        public StatisticsAndMonthNumber StatisticsAndMonthNumber { get; set; }
    }
}
