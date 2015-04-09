using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WeigthScale.WebApiHost.Infrastructure
{
    class FilesModelBinder : IModelBinder
    {
        /// <summary>
        /// Binds the model to a value by using the specified controller context
        /// and binding context.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <param name="bindingContext">The binding context.</param>
        /// <returns>true if model binding is successful; otherwise, false.</returns>
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType == typeof(IEnumerable<string>))
            {
                var value = actionContext.Request.Content.ReadAsStringAsync().Result;
                var strFiles = JObject.Parse(value).Root["files"].ToString();

                if (string.IsNullOrEmpty(strFiles))
                {
                    return false;
                }

                try
                {
                    bindingContext.Model = JsonConvert.DeserializeObject<IEnumerable<string>>(strFiles);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
        }
    }
}
