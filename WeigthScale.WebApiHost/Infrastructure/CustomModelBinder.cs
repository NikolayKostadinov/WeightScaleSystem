using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WeightScale.Application;
using WeightScale.Application.Contracts;
using WeightScale.Domain.Abstract;
using WeightScale.Domain.Common;
using WeightScale.Domain.Concrete;

namespace WeigthScale.WebApiHost.Infrastructure
{
    public class CustomModelBinder : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            string assemblyName = "WeightScale.Domain";

            if (bindingContext.ModelType == typeof(IWeightScaleMessageDto))
            {
                var value = actionContext.Request.Content.ReadAsStringAsync().Result;
                var model = actionContext.Request.GetDependencyScope().GetService(typeof(IWeightScaleMessageDto)) as IWeightScaleMessageDto;
                var message = JObject.Parse(value).Root["Message"].ToString();
                var messageType = actionContext.Request.Headers.GetValues("X-MessageType").FirstOrDefault();

                // Executes generic method without knowing of concrete type
                MethodInfo deserializeMethod = typeof(JsonConvert).GetMethod("DeserializeObject");
                MethodInfo genericMethod = deserializeMethod.MakeGenericMethod(Activator.CreateInstance(assemblyName, messageType).GetType());
                IWeightScaleMessage des = genericMethod.Invoke(null, new object[] { message }) as IWeightScaleMessage;
                //switch (messageType)
                //{
                //    case "WeightScaleMessageOld":
                //        model.Message = JsonConvert.DeserializeObject<WeightScaleMessageOld>(message);
                //        break;
                //    case "WeightScaleMessageNew":
                //        model.Message = JsonConvert.DeserializeObject<WeightScaleMessageNew>(message);
                //        break;
                //    default:
                //        break;
                //}
                var validationMessages = JObject.Parse(value).Root["ValidationMessages"].ToString();
                model.ValidationMessages = JsonConvert.DeserializeObject<ValidationMessageCollection>(validationMessages);
                bindingContext.Model = model;
                return true;
            }
            return false;
        }
    }
}
