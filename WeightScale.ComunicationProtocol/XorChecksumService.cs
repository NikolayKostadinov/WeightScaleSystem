//---------------------------------------------------------------------------------
// <copyright file="XorChecksumService.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.ComunicationProtocol
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using WeightScale.ComunicationProtocol.Contracts;

    /// <summary>
    /// Provides method calculating check sum for Weight scale messages 
    /// </summary>
    public class XorChecksumService : IChecksumService
    { 
        /// <summary>
        /// Calculates the check sum.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="leadBytes">The lead bytes.</param>
        /// <param name="trailBytes">The trail bytes.</param>
        /// <returns>calculated check sum as byte</returns>
        public byte CalculateCheckSum(byte[] input, byte[] leadBytes = null, byte[] trailBytes = null)
        {
            List<byte> buffer = new List<byte>();

            if (leadBytes != null && leadBytes.Count() > 0)
            {
                buffer.AddRange(leadBytes);
            }

            buffer.AddRange(input);

            if (trailBytes != null && trailBytes.Count() > 0)
            {
                buffer.AddRange(trailBytes);
            }

            byte result = buffer[0];
           
            for (int i = 1; i < buffer.Count(); i++)
            {
                result = Convert.ToByte(result ^ buffer[i]);
            }

            result = Convert.ToByte(result ^ 128);
            return result;
        }
    }
}