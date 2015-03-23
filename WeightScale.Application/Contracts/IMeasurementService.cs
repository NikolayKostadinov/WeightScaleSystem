//---------------------------------------------------------------------------------
// <copyright file="IMeasurementService.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.Application.Contracts
{
    using System;

    public interface IMeasurementService:IDisposable
    {
        void Measure(IWeightScaleMessageDto messageDto);

        bool IsWeightScaleOk(IWeightScaleMessageDto messageDto);
    }
}
