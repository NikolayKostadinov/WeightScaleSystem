using System;

namespace WeightScale.ComunicationProtocol.Contracts
{
    public interface ICommandFactory
    {
        byte[] Acknowledge();
        byte[] EndOfTransmit();
        byte[] NegativeAcknowledge();
        byte[] SendDataToWeightScale(WeightScale.Domain.Abstract.IBlock inputObject);
        byte[] WeightScaleRequest(WeightScale.Domain.Abstract.IBlock inputObject);
    }
}
