namespace WeightScale.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using WeightScale.Application.Contracts;
 
    public class JsonDeserializeService : WeightScale.Application.Contracts.IJsonDeserializeService
    {

        public object GetResultFromJson(string jsonAnswer, object inputObject)
        {
            object message = Activator.CreateInstance(inputObject.GetType());
            CopyPropertiesFromInputObject(inputObject, message);
            var properties = message.GetType().GetProperties();

            foreach (var property in properties)
            {
                Type concreteType = property.GetValue(message).GetType();
                var currentPropertyJson = JObject.Parse(jsonAnswer).Root[property.Name].ToString();

                object deserializedProperty = DeserializeProperty(concreteType, currentPropertyJson);

                property.SetValue(message, deserializedProperty, null);

            }

            return message;
        }
 
        private void CopyPropertiesFromInputObject(object inputObject, object message)
        {
            var properties = inputObject.GetType().GetProperties();
            foreach (var property in properties)
            {
                object propValue = property.GetValue(inputObject);
                property.SetValue(message, propValue,null);
            }
        }
 
        private object DeserializeProperty(Type resultType, string currentPropertyJson)
        {
            MethodInfo deserialize = typeof(JsonDeserializeService).GetMethod("DeserializeObject", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo genericMethod = deserialize.MakeGenericMethod(resultType);
            return genericMethod.Invoke(this, new object[] { currentPropertyJson });
        }

        private T DeserializeObject<T>(string message)
        {
            return JsonConvert.DeserializeObject<T>(message);
        }
    }
}
