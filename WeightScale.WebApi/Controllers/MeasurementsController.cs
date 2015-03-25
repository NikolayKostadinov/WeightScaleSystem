//namespace WeightScale.WebApi
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Net;
//    using System.Net.Http;
//    using System.Web.Http;
//    using System.Web.Http.ModelBinding;
//    using Ninject;
//    using WeightScale.Application;
//    using WeightScale.Application.AppStart;
//    using WeightScale.Application.Contracts;
//    using WeightScale.Application.Services;
//    using WeightScale.Domain.Common;
//    using WeightScale.Domain.Abstract;
//    using WeightScale.Domain.Concrete;
//    using WeightScale.WebApi;
//    using WeightScale.WebApi.Infrastructure;

//    public class Person 
//    {
//        public int Id { get; set; }
//        public string FirstName { get; set; }
//        public string LastName { get; set; }
//    }
//}

namespace WeightScale.WebApi.Controllers
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
    using WeightScale.WebApi.Infrastructure;

    public class MeasurementsController : ApiController
    {
        private readonly IMeasurementService mService;

        public MeasurementsController(IMeasurementService mServiceParam)
        {
            if (mServiceParam == null)
            {
                throw new ArgumentException("Cannot initialize measurement service.");
            }
            
            this.mService = mServiceParam;
        }

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
            if (ModelState.IsValid && value != null)
            {
                try
                {
                    this.mService.Measure(value);
                }
                catch (Exception ex)
                {
                    value.ValidationMessages.AddError("PostMeasurement", ex.Message);
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new ArgumentException("value", "Invalid input weigh scale message"));
            }

            return Request.CreateResponse(HttpStatusCode.OK, value);
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

        //[HttpPost]
        //public HttpResponseMessage PostPerson(Person p) 
        //{
        //    HttpResponseMessage response;
        //    if (ModelState.IsValid)
        //    {
        //        response = Request.CreateResponse(HttpStatusCode.OK, p);
        //    }
        //    else 
        //    {
        //        response = Request.CreateErrorResponse(HttpStatusCode.NotModified, new ArgumentException("p", "Invalid person data!"));
        //    }
        //    return response;
        //}

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
