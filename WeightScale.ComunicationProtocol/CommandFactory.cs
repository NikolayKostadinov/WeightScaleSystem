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
    public class CommandFactory : ICommandFactory
    {
        private readonly IChecksumService checkSumService;
        private readonly IComSerializer serializer;

        /// <summary>
        /// Dependency injection constructor. Initializes a new instance of the <see cref="CommandFactory" /> class.
        /// </summary>
        /// <param name="checkSumServiceParam">concrete implementation of IChecksumService interface</param>
        public CommandFactory(IChecksumService checkSumServiceParam, IComSerializer serializerParam)
        {
            this.checkSumService = checkSumServiceParam;
            this.serializer = serializerParam;
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
            barr.AddRange(inputObject.ToBlock(this.serializer));
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

        public bool CheckMeasurementDataFromWeightScale(int blockLen, int weightScaleNumber, byte[] serializedMessage)
        {
            // The Payload of the protocol
            int payload = 5;

            // Offset of block from the beginning of serializedMessage
            int startOffset = 3;
            var len = serializedMessage.Length;

            byte[] block = this.GetBlock(serializedMessage, blockLen, startOffset);
            byte etx = serializedMessage[startOffset + blockLen];
            byte expectedCheckSum = this.checkSumService.CalculateCheckSum(block, null, new byte[] { (byte)ComunicationConstants.Etx });
            byte actualCheckSum = serializedMessage[serializedMessage.Length - 1];

            var result = (len == (blockLen + payload)) &&
                    (serializedMessage[0] == (byte)ComunicationConstants.Soh) &&
                    (serializedMessage[1] == weightScaleNumber) &&
                    (serializedMessage[2] == (byte)ComunicationConstants.Stx) &&
                    (etx == (byte)ComunicationConstants.Etx) &&
                    (actualCheckSum == expectedCheckSum);

            return result;
        }

        /// <summary>
        /// Gets the block.
        /// </summary>
        /// <param name="serializedMessage">The serialized message.</param>
        /// <param name="blockLen">The block len.</param>
        /// <returns>>String representation of the "WeightScaleNew" object</returns>
        private byte[] GetBlock(byte[] serializedMessage, int blockLen, int offset)
        {
            var result = new byte[blockLen];
            Array.Copy(serializedMessage, offset, result, 0, blockLen);
            return result;
        }

        private byte CalculateCheckSum(IBlock inputObject)
        {
            var checkSum =
                this.checkSumService.CalculateCheckSum(
                    inputObject.ToBlock(this.serializer),
                    null,
                    new byte[] { (byte)ComunicationConstants.Etx });
            return checkSum;
        }
    }
}