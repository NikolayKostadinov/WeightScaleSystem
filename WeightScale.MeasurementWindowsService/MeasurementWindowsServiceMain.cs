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
    public class MeasurementWindowsServiceMain
    {
        private static ILog logger;
        private static HttpSelfHostConfiguration config;
        private static HttpSelfHostServer server;

        public static void Main(string[] args)
        {
            logger = LogManager.GetLogger("WeightScale.MeasurementService");
            logger.Info("WeightScale.MeasurementService is started!");
            StartSelfHostedWebApiServer();

            
        }

        private static void StartSelfHostedWebApiServer()
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
                    routeTemplate: "api/{controller}/{action}",
                    defaults: new { action = "PostMeasurement" }
                );
                config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    //routeTemplate: "{controller}/{action}/{id}",
                    routeTemplate: "api/{controller}/{action}/{id}",
                    defaults: new { id = RouteParameter.Optional }
                );

                server = new HttpSelfHostServer(config);
                server.OpenAsync().Wait();
                Console.Read();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }
    }
}
