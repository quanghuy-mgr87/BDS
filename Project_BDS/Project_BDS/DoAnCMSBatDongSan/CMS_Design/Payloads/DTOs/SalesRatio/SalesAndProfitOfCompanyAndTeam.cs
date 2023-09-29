using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.DTOs.SalesRatio
{
    public class SalesAndProfitOfCompanyAndTeam
    {
        public double SalesOfCompany { get; set; }
        public double ProfitOfCompany { get; set; }
        public StatisticsOfTeam StatisticsOfTeam { get; set; }
    }
}
