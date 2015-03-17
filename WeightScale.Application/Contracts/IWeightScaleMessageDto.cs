//---------------------------------------------------------------------------------
// <copyright file="IWeightScaleMessageDto.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.Application.Contracts
{
    using System;
    using System.Linq;
    using WeightScale.Domain.Abstract;

    public interface IWeightScaleMessageDto
    {
        IWeightScaleMessage Message { get; set; }

        IValidationMessageCollection ValidationMessages { get; set; }
    }
}
