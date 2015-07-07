/// <summary>
/// Summary description for Class1
/// </summary>
namespace WeightScale.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.ServiceModel.Channels;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using log4net;

    public class LogRequestAndResponseHandler : DelegatingHandler
    {
        private readonly ILog logger;
        public LogRequestAndResponseHandler(ILog loggerParam)
        {
            this.logger = loggerParam;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //logging request body
            string requestMessage = GetDataFromRequest(request);
            logger.Info(String.Format("REQUEST BODY: {0}", requestMessage));

            //let other handlers process the request
            return await base.SendAsync(request, cancellationToken)
                .ContinueWith(task =>
                    {
                        string responseMessage = GetDataFromResponse(task.Result);
                        //once response is ready, log it

                        logger.Info(String.Format("RESPONSE BODY: {0}", responseMessage));

                        return task.Result;
                    });
        }

        /// <summary>
        /// Gets the data from response.
        /// </summary>
        /// <typeparam name="TResult">The type of the T result.</typeparam>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        private string GetDataFromResponse(HttpResponseMessage response)
        {
            string responseBody = "No details for content";
            if (response.Content != null)
            {
                if (response.Content.Headers.ContentType.MediaType != "application/octet-stream")
                {
                    responseBody = response.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    responseBody = @"""Binary stream""";
                }
            }
            
            return string.Format("Response: {0}; DetailContent:{1}", 
                response.ToString().Replace("\n", "\t").Replace("\r", string.Empty), 
                responseBody.Replace("\n", "\t").Replace("\r", string.Empty)
                );
        }

        /// <summary>
        /// Gets the data from request.
        /// </summary>
        /// <param name="await">The await.</param>
        /// <returns></returns>
        private string GetDataFromRequest(HttpRequestMessage request)
        {
            string result = string.Format("Request: {0}; UserHostAddress: {1}; DetailContent: {2}",
                request.ToString().Replace("\n", "\t").Replace("\r", string.Empty),
                GetClientIp(request) ?? "No client IP",
                request.Content != null ? request.Content.ReadAsStringAsync().Result.Replace("\n", "\t").Replace("\r", string.Empty) : "No details for content"
                );
            //string content = await request.Content.ReadAsStringAsync();
            return result;
        }

        private string GetClientIp(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                RemoteEndpointMessageProperty prop;
                prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }
            else
            {
                return null;
            }
        }
    }
}
