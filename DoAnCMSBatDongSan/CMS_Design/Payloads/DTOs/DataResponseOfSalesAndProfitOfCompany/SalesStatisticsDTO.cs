﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.DTOs.DataResponseOfSalesAndProfitOfCompany
{
    public class SalesStatisticsDTO
    {
        public int MonthNumber { get; set; }
        public double Sales { get; set; }
        public double Profit { get; set; }
    }
}
