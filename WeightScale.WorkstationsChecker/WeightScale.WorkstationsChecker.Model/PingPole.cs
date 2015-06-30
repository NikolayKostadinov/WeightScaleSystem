/// <summary>
/// Summary description for Ping
/// </summary>
namespace WeightScale.WorkstationsChecker.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using WeightScale.WorkstationsChecker.Contracts;

    public class PingPole : DeletableEntity, IEntity
    {
        public PingPole()
        {
            this.TimeStamp = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }

        public DateTime TimeStamp { get; private set; }

        public PingReplyView PingReply { get; set; }

        public int WeightScaleWorkStationId { get; set; }

        public virtual WeightScaleWorkStation WeightScaleWorkStation { get; set; }
    }
}
