/// <summary>
/// Summary description for WeightScale
/// </summary>
namespace WeightScale.WorkstationsChecker.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using WeightScale.WorkstationsChecker.Contracts;

    public class WeightScaleWorkStation: DeletableEntity, IEntity
    {
        private ICollection<PingPole> pings;

        public WeightScaleWorkStation() 
        {
            this.pings = new HashSet<PingPole>();
        }

        [Key]
        public int Id { get; set; }

        public string Address { get; set; }

        public string Name { get; set; }

        public int ScreenPosition { get; set; }

        [DefaultValue(false)]
        public bool IsStopped { get; set; }


        public virtual ICollection<PingPole> Pings
        {
            get { return this.pings; }
            set { this.pings = value; }
        }

    }
}
