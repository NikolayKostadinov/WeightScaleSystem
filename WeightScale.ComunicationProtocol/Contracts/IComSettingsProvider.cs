//---------------------------------------------------------------------------------
// <copyright file="IComSettingsProvider.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.ComunicationProtocol.Contracts
{
    using System;

   public interface IComSettingsProvider
    {
        int BaudRate { get; set; }

        int DataBits { get; set; }

        System.IO.Ports.Parity Parity { get; set; }

        string PortName { get; set; }

        System.IO.Ports.StopBits StopBits { get; set; }
    }
}
