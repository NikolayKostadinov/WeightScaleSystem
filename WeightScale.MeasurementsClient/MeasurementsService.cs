namespace WeightScale.MeasurementsClient
{
    using System;
    using System.Linq;
    using System.ServiceProcess;
    using System.Threading;
    using log4net;

    public partial class MeasurementsService : ServiceBase
    {
        private readonly ILog logger;

        private Timer logFilesSynhTimer = null;
        private static readonly object lockObject = new object();

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

                logFilesSynhTimer = new Timer(LogFilesProcessingCallback, null, Properties.Settings.Default.TimerDueTime, Timeout.Infinite);

                logger.Info("Service started!");
                MeasurementClientMain.StopProcessMeasurementThread = false;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }

        private void LogFilesProcessingCallback(object stateInfo)
        {
            lock (lockObject)
            {
                try
                {
                    logFilesSynhTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    MeasurementClientMain.ProcessLogs();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message, ex);
                }
                finally
                {
                    logFilesSynhTimer.Change(Properties.Settings.Default.TimerDueTime, System.Threading.Timeout.Infinite);
                }
            }
        }

        protected override void OnStop()
        {
            MeasurementClientMain.StopProcessMeasurementThread = true;
            logger.Info("Service stopped!");
        }
    }
}
