namespace WeightScale.Application.Contracts
{
    using System;
    using WeightScale.Domain.Abstract;

    public interface IMeasurementService
    {
        void Measure(IWeightScaleMessageDto messageDto);
    }
}
