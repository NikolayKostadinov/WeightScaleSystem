//---------------------------------------------------------------------------------
// <copyright file="IComManager.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.ComunicationProtocol.Contracts
{
    using System;

    public interface IComManager
    {
        System.IO.Ports.SerialDataReceivedEventHandler DataReceivedHandler { set; }
        bool IsOpen { get; }

        string PortName { get; }

        int ReceiveBytesThreshold { get; set; }

        void Dispose();

        void Open();

        byte[] Read();

        byte[] ReadAll();

        void SendComman(byte[] command);

        void SendComman(byte[] command, int receiveBufferSize);
    }
}
