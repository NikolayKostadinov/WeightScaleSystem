using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WeightScale.Application.Contracts;
using WeightScale.Domain.Concrete;
using log4net;

namespace WeightScale.Application.Services
{
    public class MockedMeasurementService : IMeasurementService
    {
        private readonly ILog logger;
        public MockedMeasurementService(ILog loggerParam)
        {
            this.logger = loggerParam;
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing,
        /// or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            //do Nothing :)
        }

        /// <summary>
        /// Provides measurement for the specified message.
        /// </summary>
        /// <param name="messageDto">The message data transfer object (DTO).</param>
        public void Measure(IWeightScaleMessageDto messageDto)
        {
            var begin = DateTime.Now;

            this.logger.Debug(string.Format("------------------ Message Id: {0}", messageDto.Id));
            this.logger.Debug(string.Format("Message received from client: {0}", messageDto.Message.ToString()));
            var messageType = messageDto.Message.GetType();
            switch (messageType.Name)
            {
                case "WeightScaleMessageNew":

                    if (((WeightScaleMessageNew)messageDto.Message).MeasurementNumber == 1)
                    {
                        ((WeightScaleMessageNew)messageDto.Message).TimeOfFirstMeasure = DateTime.Now;
                        ((WeightScaleMessageNew)messageDto.Message).MeasurementStatus = 0;
                        ((WeightScaleMessageNew)messageDto.Message).TransactionNumber = 200;
                        ((WeightScaleMessageNew)messageDto.Message).TareWeight = 10000;
                        ((WeightScaleMessageNew)messageDto.Message).TotalNetOfOutput = 22000;
                        ((WeightScaleMessageNew)messageDto.Message).TotalNetByProductOutput = 12000;
                    }
                    else
                    {
                        ((WeightScaleMessageNew)messageDto.Message).TimeOfFirstMeasure = DateTime.Now;
                        ((WeightScaleMessageNew)messageDto.Message).TimeOfSecondMeasure = DateTime.Now.AddSeconds(1);
                        ((WeightScaleMessageNew)messageDto.Message).MeasurementStatus = 0;
                        ((WeightScaleMessageNew)messageDto.Message).TransactionNumber = 200;
                        ((WeightScaleMessageNew)messageDto.Message).TareWeight = 10000;
                        ((WeightScaleMessageNew)messageDto.Message).GrossWeight = 25000;
                        ((WeightScaleMessageNew)messageDto.Message).NetWeight = 15000;
                        ((WeightScaleMessageNew)messageDto.Message).TotalNetOfOutput = 37000;
                        ((WeightScaleMessageNew)messageDto.Message).TotalNetByProductOutput = 27000;

                    }
                    break;

                case "WeightScaleMessageOld":
                    if (((WeightScaleMessageOld)messageDto.Message).MeasurementNumber == 1)
                    {

                        ((WeightScaleMessageOld)messageDto.Message).TimeOfFirstMeasure = DateTime.Now;
                        ((WeightScaleMessageOld)messageDto.Message).MeasurementStatus = 0;
                        ((WeightScaleMessageOld)messageDto.Message).TransactionNumber = 501;
                        ((WeightScaleMessageOld)messageDto.Message).TareWeight = 5000;
                        ((WeightScaleMessageOld)messageDto.Message).TotalOfNetWeight = 13000;
                    }
                    else
                    {
                        ((WeightScaleMessageOld)messageDto.Message).TimeOfFirstMeasure = DateTime.Now;
                        ((WeightScaleMessageOld)messageDto.Message).TimeOfSecondMeasure = DateTime.Now.AddSeconds(1);
                        ((WeightScaleMessageOld)messageDto.Message).MeasurementStatus = 0;
                        ((WeightScaleMessageOld)messageDto.Message).TransactionNumber = 501;
                        ((WeightScaleMessageOld)messageDto.Message).TareWeight = 5000;
                        ((WeightScaleMessageOld)messageDto.Message).GrossWeight = 12000;
                        ((WeightScaleMessageOld)messageDto.Message).NetWeight = 7000;
                        ((WeightScaleMessageOld)messageDto.Message).TotalOfNetWeight = 20000;

                    }

                    break;

                default:
                    throw new NotImplementedException(
                        string.Format("There is no such protocol! {0}", messageType.Name));
            }

            this.logger.Debug(string.Format("Message Sent to client: {0}", messageDto.Message.ToString()));
            Thread.Sleep(300);
            this.logger.Debug(string.Format("Estimated time for the transaction: {0}", (DateTime.Now - begin).ToString(@"ss\:fff")));
        }

        /// <summary>
        /// Determines whether given weight scale is OK.
        /// </summary>
        /// <param name="messageDto">The message data transfer object.</param>
        /// <exception cref="System.InvalidOperationException">Cannot get exclusive access to measurement service. </exception>
        /// <returns>True or false</returns>
        public bool IsWeightScaleOk(IWeightScaleMessageDto messageDto)
        {
            return true;
        }
    }
}
