namespace WeightScale.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.IO.Ports;
    using WeightScale.Application.Properties;

    public class ComManager : IDisposable
    {
        private SerialPort port;
        private int receiveBufferTreshold;

        #region Constructors
        public ComManager()
            : this(
            Settings.Default.PortName,
            Settings.Default.BaudRate,
            Settings.Default.Parity,
            Settings.Default.DateBits,
            Settings.Default.StopBits
            )
        { }

        public ComManager(string portName, int baudRate, Parity parity, int dateBits, StopBits stopBits)
        {
            this.port = new SerialPort(portName, baudRate, parity, dateBits, stopBits);
        }
        #endregion

        #region Properties
        public SerialDataReceivedEventHandler DataReceivedHandler
        {
            set
            {
                this.port.DataReceived += value;
            }
        }

        public int ReceiveBytesThreshold
        {
            get { return this.receiveBufferTreshold; }
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

        public string PortName { get { return this.port.PortName; } }
        #endregion

        #region Methods
        public void Open()
        {
            if (port.IsOpen)
            {
                throw new InvalidOperationException(string.Format("The port {0} is already opened!", port.PortName));
            }

            port.Open();

            if (!port.IsOpen)
            {
                throw new InvalidOperationException(string.Format("Cannot Open {0} port.", port.PortName));
            }
        }

        public void SendComman(byte[] command, int receiveBufferSize)
        {
            if (receiveBufferSize >0)
            {
                this.ReceiveBytesThreshold = receiveBufferSize;
            }
          
            port.Write(command, 0, command.Length);
        }

        public void SendComman(byte[] command)
        {
            port.Write(command, 0, command.Length);
        }

        public byte[] Read() 
        {
            var result = new byte[this.ReceiveBytesThreshold];
            port.Read(result, 0, result.Length);
            return result;
        }

        public byte[] ReadAll() 
        {
            var result = new byte[this.port.BytesToRead];
            port.Read(result, 0, result.Length);
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
