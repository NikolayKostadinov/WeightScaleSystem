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
            if (bindingContext.ModelType == typeof(IWeightScaleMessageDto))
            {
                var value = actionContext.Request.Content.ReadAsStringAsync().Result;
                var model = actionContext.Request.GetDependencyScope().GetService(typeof(IWeightScaleMessageDto)) as IWeightScaleMessageDto;
                var message = JObject.Parse(value).Root["Message"].ToString();
                var messageType = actionContext.Request.Headers.GetValues("X-MessageType").FirstOrDefault();

                try
                {
                    Type t = GetType(messageType);
                    MethodInfo deserialize = typeof(CustomModelBinder).GetMethod("DeserializeObject", BindingFlags.NonPublic | BindingFlags.Instance);
                    MethodInfo genericMethod = deserialize.MakeGenericMethod(t);
                    IWeightScaleMessage deserializedMessage = genericMethod.Invoke(this, new object[] { message }) as IWeightScaleMessage;
                    model.Message = deserializedMessage;
                }
                catch (Exception ex)
                {
                    return false;
                }
                var validationMessages = JObject.Parse(value).Root["ValidationMessages"].ToString();
                model.ValidationMessages = JsonConvert.DeserializeObject<ValidationMessageCollection>(validationMessages);
                bindingContext.Model = model;
                return true;
            }
            return false;
        }

        private T DeserializeObject<T>(string message)
        {
            return JsonConvert.DeserializeObject<T>(message);
        }

        public static Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetTypes().FirstOrDefault(x => x.Name == typeName);
                if (type != null)
                    return type;
            }
            return null;
        }
    }

}
