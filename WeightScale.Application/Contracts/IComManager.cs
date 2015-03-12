using System;

namespace WeightScale.Application.Contracts
{
    public interface IComManager
    {
        System.IO.Ports.SerialDataReceivedEventHandler DataReceivedHandler { set; }
        void Dispose();
        void Open();
        string PortName { get; }
        byte[] Read();
        byte[] ReadAll();
        int ReceiveBytesThreshold { get; set; }
        void SendComman(byte[] command);
        void SendComman(byte[] command, int receiveBufferSize);
    }
}
