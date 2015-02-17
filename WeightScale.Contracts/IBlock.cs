//---------------------------------------------------------------------------------
// <copyright file="IBlock.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.Contracts
{
    /// <summary>
    /// Public interface. Abstracts classes witch can be processed as input for the CommandFactory
    /// </summary>
    public interface IBlock
    {
        /// <summary>
        /// Gets or sets Id of target Weight scale
        /// </summary>
        byte WeightScaleId { get; set; }

        /// <summary>
        /// Provides data block to be sent as Weight scale message block element.
        /// </summary>
        /// <returns>byte[] - Array of bytes</returns>
        byte[] ToBlock();
    }
}