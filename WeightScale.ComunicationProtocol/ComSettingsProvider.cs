namespace WeightScale.ComunicationProtocol
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO.Ports;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using WeightScale.ComunicationProtocol.Contracts;

    /// <summary>
    /// 
    /// </summary>
    public class ComSettingsProvider : IComSettingsProvider
    {
        private static object LockObj = new object();
        private static ComSettingsProvider settings;
        private ComSettingsProvider(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            this.PortName = portName;
            this.BaudRate = baudRate;
            this.Parity = parity;
            this.DataBits = dataBits;
            this.StopBits = stopBits;
        }
        public string PortName { get; set; }

        public int BaudRate { get; set; }

        public Parity Parity { get; set; }

        public int DataBits { get; set; }

        public StopBits StopBits { get; set; }

        /// <summary>
        /// Gets the COM settings provider.
        /// </summary>
        /// <returns>
        /// ComSettingsProvider with the settings witch you described in your App.config or Web.Config.
        /// If you don't describe settings the default settings will be:
        /// PortName: "COM1"
        /// BoudRate: 4800
        /// Parity: Parity.Even
        /// DataBits: 8
        /// StopBits: StopBits.One
        /// </returns>
        public static ComSettingsProvider GetComSettingsProvider
        {
            // Singleton
            // Todo: refactore to be a real singleton !!! 
            get
            {
                if (settings == null)
                {
                    lock (LockObj)
                    {
                        if (settings == null)
                        {
                            settings = GetComSettingsFromCurrentApplication();
                        }
                    }
                }

                return settings;
            }
        }

        /// <summary>
        /// Gets the COM settings from current application.
        /// </summary>
        /// <returns>ComSettingsProvider</returns>
        private static ComSettingsProvider GetComSettingsFromCurrentApplication()
        {
            string exeFileName = Assembly.GetEntryAssembly().Location;
            //string configFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(exeFileName);
            ConfigurationSectionGroup appSettingsGroup = configuration.GetSectionGroup("applicationSettings");
            if (appSettingsGroup != null)
            {
                ConfigurationSection appSettingsSection = appSettingsGroup.Sections[0]; // also may be used a name
                ClientSettingsSection settings = appSettingsSection as ClientSettingsSection;

                string portName = "COM1";
                if (CheckIfSettingExists(settings.Settings.Get("PortName")))
                {
                    portName = settings.Settings.Get("PortName").Value.ValueXml.InnerText;
                }

                int baudRate = 4800;
                if (CheckIfSettingExists(settings.Settings.Get("BaudRate")))
                {
                    baudRate = Convert.ToInt32(settings.Settings.Get("BaudRate").Value.ValueXml.InnerText);
                }

                Parity parity = Parity.Even;
                if (CheckIfSettingExists(settings.Settings.Get("Parity")))
                {
                    parity = (Parity)Enum.Parse(typeof(Parity), settings.Settings.Get("Parity").Value.ValueXml.InnerText);
                }

                int dataBits = 8;
                if (CheckIfSettingExists(settings.Settings.Get("BaudRate")))
                {
                    dataBits = Convert.ToInt32(settings.Settings.Get("DataBits").Value.ValueXml.InnerText);
                }

                StopBits stopBits = StopBits.One;
                if (CheckIfSettingExists(settings.Settings.Get("BaudRate")))
                {
                    stopBits = (StopBits)Enum.Parse(typeof(StopBits), settings.Settings.Get("StopBits").Value.ValueXml.InnerText);
                }

                return new ComSettingsProvider(portName, baudRate, parity, dataBits, stopBits);
            }
            else
            {
                return new ComSettingsProvider("COM1", 4800, Parity.Even, 8, StopBits.One);
            }
        }


        /// <summary>
        /// Checks if setting exists.
        /// </summary>
        /// <param name="get">Checked setting</param>
        /// <returns></returns>
        private static bool CheckIfSettingExists(SettingElement setting)
        {
            if (setting == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(setting.Value.ValueXml.InnerText))
            {
                return false;
            }
            return true;
        }
    }
}
