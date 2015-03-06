namespace WeightScale.Application.Services
{
    using System;
    using WeightScale.Domain.Abstract;

    interface IMeasurementService
    {
        IWeightScaleMessage Measure(IWeightScaleMessage message);
    }
}
