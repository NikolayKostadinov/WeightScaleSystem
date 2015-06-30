using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeightScale.WorkstationsChecker.Model;
using WeightScale.WorkstationsChecker.Web.Infrastructure.Mappings;

namespace WeightScale.WorkstationsChecker.Web.Models
{
    public class WeightScaleWorkStationViewModel:IMapFrom<WeightScaleWorkStation>
    {
        public int Id { get; set; }

        public string Address { get; set; }

        public string Name { get; set; }
    }
}