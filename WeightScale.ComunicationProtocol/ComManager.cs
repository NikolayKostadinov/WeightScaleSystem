//---------------------------------------------------------------------------------
// <copyright file="ComManager.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.ComunicationProtocol
{
    using System;
    using System.IO.Ports;
    using System.Linq;
    using System.Text.RegularExpressions;
    using WeightScale.ComunicationProtocol.Contracts;

    /// <summary>
    /// Custom serial port manager class
    /// </summary>
    public class ComManager : IDisposable, WeightScale.ComunicationProtocol.Contracts.IComManager
    {
        private const string PORT_NAME_PATTERN = @"\bCOM\d+\b";
        private const int DATA_BITS_MIN_VALUE = 5;
        private const int DATA_BITS_MAX_VALUE = 8;
        private readonly SerialPort port;
        private int receiveBufferTreshold;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ComManager" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public ComManager(IComSettingsProvider settings)
            : this(
                settings.PortName,
                settings.BaudRate,
                settings.Parity,
                settings.DataBits,
                settings.StopBits)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComManager" /> class.
        /// </summary>
        /// <param name="portName">Name of the port.</param>
        /// <param name="baudRate">The baud rate.</param>
        /// <param name="parity">The parity.</param>
        /// <param name="dataBits">The data bits.</param>
        /// <param name="stopBits">The stop bits.</param>
        public ComManager(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            if (DATA_BITS_MIN_VALUE > dataBits || dataBits > DATA_BITS_MAX_VALUE)
            {
                string message = string.Format(
                    "The value of dataBits must be between {0} and {1}.\nActual value is {2}.",
                    DATA_BITS_MIN_VALUE,
                    DATA_BITS_MAX_VALUE,
                    dataBits);
                throw new ArgumentOutOfRangeException("dataBits", message);
            }

            MatchCollection mc = Regex.Matches(portName, PORT_NAME_PATTERN);
            if (mc.Count != 1)
            {
                throw new ArgumentException("portName", "Invalid serial port name " + portName + "!");
            }

            this.port = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
        }

        #endregion

        #region Properties
        public bool IsOpen
        {
            get
            {
                return this.port.IsOpen;
            }
        }

        public SerialDataReceivedEventHandler DataReceivedHandler
        {
            set
            {
                this.port.DataReceived += value;
            }
        }

        public int ReceiveBytesThreshold
        {
            get
            {
                return this.receiveBufferTreshold;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("ReceiveBufferSize");
                }

                if (value < 1)
                {
                    this.port.ReceivedBytesThreshold = 1;
                    this.receiveBufferTreshold = value;
                }
                else
                {
                    this.port.ReceivedBytesThreshold = value;
                    this.receiveBufferTreshold = value;
                }
            }
        }

        public string PortName
        {
            get
            {
                return this.port.PortName;
            }
        }

        #endregion

        #region Methods

        public void Open()
        {
            if (this.port.IsOpen)
            {
                throw new InvalidOperationException(string.Format("The port {0} is already opened!", this.port.PortName));
            }

            this.port.Open();

            if (!this.port.IsOpen)
            {
                throw new InvalidOperationException(string.Format("Cannot Open {0} port.", this.port.PortName));
            }
        }

        public void SendComman(byte[] command, int receiveBufferSize)
        {
            if (receiveBufferSize > 0)
            {
                this.ReceiveBytesThreshold = receiveBufferSize;
            }

            this.port.Write(command, 0, command.Length);
        }

        public void SendComman(byte[] command)
        {
            this.port.Write(command, 0, command.Length);
        }

        public byte[] Read()
        {
            var result = new byte[this.ReceiveBytesThreshold];
            this.port.Read(result, 0, result.Length);
            return result;
        }

        public byte[] ReadAll()
        {
            var result = new byte[this.port.BytesToRead];
            this.port.Read(result, 0, result.Length);
            return result;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing,
        /// or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.port.Close();
            this.port.Dispose();
        }
        #endregion
    }
}