namespace WeightScale.Application.Contracts
{
    using System;

    public interface IMappingService
    {
        void ToProxy(WeightScale.Application.Contracts.IWeightScaleMessageDto source, WeightScale.CacheApi.SoapProxy.SoapMessage proxy);
    }
}
