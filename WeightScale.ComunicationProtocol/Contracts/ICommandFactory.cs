//---------------------------------------------------------------------------------
// <copyright file="ICommandFactory.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.ComunicationProtocol.Contracts
{
    using System;
    using WeightScale.Domain.Abstract;

    public interface ICommandFactory
    {
        byte[] Acknowledge();

        byte[] EndOfTransmit();

        byte[] NegativeAcknowledge();

        byte[] SendDataToWeightScale(IBlock inputObject);

        byte[] WeightScaleRequest(IBlock inputObject);

        bool CheckMeasurementDataFromWeightScale(int blockLen, int weightScaleNumber, byte[] serializedMessage);
    }
}
