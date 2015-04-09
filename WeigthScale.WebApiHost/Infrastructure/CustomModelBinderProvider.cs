namespace WeigthScale.WebApiHost.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.ModelBinding;
    using WeightScale.Application;
    using WeightScale.Application.Contracts;
    public class CustomModelBinderProvider : ModelBinderProvider
    {
        public override IModelBinder GetBinder(HttpConfiguration configuration, Type modelType)
        {
            return new WeigthScale.WebApiHost.Infrastructure.CustomModelBinder();
        }
    }
}
