namespace WeightScale.ComunicationProtocol.CommunicationPort
{
    using System;
    using System.IO.Ports;
    using System.Text.RegularExpressions;

    public class CommunicationPortManager
    {
        private const string PORT_NAME_PATTERN = @"\bCOM\d+\b";
        private const int DATA_BITS_MIN_VALUE = 5;
        private const int DATA_BITS_MAX_VALUE = 8;

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
                MatchCollection mc = Regex.Matches(value, PORT_NAME_PATTERN);
                if (mc.Count != 1)
                {
                    throw new ArgumentException("Invalid serial port name!");
                }

                this.portName = value;
            }
        }

        public int BaudRate { get; set; }

        public Parity Parity { get; set; }

        private int dataBits;
        public int DataBits 
        { 
            get
            {
                return dataBits;
            } 
            set
            {
                if (value <= DATA_BITS_MIN_VALUE || value >= DATA_BITS_MAX_VALUE)
                {
                    throw new ArgumentOutOfRangeException("The data bits value is less than 5 or more than 8!");  
                }

                dataBits = value;
            }
        }

        public StopBits StopBits { get; set; }

        public int ReadTimeout { get; set; }

        public int WriteTimeout { get; set; }

        public bool IsOpen { get; set; }

        private  byte[] readedBytes;
        public byte[] ReadedBytes
        {
            get 
            { 
                return readedBytes; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public CommunicationPortManager() 
        {
            // read default serial port's properties from configuration file ot other source

            this.comPort = new SerialPort();
            this.comPort.DataReceived += ComPortDataReceived;
        }

        public CommunicationPortManager(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            this.PortName = portName;
            this.BaudRate = baudRate;
            this.Parity = parity;
            this.DataBits = dataBits;
            this.StopBits = stopBits;
            this.IsOpen = false;

            this.comPort = new SerialPort();
            this.comPort.DataReceived += ComPortDataReceived;
        }

        private bool ReadSettingg()
        {
            throw new NotImplementedException();
        }

        private bool WriteSettings()
        {
            throw new NotImplementedException();
        }

        void Open()
        {
            if (!this.IsOpen)
            {
                this.comPort.PortName = this.PortName;
                this.comPort.BaudRate = this.BaudRate;
                this.comPort.Parity = this.Parity;
                this.comPort.DataBits = this.DataBits;
                this.comPort.StopBits = this.StopBits;

                this.comPort.Open();
                this.IsOpen = true;
            }
        }

        void Close()
        {
            this.comPort.Close();
            this.IsOpen = false;
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
            // All returned data from serial port is stored in that array of bytes
            readedBytes = new byte[] { };
        }
    }
}
