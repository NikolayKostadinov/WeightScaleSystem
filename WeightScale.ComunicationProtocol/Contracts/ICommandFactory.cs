using System;
using WeightScale.Domain.Abstract;

namespace WeightScale.ComunicationProtocol.Contracts
{
    public interface ICommandFactory
    {
        byte[] Acknowledge();
        byte[] EndOfTransmit();
        byte[] NegativeAcknowledge();
        byte[] SendDataToWeightScale(IBlock inputObject);
        byte[] WeightScaleRequest(IBlock inputObject);
        bool CheckMeasurementDataFromWeightScale(int blockLen,int weightScaleNumber , byte[] serializedMessage);
    }
}
