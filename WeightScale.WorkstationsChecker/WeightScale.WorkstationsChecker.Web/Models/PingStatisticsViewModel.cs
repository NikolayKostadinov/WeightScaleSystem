using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using WeightScale.WorkstationsChecker.Model;
using WeightScale.WorkstationsChecker.Web.Infrastructure.Mappings;

namespace WeightScale.WorkstationsChecker.Web.Models
{
    public class PingStatisticsViewModel:IMapFrom<PingPole>, IHaveCustomMappings
    {
        public DateTime TimeStamp { get; set; }

        public long RoundtripTime { get; set; }
        /// <summary>
        /// Creates the mappings.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<PingPole, PingStatisticsViewModel>()
                .ForMember(p => p.RoundtripTime, opt => opt.MapFrom(p => p.PingReply.RoundtripTime));
        }
    }
}