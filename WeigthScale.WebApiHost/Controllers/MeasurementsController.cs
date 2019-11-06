namespace WeigthScale.WebApiHost.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.ModelBinding;
    using WeightScale.Application;
    using WeightScale.Application.Contracts;
    using WeightScale.Domain.Abstract;
    using WeightScale.Domain.Common;
    using WeightScale.Domain.Concrete;
    using WeigthScale.WebApiHost.Infrastructure;
    using log4net;
    using Newtonsoft.Json;

    public class MeasurementsController : ApiController
    {
        private readonly IMeasurementService mService;
        private readonly ILog logger;

        public MeasurementsController(IMeasurementService mServiceParam, ILog logerParam)
        {
            if (mServiceParam == null)
            {
                throw new ArgumentException("Cannot initialize measurement service.");
            }

            if (logerParam == null)
            {
                throw new ArgumentException("Cannot initialize logger service.");
            }
            
            this.mService = mServiceParam;
            this.logger = logerParam;
        }

        [Route("api/Measurements/GetTest")]
        public IWeightScaleMessageDto GetTest()
        {
            IWeightScaleMessage message = GenerateWeightBlock();
            IWeightScaleMessageDto value = new WeightScaleMessageDto() { Message = message, ValidationMessages = new ValidationMessageCollection() };
            return value;
        }

        public IWeightScaleMessageDto GetOld()
        {
            IWeightScaleMessage message = GenerateWeightBlockOld();
            IWeightScaleMessageDto value = new WeightScaleMessageDto() { Message = message, ValidationMessages = new ValidationMessageCollection() };
            return value;
        }

        // POST api/Measurements
        [HttpPost]
        public HttpResponseMessage PostMeasurement([ModelBinder(typeof(CustomModelBinder))]IWeightScaleMessageDto value)
        {
            var beginTime = DateTime.Now;
            this.logger.Debug(string.Format("------------- Processing request Id: {0} -------------", value.Id));
            if (ModelState.IsValid && value != null)
            {
                try
                {
                    this.mService.Measure(value);
                }
                catch (Exception ex)
                {
                    value.ValidationMessages.AddError("PostMeasurement", ex.Message);
                    logger.Error(ex.Message, ex);
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new ArgumentException("value", "Invalid input weigh scale message"));
            }

            this.logger.Debug(string.Format("[Id: \t{0}\t] Sent response message: {1}", value.Id, JsonConvert.SerializeObject(value)));
            this.logger.Debug(string.Format("[Id: \t{0}\t] Estimated time for transaction {1}", value.Id, DateTime.Now - beginTime));
            return Request.CreateResponse(HttpStatusCode.OK, value);
        }

        // POST api/Measurements
        [HttpPost]
        public HttpResponseMessage PostIsOk([ModelBinder(typeof(CustomModelBinder))]IWeightScaleMessageDto value)
        {
            // URL: http://10.94.21.10:8111/api/Measurements/PostIsOk
            // X-MessageType: WeightScaleMessageNew
            // Body: {"Id":0,"Message":{"ExciseDocumentNumber":"1400032512","TotalNetOfInput":null,"TotalNetOfOutput":null,"TotalNetByProductInput":null,"TotalNetByProductOutput":null,"Number":4,"Direction":1,"TimeOfFirstMeasure":null,"TimeOfSecondMeasure":null,"MeasurementStatus":0,"SerialNumber":12345678,"TransactionNumber":12345,"MeasurementNumber":1,"ProductCode":100,"TareWeight":null,"GrossWeight":null,"NetWeight":null,"Vehicle":"A3335KX","DocumentNumber":null},"ValidationMessages":[]}
            bool result = false;

            if (ModelState.IsValid && value != null)
            {
                try
                {
                    result = this.mService.IsWeightScaleOk(value);
                }
                catch (Exception ex)
                {
                    value.ValidationMessages.AddError("PostMeasurement", ex.Message);
                    logger.Error(ex.Message, ex);
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new ArgumentException("value", "Invalid input weigh scale message"));
            }

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        private static IWeightScaleMessage GenerateWeightBlock()
        {
            var ser = new WeightScaleMessageNew();
            //var ser = new WeightScaleMessageOld();
            ser.Number = 3;
            ser.Direction = Direction.In;
            ser.SerialNumber = 12345678;
            ser.TransactionNumber = 12345;
            ser.MeasurementNumber = 2;
            ser.ProductCode = 100;
            ser.ExciseDocumentNumber = "1400032512";
            ser.Vehicle = "A3335KX";
            return ser;
        }

        private IWeightScaleMessage GenerateWeightBlockOld()
        {
            //var ser = new WeightScaleMessageNew();
            var ser = new WeightScaleMessageOld();
            ser.Number = 3;
            ser.Direction = Direction.In;
            ser.SerialNumber = 12345678;
            ser.TransactionNumber = 12345;
            ser.MeasurementNumber = 2;
            ser.ProductCode = 100;
            //ser.ExciseDocumentNumber = "1400032512";
            ser.Vehicle = "A3335KX";
            return ser;
        }

        //// PUT api/Measurements/5
        //public void PutMeasurement(int id, [FromBody]
        //                           string value)
        //{
        //}

        //// DELETE api/Measurements/5
        //public void DeleteMeasurement(int id)
        //{
        //}
    }
}
