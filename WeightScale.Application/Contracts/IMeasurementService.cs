//---------------------------------------------------------------------------------
// <copyright file="IMeasurementService.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.Application.Contracts
{
    using System;

    /// <summary>
    /// Measurement service
    /// </summary>
    public interface IMeasurementService : IDisposable
    {
        /// <summary>
        /// Provides measurement for the specified message.
        /// </summary>
        /// <param name="messageDto">The message data transfer object (DTO).</param>
        void Measure(IWeightScaleMessageDto messageDto);

        /// <summary>
        /// Determines whether given weight scale is OK.
        /// </summary>
        /// <param name="messageDto">The message data transfer object.</param>
        /// <returns> True or false </returns>
        /// <exception cref="System.InvalidOperationException">Cannot get exclusive access to measurement service. </exception>
        bool IsWeightScaleOk(IWeightScaleMessageDto messageDto);
    }
}
