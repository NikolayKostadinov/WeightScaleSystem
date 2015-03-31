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
            Mapper.Map(source.Message, proxy.Message);
            Mapper.Map(source.ValidationMessages, proxy.ValidationMessages);
        }
    }
}
