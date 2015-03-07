namespace WeightScale.ComunicationProtocol.CommunicationPort
{
    using System;
    using System.IO.Ports;
    using System.Text.RegularExpressions;

    public class CommunicationPortManager
    {
        private const string PortNamePattern = @"\bCOM\d+\b";

        private readonly SerialPort comPort;
        private string portName;
        public string PortName 
        {
            get 
            {
                return this.portName;
            }
            set
            {
                MatchCollection mc = Regex.Matches(value, PortNamePattern);

                if (mc.Count != 1)
                {
                    throw new ArgumentException("Invalid serial port name!");
                }

                this.portName = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public CommunicationPortManager() 
        {
            this.comPort = new SerialPort();
            this.comPort.DataReceived += ComPortDataReceived;
        }

        public CommunicationPortManager(string portName)
        {
            this.PortName = portName;

            this.comPort = new SerialPort();
            this.comPort.DataReceived += ComPortDataReceived;
        }

        void Open()
        {
            if (!this.comPort.IsOpen)
            {
                this.comPort.PortName = this.PortName;

                this.comPort.Open();
            }
        }

        void Close()
        {
            this.comPort.Close();
        }

        void Write()
        {
            throw new System.NotImplementedException();
        }

        void Read()
        {
            throw new System.NotImplementedException();
        }

        void ComPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
