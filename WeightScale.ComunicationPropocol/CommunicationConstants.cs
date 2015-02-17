//---------------------------------------------------------------------------------
// <copyright file="CommunicationConstants.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.ComunicationPropocol
{
    using System;
    using System.Linq;

    /// <summary>
    /// Provides named constants for Weight scale messages construction
    /// </summary>
    public enum CommunicationConstants 
    {
        /// <summary>
        /// Start of heading
        /// </summary>
        Soh = '\x01',

        /// <summary>
        /// Start of text
        /// </summary>
        Stx = '\x02',

        /// <summary>
        /// End of text
        /// </summary>
        Etx = '\x03',

        /// <summary>
        /// End of transmit
        /// </summary>
        Eot = '\x04',

        /// <summary>
        /// Enquiry
        /// </summary>
        Enq = '\x05',

        /// <summary>
        /// Acknowledge
        /// </summary>
        Ack = '\x06',

        /// <summary>
        /// Negative acknowledge
        /// </summary>
        Nac = '\x15',

        /// <summary>
        /// Poll
        /// </summary>
        Pol = 'p',
    }
}
