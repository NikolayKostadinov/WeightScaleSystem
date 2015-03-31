

namespace WeigthScale.WebApiHost
{
    using System.ServiceProcess;
    using System.Threading;
    using log4net;

    partial class WebApiHostService : ServiceBase
    {
        private readonly ILog logger;

        public WebApiHostService(ILog loggerParam)
        {
            InitializeComponent();
            this.logger = loggerParam;
        }

        protected override void OnStart(string[] args)
        {
            Thread t = new Thread(new ThreadStart(WebApiHostMain.App_Start));
            t.Start();
            logger.Info("WebApi Service started");
            WebApiHostMain.StopWebApiServer = false; 
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            // JUST test
            WebApiHostMain.StopWebApiServer = true; 
            logger.Info("WebApi Service stopped");
        }
    }
}
