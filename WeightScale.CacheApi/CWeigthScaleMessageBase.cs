using System;
using System.Linq;
using System.Text;
using AutoMapper;
using WeightScale.Domain.Abstract;
using WeightScale.Domain.Concrete;

namespace WeightScale.CacheApi.SoapProxy
{
    public partial class SoapMessage 
    {

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            var properties = this.GetType().GetProperties();

            foreach (var prop in properties)
            {
                sb.AppendLine(string.Format("{0}: {1}", prop.Name, prop.GetValue(this)!=null?prop.GetValue(this).ToString():string.Empty));
            }

            return sb.ToString();
        }
    }
}

namespace WeightScale.CacheApi.SoapProxy
{
    public partial class CWeigthScaleMessageBase
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            var properties = this.GetType().GetProperties();
            sb.AppendLine();
            foreach (var prop in properties)
            {
                sb.AppendLine(string.Format("-----{0}: {1}", prop.Name, prop.GetValue(this)));
            }

            return sb.ToString();
        }

        public abstract WeightScaleMessageBase ToDomainType();
    }

    public partial class CWeigthScaleMessageNew 
    {
        /// <summary>
        /// Toes the type of the domain.
        /// </summary>
        /// <returns></returns>
        public override WeightScaleMessageBase ToDomainType()
        {
            var domain = new WeightScaleMessageNew();
            Mapper.Map(this, domain);
            return domain;
        }
    }

    public partial class CWeigthScaleMessageOld 
    {
        /// <summary>
        /// Toes the type of the domain.
        /// </summary>
        /// <returns></returns>
        public override WeightScaleMessageBase ToDomainType()
        {
            var domain = new WeightScaleMessageOld();
            Mapper.Map(this, domain);
            return domain;
        }
    }
}
