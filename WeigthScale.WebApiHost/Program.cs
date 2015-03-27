

namespace WeigthScale.WebApiHost
{
    using System;
    using System.Web.Http;
    using System.Web.Http.SelfHost;
    using Ninject;
    using WeightScale.Application.AppStart;
    using log4net;
    using Ninject.Web.Common.SelfHost;
    class Program
    {
        private static ILog logger;
        static void Main(string[] args)
        {
            App_Start();
        }

        internal static void App_Start()
        {
            try
            {
                logger = LogManager.GetLogger("WeigthScale.WebApiHost");

                Uri baseAddress = new Uri("http://localhost:8123/");
                // var assembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
                // string path = assembly.Substring(0, assembly.LastIndexOf("\\")) + "\\WeightScale.WebApi.dll";
                var config = new HttpSelfHostConfiguration(baseAddress);
                //config.Services.Replace(typeof(IAssembliesResolver), new CustomAssemblyResolver(path));

                config.Routes.MapHttpRoute(
                    name: "DefaultyMeasurementRouteGet",
                    routeTemplate: "GetTest",
                    defaults: new { controller = "Measurements", action = "GetTest" }
                );

                config.Routes.MapHttpRoute(
                    name: "DefaultyMeasurementRoute",
                    routeTemplate: "{controller}/{action}",
                    defaults: new { controller="Measurements", action = "PostMeasurement" }
                );

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
                using (var selfHost = new NinjectSelfHostBootstrapper(CreateKernel, config))
                {
                    selfHost.Start();
                    Console.WriteLine("Press Enter to quit.");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
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
