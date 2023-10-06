using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Design.Payloads.DTOs.DataResponseGeocoding
{
    public class GoogleMapsResponse
    {
        public List<Result> Results { get; set; }
    }
}
