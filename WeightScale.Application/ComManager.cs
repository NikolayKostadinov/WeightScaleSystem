namespace WeightScale.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.IO.Ports;
    using WeightScale.Application.Properties;

    public class ComManager
    {
        private SerialPort port;

        public ComManager()
            : this(
            Settings.Default.PortName,
            Settings.Default.BaudRate,
            Settings.Default.Parity,
            Settings.Default.DateBits,
            Settings.Default.StopBits
            )
        {}

        public ComManager(string portName, int baudRate, Parity parity, int dateBits, StopBits stopBits)
        {
            this.port = new SerialPort(portName, baudRate, parity, dateBits, stopBits);
        }

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

        public SerialDataReceivedEventHandler DataReceivedHandler 
        {
            set 
            {
                this.port.DataReceived += value;
            }
        }

        public void SendComman(byte[] command, int receiveBufferSize)
        {
            port.ReceivedBytesThreshold = receiveBufferSize;
            port.Write(command, 0, command.Length);
        }
    }
}
