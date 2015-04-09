using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeightScale.CacheApi.Contract;
using WeightScale.CacheApi.SoapProxy;
using WeightScale.CacheApi.Utility;

namespace WeightScale.CacheApi.Concrete
{
    public class MeasurementRequestsRepository:IRepository<SoapMessage,CValidationMessage>
    {
        private readonly WeightScaleServiceSoapClient client;
        public MeasurementRequestsRepository() 
        {
            this.client = new WeightScaleServiceSoapClient();
            this.client.Endpoint.EndpointBehaviors.Add(new MessageBehavior("_system", "ADMCACHE"));
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SoapMessage> GetAll()
        {
            return this.client.GetAllMeasurementRequests();
        }

        /// <summary>
        /// Gets the target urls.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetTargetUrls()
        {
            return this.client.GetAllHostAndPort();
        }

        /// <summary>
        /// Updates the specified message.
        /// </summary>
        /// <typeparam name="T">The type of the T.</typeparam>
        /// <param name="message">The message.</param>
        public IEnumerable<CValidationMessage> Update(SoapMessage message)
        {
            return this.client.InsertMeasurementResult(message);
        }
    }
}
