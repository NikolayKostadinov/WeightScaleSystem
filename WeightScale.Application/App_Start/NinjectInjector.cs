//  ------------------------------------------------------------------------------------------------
//   <copyright file="NinjectInjector.cs" company="Business Management System Ltd.">
//       Copyright "2019" (c), Business Management System Ltd.
//       All rights reserved.
//   </copyright>
//   <author>Nikolay.Kostadinov</author>
//  ------------------------------------------------------------------------------------------------

namespace WeightScale.Application.AppStart
{
    #region Using

    using System.Web.Http.Tracing;

    using log4net;

    using Ninject;
    using Ninject.Web.Common;

    using WeightScale.Application.Contracts;
    using WeightScale.Application.Services;
    using WeightScale.CacheApi.Concrete;
    using WeightScale.CacheApi.Contract;
    using WeightScale.CacheApi.SoapProxy;
    using WeightScale.ComunicationProtocol;
    using WeightScale.ComunicationProtocol.Contracts;
    using WeightScale.Domain.Abstract;
    using WeightScale.Domain.Common;
    using WeightScale.LogFileService;

    #endregion

    /// <summary>
    ///     Dependency injector provider
    /// </summary>
    public class NinjectInjector
    {
        private static readonly object LockObj = new object();

        private static NinjectInjector injector;

        private static IKernel kernel;

        /// <summary>
        ///     Prevents a default instance of the <see cref="NinjectInjector" /> class from being created.
        /// </summary>
        private NinjectInjector()
        {
            kernel = new StandardKernel();
            InitializeKernel(kernel);
        }

        public static IKernel GetInjector
        {
            get
            {
                if (injector == null)
                {
                    lock (LockObj)
                    {
                        if (injector == null)
                        {
                            injector = new NinjectInjector();
                        }
                    }
                }

                return kernel;
            }
        }

        /// <summary>
        ///     Initializes the kernel.
        /// </summary>
        public static void InitializeKernel(IKernel kernel)
        {
            kernel.Bind<IComSerializer>().To<ComSerializer>().InRequestScope();
            kernel.Bind<IChecksumService>().To<XorChecksumService>().InRequestScope();
            kernel.Bind<ICommandFactory>().To<CommandFactory>().InRequestScope();
            kernel.Bind<IComSettingsProvider>().ToMethod(context => ComSettingsProvider.GetComSettingsProvider)
                .InRequestScope();
            kernel.Bind<IComManager>().To<ComManager>().InSingletonScope();
            kernel.Bind<IValidationMessageCollection>().To<ValidationMessageCollection>().InRequestScope();
            kernel.Bind<ILog>().ToMethod(context => LogManager.GetLogger(context.Request.Target.Member.ReflectedType))
                .InRequestScope();
            kernel.Bind<ILog>().ToMethod(context => LogManager.GetLogger("WebApiTrace"))
                .WhenInjectedExactlyInto(typeof(CustomTraceWriter)).InRequestScope();
            kernel.Bind<ILog>().ToMethod(context => LogManager.GetLogger("WebApiTrace"))
                .WhenInjectedExactlyInto(typeof(LogRequestAndResponseHandler)).InRequestScope();
            kernel.Bind<IMeasurementService>().To<MeasurementService>().InRequestScope();

            // service for test kernel.Bind<IMeasurementService>().To<MockedMeasurementService>().InRequestScope();
            kernel.Bind<IWeightScaleMessageDto>().To<WeightScaleMessageDto>().InRequestScope();
            kernel.Bind<IRepository<SoapMessage, CValidationMessage>>().To<MeasurementRequestsRepository>();
            kernel.Bind<IJsonDeserializeService>().To<JsonDeserializeService>();
            kernel.Bind<IMappingService>().To<MappingService>();
            kernel.Bind<IFileService>().To<LogFileService>();
            kernel.Bind<ITraceWriter>().To<CustomTraceWriter>();
        }
    }
}