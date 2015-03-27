

namespace WeigthScale.WebApiHost
{
    using System.ServiceProcess;

    partial class WebApiHostService : ServiceBase
    {
        public WebApiHostService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Program.App_Start();
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }
    }
}
