namespace WeightScale.MeasurementWindowsService
{
    using System;
    using System.Linq;
    using System.ServiceProcess;
    using System.Web.Http;
    using System.Web.Http.Dispatcher;
    using System.Web.Http.SelfHost;
    using log4net;
    using WeightScale.WebApi;

    public partial class MeasurementService : ServiceBase
    {
        private static ILog logger;
        private HttpSelfHostConfiguration config = null;
        private HttpSelfHostServer server = null;

        public MeasurementService()
        {
            InitializeComponent();

            logger = LogManager.GetLogger("WeightScale.MeasurementService");
        }

        protected override void OnStart(string[] args)
        {
            logger.Info("WeightScale.MeasurementService is started!");
            StartSelfHostedWebApiServer();
        }

        protected override void OnStop()
        {
            logger.Info("WeightScale.MeasurementService is stopped!");
            try
            {
                server.CloseAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }

        private void StartSelfHostedWebApiServer()
        {
            try
            {
                Uri baseAddress = new Uri("http://localhost:8123/");

                var assembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string path = assembly.Substring(0, assembly.LastIndexOf("\\")) + "\\WeightScale.WebApi.dll";

                config = new HttpSelfHostConfiguration(baseAddress);
                config.Services.Replace(typeof(IAssembliesResolver), new CustomAssemblyResolver(path));
                config.Routes.MapHttpRoute(
                    name: "",
                    routeTemplate: "api/{controller}/PostMeasurement"
                );
                config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    //routeTemplate: "{controller}/{action}/{id}",
                    routeTemplate: "api/{controller}/{action}/{id}",
                    defaults: new { id = RouteParameter.Optional }
                );

                server = new HttpSelfHostServer(config);
                server.OpenAsync().Wait();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }
    }
}
