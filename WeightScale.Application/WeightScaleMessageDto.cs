//---------------------------------------------------------------------------------
// <copyright file="WeightScaleMessageDto.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.Application
{
    using System;
    using System.Linq;
    using WeightScale.Application.Contracts;
    using WeightScale.Domain.Abstract;

    public class WeightScaleMessageDto : IWeightScaleMessageDto
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public IWeightScaleMessage Message { get; set; }


        /// <summary>
        /// Gets or sets the validation messages.
        /// </summary>
        /// <value>The validation messages.</value>
        public IValidationMessageCollection ValidationMessages { get; set; }
    }
}
