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
    using WeightScale.ComunicationProtocol;
    using WeightScale.Contracts;

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
            barr.Add((byte)CommunicationConstants.Soh);
            barr.Add(inputObject.WeightScaleId);
            barr.Add((byte)CommunicationConstants.Pol);
            barr.Add((byte)CommunicationConstants.Enq);
            return barr.ToArray();
        }

        public byte[] SendDataToWeightScale(IBlock inputObject)
        {
            List<byte> barr = new List<byte>();
            barr.Add((byte)CommunicationConstants.Soh);
            barr.Add(inputObject.WeightScaleId);
            barr.Add((byte)CommunicationConstants.Stx);
            barr.AddRange(inputObject.ToBlock());
            barr.Add((byte)CommunicationConstants.Etx);
            var checkSum = this.CalculateCheckSum(inputObject);
            barr.Add(checkSum);
            barr.Add((byte)CommunicationConstants.Enq);
            return barr.ToArray();
        }

        public byte[] EndOfTransfer()
        {
            return new byte[] { (byte)CommunicationConstants.Eot };
        }

        public byte[] Acknowledge()
        {
            return new byte[] { (byte)CommunicationConstants.Ack };
        }

        public byte[] NegativeAcknowledge()
        {
            return new[] { (byte)CommunicationConstants.Nac };
        }

        private byte CalculateCheckSum(IBlock inputObject)
        {
            var checkSum =
                this.checkSumService.CalculateCheckSum(
                    inputObject.ToBlock(),
                    null,
                    new byte[] { (byte)CommunicationConstants.Etx });
            return checkSum;
        }
    }
}