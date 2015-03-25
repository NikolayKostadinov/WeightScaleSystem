

namespace WeigthScale.WebApiHost
{

    using System;
    using System.Reflection;
    using System.Web.Http;
    using System.Web.Http.Dependencies;
    using System.Web.Http.Dispatcher;
    using System.Web.Http.SelfHost;
    using Ninject;
    using Ninject.Web.Common.SelfHost;
    using Ninject.Web.WebApi;
    using WeightScale.WebApi;
    using WeigthScale.WebApiHost.App_Start;
    using log4net;

    class Program
    {
        private static ILog logger;
        static void Main(string[] args)
        {
            try
            {
                logger = LogManager.GetLogger("WeigthScale.WebApiHost");

                Uri baseAddress = new Uri("http://localhost:8123/");
                var assembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string path = assembly.Substring(0, assembly.LastIndexOf("\\")) + "\\WeightScale.WebApi.dll";
                var config = new HttpSelfHostConfiguration(baseAddress);

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

                var kernel = new StandardKernel();
                

                var resolver = new NinjectDependencyResolver(kernel);

                config.DependencyResolver = resolver;

                var mSelfHost = new NinjectSelfHostBootstrapper(CreateKernel, config);

                mSelfHost.Start();

                Console.ReadLine();

                //var server = new HttpSelfHostServer(config);


                //server.OpenAsync().Wait();

                //Console.ReadKey();
                
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }

        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            NinjectSetup.Config(kernel);
            return kernel;
        }
    }
}
