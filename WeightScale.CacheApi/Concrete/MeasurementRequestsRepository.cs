namespace WeightScale.CacheApi.Concrete
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using WeightScale.CacheApi.Contract;
    using WeightScale.CacheApi.SoapProxy;
    using WeightScale.CacheApi.Utility;

    public class MeasurementRequestsRepository : IRepository<SoapMessage, CValidationMessage>
    {
        private readonly WeightScaleServiceSoapClient client;
        public MeasurementRequestsRepository()
        {
            this.client = new WeightScaleServiceSoapClient();
            //this.client.Endpoint.EndpointBehaviors.Add(new MessageBehavior("_system", "ADMCACHE"));
            this.client.Endpoint.EndpointBehaviors.Add(new MessageBehavior("_system", "SYS"));
        }

        /// <summary>
        /// Gets all measurement requests.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<SoapMessage>> GetAllAsynk()
        {
            return await this.client.GetAllMeasurementRequestsAsync();
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
        /// Gets the target URLs asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetTargetUrlsAsync()
        {
            return await Task.Factory.StartNew(() => GetTargetUrls());
        }

        /// <summary>
        /// Gets the target URLs.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetTargetUrls()
        {
            return this.client.GetAllHostAndPort();//new List<string>{"http://10.94.23.80:8111/"};//
        }

        /// <summary>
        /// Updates the record asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public async Task<IEnumerable<CValidationMessage>> UpdateAsync(SoapMessage message)
        {
            return await this.client.InsertMeasurementResultAsync(message);
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
