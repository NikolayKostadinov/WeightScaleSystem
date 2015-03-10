namespace WeightScale.Application.CommunicationPort
{
    using System;
    using System.IO.Ports;
    using System.Text.RegularExpressions;

    using WeightScale.Application.Properties;
    using WeightScale.Utility.Helpers;

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
                //if (value <= DATA_BITS_MIN_VALUE || value >= DATA_BITS_MAX_VALUE)
                //{
                //    throw new ArgumentOutOfRangeException("The data bits value is less than 5 or more than 8!");
                //}

                dataBits = value;
            }
        }

        public StopBits StopBits { get; set; }

        public int ReadTimeout { get; set; }

        public int WriteTimeout { get; set; }

        public bool IsOpen { get; set; }

        private byte[] readedBytes;
        public byte[] ReadedBytes
        {
            get
            {
                return readedBytes;
            }
        }

        public CommunicationPortManager()
        {
            // read default serial port's properties from configuration file ot other source
            ReadSettingg();
            this.IsOpen = false;
            this.comPort = new SerialPort();
            this.comPort.DataReceived += ComPortDataReceived;
        }

        public CommunicationPortManager(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits,
            int readTimeout, int writeTimeout)
            : this()
        {
            this.PortName = portName;
            this.BaudRate = baudRate;
            this.Parity = parity;
            this.DataBits = dataBits;
            this.StopBits = stopBits;
            this.ReadTimeout = readTimeout;
            this.WriteTimeout = writeTimeout;
        }

        private void ReadSettingg()
        {
            Settings settings = Settings.Default;
            this.PortName = settings.PortName;
            this.BaudRate = settings.BaudRate;
            this.Parity = settings.Parity;
            this.DataBits = settings.DateBits;
            this.StopBits = settings.StopBits;
            this.ReadTimeout = settings.ReadTimeout;
            this.WriteTimeout = settings.WriteTimeout;
        }

        private bool WriteSettings()
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            if (!this.IsOpen)
            {
                // need to exclude all these and set only serial port values into constructor
                this.comPort.PortName = this.PortName;
                this.comPort.BaudRate = this.BaudRate;
                this.comPort.Parity = this.Parity;
                this.comPort.DataBits = this.DataBits;
                this.comPort.StopBits = this.StopBits;

                this.comPort.Open();
                this.IsOpen = true;
            }
        }

        public void Close()
        {
            this.comPort.Close();
            this.IsOpen = false;
        }

        public void Write(string data)
        {
            this.Open();
            if (this.IsOpen)
            {
                byte[] sendData = data.ToByteArray();
                this.comPort.Write(sendData, 0, sendData.Length);
            }
        }

        public void Write(byte[] data)
        {
            this.Open();
            if (this.IsOpen)
            {
                this.comPort.Write(data, 0, data.Length);
            }
        }

        void ComPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // All returned data from serial port is stored in that array of bytes
            int bytesToRead = this.comPort.BytesToRead - 1;
            byte[] comBuffer = new byte[bytesToRead];
            int realReadedBytes = comPort.Read(comBuffer, 0, bytesToRead);

            // sometimes we can't read at once all data from serial port.
            if (realReadedBytes < bytesToRead)
            {
                int diff = bytesToRead - realReadedBytes - 1;
                byte[] diffBuffer = new byte[diff];
                comPort.Read(diffBuffer, 0, diff);
                Buffer.BlockCopy(diffBuffer, 0, comBuffer, realReadedBytes, diff); // need to test
            }

            this.readedBytes = comBuffer;
        }
    }
}
