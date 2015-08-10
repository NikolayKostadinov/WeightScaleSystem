

namespace WeigthScale.WebApiHost
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.ServiceProcess;
    using System.Web.Http;
    using System.Web.Http.Dispatcher;
    using System.Web.Http.SelfHost;
    using System.Web.Http.Tracing;
    using Newtonsoft.Json.Serialization;
    using Ninject;
    using WeightScale.Application.AppStart;
    using WeightScale.Application.Services;
    using WeigthScale.WebApiHost.Infrastructure;
    using log4net;
    using Ninject.Web.Common.SelfHost;

    class WebApiHostMain
    {
        private static ILog logger;

        private static volatile bool stopWebApiServer;

        public static bool StopWebApiServer
        {
            get { return stopWebApiServer; }
            set { stopWebApiServer = value; }
        }

        static WebApiHostMain() 
        {
            logger = LogManager.GetLogger("WeigthScale.WebApiHost");
            StopWebApiServer = false;
        }

        static void Main(string[] args)
        {
            ServiceBase[] servicesToRun;
            servicesToRun = new ServiceBase[] 
            { 
                new WebApiHostService(logger) 
            };
            ServiceBase.Run(servicesToRun);

            // !!!! Comment the lines before and uncomment next line if you want to compile application as a console.
            // App_Start();
        }

        /// <summary>
        /// start a self-hosted web server with methods for working and monitoring of the electron weight scale
        /// </summary>
        internal static void App_Start()
        {
            try
            {
                string uri = string.Format("{0}:{1}", 
                    Properties.Settings.Default.SelfHostedWebApiHost, 
                    Properties.Settings.Default.SelfHostedWebApiPort);
                Uri baseAddress = new Uri(uri);
                var config = new HttpSelfHostConfiguration(baseAddress);

                config.Routes.MapHttpRoute(
                    name: "",
                    routeTemplate: "api/{controller}/{action}",
                    defaults: new { action = "PostMeasurement" }
                );
                config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{action}/{id}",
                    defaults: new { id = RouteParameter.Optional }
                );
                config.Filters.Add(new HandleErrorAttribute(logger));

                config.MessageHandlers.Add(new LogRequestAndResponseHandler(LogManager.GetLogger("WebApiTrace")));

                using (var selfHost = new NinjectSelfHostBootstrapper(CreateKernel, config))
                {
                    selfHost.Start();
                    StartRequest();
                    logger.Info("WebApi SelfHosted thread is started!");
                    while (!StopWebApiServer) { }
                    logger.Info("SelfHosted WebApi service is stopped!");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }
 
        /// <summary>
        /// Starts the request.
        /// </summary>
        private static void StartRequest()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string url = string.Format("{0}:{1}/{2}",
                    Properties.Settings.Default.SelfHostedWebApiHost,
                    Properties.Settings.Default.SelfHostedWebApiPort,
                    "api/Measurements/GetTest");
            


            HttpResponseMessage response = null;
            try
            {
                response = client.GetAsync(url).Result;
                if (response!=null && response.Content!=null)
                {
                    logger.Info("MeasurementsController was initialized successfully");
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("{0}\n{1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// Creates the kernel.
        /// </summary>
        /// <returns>the newly created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = NinjectInjector.GetInjector;
            return kernel;
        }
    }
}
