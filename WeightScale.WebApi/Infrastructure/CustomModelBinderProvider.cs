using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using WeightScale.Application;
using WeightScale.Application.Contracts;

namespace WeightScale.WebApi.Infrastructure
{
    public class CustomModelBinderProvider : ModelBinderProvider
    {
        public override IModelBinder GetBinder(HttpConfiguration configuration, Type modelType)
        {
            // if (modelType == typeof(WeightScaleMessageConcreteNewDto))
            // {
                return new CustomModelBinder();
            // }
            // else if (modelType == typeof(WeightScaleMessageConcreteOldDto)) 
            // {
            //    return new CustomModelBinderOld();
            // }

            // return null;
        }
    }
}
