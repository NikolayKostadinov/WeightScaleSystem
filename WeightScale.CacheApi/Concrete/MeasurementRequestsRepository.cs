//  ------------------------------------------------------------------------------------------------
//   <copyright file="MeasurementRequestsRepository.cs" company="Business Management System Ltd.">
//       Copyright "2019" (c), Business Management System Ltd.
//       All rights reserved.
//   </copyright>
//   <author>Nikolay.Kostadinov</author>
//  ------------------------------------------------------------------------------------------------

namespace WeightScale.CacheApi.Concrete
{
    #region Using

    using System.Collections.Generic;
    using System.Threading.Tasks;

    using WeightScale.CacheApi.Contract;
    using WeightScale.CacheApi.SoapProxy;
    using WeightScale.CacheApi.Utility;

    #endregion

    public class MeasurementRequestsRepository : IRepository<SoapMessage, CValidationMessage>
    {
        private readonly WeightScaleServiceSoapClient client;

        public MeasurementRequestsRepository(IConnectionParameters context)
        {
            this.client = new WeightScaleServiceSoapClient();
            this.client.Endpoint.EndpointBehaviors.Add(new MessageBehavior(context.UserName, context.Password));
        }

        /// <summary>
        ///     Gets all.
        /// </summary>
        /// <returns>The Soap messages</returns>
        public IEnumerable<SoapMessage> GetAll()
        {
            return this.client.GetAllMeasurementRequests();
        }

        /// <summary>
        ///     Gets all measurement requests.
        /// </summary>
        /// <returns>The Soap messages</returns>
        public async Task<IEnumerable<SoapMessage>> GetAllAsync()
        {
            return await this.client.GetAllMeasurementRequestsAsync();
        }

        /// <summary>
        ///     Gets the target URLs.
        /// </summary>
        /// <returns>The target urls.</returns>
        public IEnumerable<string> GetTargetUrls()
        {
            return this.client.GetAllHostAndPort();
        }

        /// <summary>
        ///     Gets the target URLs asynchronous.
        /// </summary>
        /// <returns>The target urls.</returns>
        public async Task<IEnumerable<string>> GetTargetUrlsAsync()
        {
            return await Task.Factory.StartNew(this.GetTargetUrls);
        }

        /// <summary>
        ///     Updates the specified message.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable{CValidationMessage}" />.
        /// </returns>
        public IEnumerable<CValidationMessage> Update(SoapMessage message)
        {
            return this.client.InsertMeasurementResult(message);
        }

        /// <summary>
        ///     Updates the record asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Validation messages.</returns>
        public async Task<IEnumerable<CValidationMessage>> UpdateAsync(SoapMessage message)
        {
            return await this.client.InsertMeasurementResultAsync(message);
        }
    }
}