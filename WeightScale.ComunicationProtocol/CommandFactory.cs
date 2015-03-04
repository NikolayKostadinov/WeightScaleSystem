//---------------------------------------------------------------------------------
// <copyright file="CommandFactory.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.ComunicationProtocol
{
    using System;
    using System.Collections.Generic;
    using WeightScale.ComunicationProtocol.Contracts;
    using WeightScale.Domain.Abstract;

    /// <summary>
    /// Provides methods which constructs binary messages sent to the WeightScale
    /// </summary>
    public class CommandFactory
    {
        private readonly IChecksumService checkSumService;

        /// <summary>
        /// Dependency injection constructor. Initializes a new instance of the <see cref="CommandFactory" /> class.
        /// </summary>
        /// <param name="checkSumServiceParam">concrete implementation of IChecksumService interface</param>
        public CommandFactory(IChecksumService checkSumServiceParam)
        {
            this.checkSumService = checkSumServiceParam;
        }

        public byte[] WeightScaleRequest(IBlock inputObject)
        {
            List<byte> barr = new List<byte>();
            barr.Add((byte)ComunicationConstants.Soh);
            barr.Add(inputObject.Number);
            barr.Add((byte)ComunicationConstants.Pol);
            barr.Add((byte)ComunicationConstants.Enq);
            return barr.ToArray();
        }

        public byte[] SendDataToWeightScale(IBlock inputObject)
        {
            List<byte> barr = new List<byte>();
            barr.Add((byte)ComunicationConstants.Soh);
            barr.Add(inputObject.Number);
            barr.Add((byte)ComunicationConstants.Stx);
            barr.AddRange(inputObject.ToBlock());
            barr.Add((byte)ComunicationConstants.Etx);
            var checkSum = this.CalculateCheckSum(inputObject);
            barr.Add(checkSum);
            barr.Add((byte)ComunicationConstants.Enq);
            return barr.ToArray();
        }

        public byte[] EndOfTransmit()
        {
            return new byte[] { (byte)ComunicationConstants.Eot };
        }

        public byte[] Acknowledge()
        {
            return new byte[] { (byte)ComunicationConstants.Ack };
        }

        public byte[] NegativeAcknowledge()
        {
            return new[] { (byte)ComunicationConstants.Nac };
        }

        private byte CalculateCheckSum(IBlock inputObject)
        {
            var checkSum =
                this.checkSumService.CalculateCheckSum(
                    inputObject.ToBlock(),
                    null,
                    new byte[] { (byte)ComunicationConstants.Etx });
            return checkSum;
        }
    }
}