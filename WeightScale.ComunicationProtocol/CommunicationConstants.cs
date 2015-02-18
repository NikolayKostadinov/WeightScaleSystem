//---------------------------------------------------------------------------------
// <copyright file="CommunicationConstants.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.ComunicationProtocol
{
    using System;
    using System.Linq;

    /// <summary>
    /// Provides named constants for Weight scale messages construction
    /// </summary>
    public enum CommunicationConstants : byte
    {
        /// <summary>
        /// Start of heading
        /// </summary>
        Soh = 0x01,

        /// <summary>
        /// Start of text
        /// </summary>
        Stx = 0x02,

        /// <summary>
        /// End of text
        /// </summary>
        Etx = 0x03,

        /// <summary>
        /// End of transmit
        /// </summary>
        Eot = 0x04,

        /// <summary>
        /// Enquiry
        /// </summary>
        Enq = 0x05,

        /// <summary>
        /// Acknowledge
        /// </summary>
        Ack = 0x06,

        /// <summary>
        /// Negative acknowledge
        /// </summary>
        Nac = 0x15,

        /// <summary>
        /// Poll
        /// </summary>
        Pol = 0x70,
    }
}
