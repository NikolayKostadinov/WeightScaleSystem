namespace WeightScale.WorkstationsChecker.Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;
    using WeightScale.WorkstationsChecker.Model;
    using WeightScale.WorkstationsChecker.Web.Infrastructure.Mappings;

    public class WorkstationViewModel : IMapFrom<WeightScaleWorkStation>
    {
        [Editable(false)]
        public int Id { get; set; }

        [Required]
        [RegularExpression(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$")]
        public string Address { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int ScreenPosition { get; set; }

        public bool IsStopped { get; set; }
    }
}