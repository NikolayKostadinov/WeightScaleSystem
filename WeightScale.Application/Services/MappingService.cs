using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using WeightScale.Application.Contracts;
using WeightScale.CacheApi.SoapProxy;
using WeightScale.Domain.Abstract;
using WeightScale.Domain.Common;
using WeightScale.Domain.Concrete;

namespace WeightScale.Application.Services
{
    public class MappingService
    {
        public void ToProxy(IWeightScaleMessageDto source, SoapMessage proxy) 
        {
            Mapper.Map(source.Message, proxy.Message, source.Message.GetType(), proxy.Message.GetType());
            var proxyValidationMessages = new List<CValidationMessage>();
            Mapper.Map(source.ValidationMessages, proxy.ValidationMessages);
            proxy.ValidationMessages = proxyValidationMessages.ToArray();
        }
    }
}
