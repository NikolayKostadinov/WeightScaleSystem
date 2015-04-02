using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using WeightScale.Application.Contracts;
using WeightScale.CacheApi.SoapProxy;
using WeightScale.Domain.Abstract;
using WeightScale.Domain.Concrete;

namespace WeightScale.Application.Services
{
    public class MappingService
    {
        public void ToProxy(IWeightScaleMessageDto source, SoapMessage proxy)
        {
            //var proxyMessage = //Activator.CreateInstance(proxy.Message.GetType());// as CWeigthScaleMessageNew;
            //var sourceMessage = //Activator.CreateInstance(source.Message.GetType());// as WeightScaleMessageNew;
            //sourceMessage = source.Message;
            Mapper.Map(source.Message, proxy.Message, source.Message.GetType(), proxy.Message.GetType());
            //var proxyValidationMessages = new List<CValidationMessage>();
            if (source.ValidationMessages.Count() > 0)
            {
                IList<CValidationMessage> proxyValidationMessages = new List<CValidationMessage>();
                foreach (var validationMessage in source.ValidationMessages)
                {
                    var proxyValidationMessage = new CValidationMessage();
                    Mapper.Map(validationMessage, proxyValidationMessage);
                    proxyValidationMessages.Add(proxyValidationMessage);
                }

                proxy.ValidationMessages = proxyValidationMessages.ToArray();
            }
        }
    }
}
