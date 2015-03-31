using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
