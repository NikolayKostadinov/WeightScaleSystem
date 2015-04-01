using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using WeightScale.Application.Contracts;
using WeightScale.CacheApi.SoapProxy;
using WeightScale.Domain.Abstract;

namespace WeightScale.Application.Services
{
    public class MappingService
    {
        public void ToProxy(IWeightScaleMessageDto source, SoapMessage proxy) 
        {
            CWeigthScaleMessageBase proxyMessage = Activator.CreateInstance(proxy.Message.GetType()) as CWeigthScaleMessageBase;
            Mapper.Map(source.Message, proxyMessage);
            proxy.Message = proxyMessage as CWeigthScaleMessageBase;
            var proxyValidationMessages = new List<CValidationMessage>();
            Mapper.Map(source.ValidationMessages, proxy.ValidationMessages);
            proxy.ValidationMessages = proxyValidationMessages.ToArray();
        }
    }
}
