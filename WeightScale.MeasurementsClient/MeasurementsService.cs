namespace WeightScale.MeasurementsClient
{
    using System;
    using System.Linq;
    using System.ServiceProcess;
    using System.Threading;
    using log4net;

    public partial class MeasurementsService : ServiceBase
    {
        //private Timer measurementsSynhTimer = null;
        //private static readonly object lockObject = new object();
        private readonly ILog logger;

        public MeasurementsService(ILog loggerParam)
        {
            InitializeComponent();
            this.logger = loggerParam;

        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Thread measurementThread = new Thread(new ThreadStart(MeasurementClientMain.ProcessMeasurements));
                measurementThread.Start();

                Thread logsThread = new Thread(new ThreadStart(MeasurementClientMain.ProcessLogs));
                logsThread.Start();

                logger.Info("Service started!");
                MeasurementClientMain.StopProcessMeasurementThread = false;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }


        }

        protected override void OnStop()
        {
            MeasurementClientMain.StopProcessMeasurementThread = true;
            logger.Info("Service stopped!");
        }
    }
}
