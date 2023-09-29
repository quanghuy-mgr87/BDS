using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.DTOs.DataResponseSalesAndProfitOfTeam.StatisticsByWeek
{
    public class SalesAndProfitOfTeamByWeek
    {
        public int TeamId { get; set; }
        public StatisticsAndWeekNumber SatisticsAndWeekNumber { get; set; }
    }
}
