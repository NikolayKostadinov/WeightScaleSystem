using System;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WeightScale.Application;
using WeightScale.Application.Contracts;
using WeightScale.Domain.Common;
using WeightScale.Domain.Concrete;

namespace WeightScale.WebApi.Infrastructure
{
    public class CustomModelBinder:IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType == typeof(IWeightScaleMessageDto))
            {
                var value = actionContext.Request.Content.ReadAsStringAsync().Result;
                var model = new WeightScaleMessageDto();
                var message = JObject.Parse(value).Root["Message"].ToString();
                var messageType = actionContext.Request.Headers.GetValues("X-MessageType").FirstOrDefault();
                switch (messageType)
                {
                    case "WeightScaleMessageOld":
                        model.Message = JsonConvert.DeserializeObject<WeightScaleMessageOld>(message);
                        break;
                    case "WeightScaleMessageNew":
                        model.Message = JsonConvert.DeserializeObject<WeightScaleMessageNew>(message);
                        break;
                    default:
                        break;
                }
                var validationMessages = JObject.Parse(value).Root["ValidationMessages"].ToString();
                model.ValidationMessages = JsonConvert.DeserializeObject<ValidationMessageCollection>(validationMessages);
                bindingContext.Model = model;
                return true;
            }
            return false;
        }
    }
}
