namespace WeightScale.Application.Contracts
{
    using System;

    public interface IMeasurementService
    {
        void Measure(IWeightScaleMessageDto messageDto);
        bool IsWeightScaleOk(IWeightScaleMessageDto messageDto);
    }
}
