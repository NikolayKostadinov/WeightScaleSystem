namespace WeightScale.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using WeightScale.Application.Contracts;
    using WeightScale.CacheApi.SoapProxy;


    public class MappingService : WeightScale.Application.Contracts.IMappingService
    {
        public void ToProxy(IWeightScaleMessageDto source, SoapMessage proxy)
        {
            Mapper.Map(source.Message, proxy.Message, source.Message.GetType(), proxy.Message.GetType());
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
