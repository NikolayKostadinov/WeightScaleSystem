//---------------------------------------------------------------------------------
// <copyright file="IChecksumService.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.ComunicationProtocol.Contracts
{
    /// <summary>
    /// Provides method for calculating check sum of given data
    /// </summary>
    public interface IChecksumService
    {
        /// <summary>
        /// Method calculating checksum of given Weight scale block
        /// </summary>
        /// <param name="input">block of Weight scale message</param>
        /// <param name="leadBytes">bytes before block (optional)</param>
        /// <param name="trailButes">bytes after block (optional)</param>
        /// <returns>calculated check sum as byte</returns>
        byte CalculateCheckSum(byte[] input, byte[] leadBytes = null, byte[] trailButes = null);
    }
}