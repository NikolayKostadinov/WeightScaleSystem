using System.ComponentModel.DataAnnotations;

namespace WeightScale.WorkstationsChecker.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class PingReplyView
    {
        public string Status{get; set;}
        public string Address { get; set; }
		public long RoundtripTime{get;set;}
		public string Ttl{get;set;}
        public bool DontFragment { get; set; }
        public string Buffer { get; set; }
    }
}